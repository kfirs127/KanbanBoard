using IntroSE.Kanban.Backend.DataAccessLayer.DalControllers;
using IntroSE.Kanban.Backend.DataAccessLayer.DalObjects;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    
     public abstract class DalController<T> where T : DalObject<T>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected readonly string _connectionString;
        protected const string DatabaseName = "KanbanDB"; 
        private readonly string _tableName;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="tableName"></param>
        public DalController(string tableName)
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "KanbanDB.db"));
            _connectionString = $"Data Source={path}; Version=3;";
            _tableName = tableName;

            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(DatabaseName);
                CreateTables();
            }
        }

        public void DeleteAllData() {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "KanbanDB.db"));
            FileInfo toCheck = new FileInfo(path);
            if (toCheck.Exists)
                toCheck.Delete();
        }

        ///// <summary>
        /// creates the tables in database
        /// </summary>
        private void CreateTables()
        {
            bool succeded = true;
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    CreateUserTable(connection);
                    CreateBoardTable(connection);
                    CreateColumnTable(connection);
                    CreateTaskTable(connection);
                    connection.Close();
                    log.Info("Database was created");
                }
                catch (Exception e)
                {
                    log.Error("Couldn't create tables: " + e.Message);
                    succeded = false;
                }
                finally
                {
                    connection.Close();
                    if (!succeded)
                        throw new Exception("Couldn't create database.");
                }
            }
        }

    

        private void CreateUserTable(SQLiteConnection connection)
        {
            string userTable = $"CREATE TABLE {UserDalController.TableName} ({DalUser.IdColumnName} INTEGER, {DalUser.EmailColumnName} " +
                $"TEXT NOT NULL UNIQUE, {DalUser.PasswordColumnName} TEXT NOT NULL,	{DalUser.NicknameColumnName} TEXT NOT NULL, " +
                $"{DalUser.BoardIdColumnName} INTEGER, PRIMARY KEY({DalUser.IdColumnName}));";
            SQLiteCommand command = new SQLiteCommand(null, connection);
            command.CommandText = userTable;
            command.ExecuteNonQuery();
            command.Dispose();
        }

        private void CreateBoardTable(SQLiteConnection connection)
        {
            string boardTable = $"CREATE TABLE {BoardDalController.TableName} ({DalBoard.IdColumnName} INTEGER PRIMARY KEY, {DalBoard.CreatorColumnName} TEXT NOT NULL, " +
                $"FOREIGN KEY ({DalBoard.CreatorColumnName}) REFERENCES {UserDalController.TableName} ({DalUser.EmailColumnName}));";
            SQLiteCommand command = new SQLiteCommand(null, connection);
            command.CommandText = boardTable;
            command.ExecuteNonQuery();
            command.Dispose();
        }

        private void CreateColumnTable(SQLiteConnection connection)
        {
            string columnTable = $"CREATE TABLE {ColumnDalController.TableName} ({DalColumn.IdColumnName} INTEGER, " +
                $"{DalColumn.BoardIdColumnName} INTEGER, {DalColumn.ColumnOrdinalColumnName} INTEGER NOT NULL, {DalColumn.NameColumnName} " +
                $"TEXT NOT NULL, {DalColumn.LimitColumnName} INTEGER NOT NULL, PRIMARY KEY({DalColumn.IdColumnName}," +
                $"{DalColumn.BoardIdColumnName}), FOREIGN KEY({DalColumn.BoardIdColumnName}) REFERENCES " +
                $"{BoardDalController.TableName} ({DalBoard.IdColumnName}));";
            SQLiteCommand command = new SQLiteCommand(null, connection);
            command.CommandText = columnTable;
            command.ExecuteNonQuery();
            command.Dispose();
        }

        private void CreateTaskTable(SQLiteConnection connection)
        {
            string taskTable = $"CREATE TABLE {TaskDalController.TableName} ({DalTask.IdColumnName} INTEGER, " +
                $" {DalTask.BoardIdColumnName} INTEGER, {DalTask.ColumnIdColumnName} INTEGER, {DalTask.TitleColumnName} " +
                $"TEXT NOT NULL, {DalTask.DescriptionColumnName} TEXT, {DalTask.DueDateColumnName} INTEGER NOT NULL, {DalTask.CreationTimeColumnName} " +
                $"INTEGER NOT NULL, {DalTask.AssigneeColumnName} TEXT NOT NULL, PRIMARY KEY({DalTask.IdColumnName},{DalTask.BoardIdColumnName}), FOREIGN " +
                $"KEY({DalTask.BoardIdColumnName}, {DalTask.ColumnIdColumnName}) REFERENCES {ColumnDalController.TableName} " +
                $"({DalColumn.BoardIdColumnName}, {DalColumn.IdColumnName}))";
            SQLiteCommand command = new SQLiteCommand(null, connection);
            command.CommandText = taskTable;
            command.ExecuteNonQuery();
            command.Dispose();
        }
        ///abstract methods
        /// <summary>
        /// inserts a dalobject to datatbase
        /// </summary>
        /// <param name="dalObject"></param>
        /// <returns>true if succeeded and false if not</returns>
        public abstract bool Insert(T dalObject);
        /// <summary>
        /// Deletes a dalobject from database
        /// </summary>
        /// <param name="dalObject"></param>
        /// <returns>true if succeeded and false if not</returns>
        public abstract bool Delete(T dalObject);
        /// <summary>
        /// freates a dal object T from a row in the database
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected abstract T ReaderToObject(SQLiteDataReader reader);
        /// <summary>
        /// loads all dalobjects by its keys
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>the list from the database</returns>
        public abstract List<T> LoadData(params int[] keys);

        ///select methods
        /// <summary>
        /// select all users
        /// </summary>
        /// <returns>list of users</returns>
        public List<T> Select() {
            List<T> output = new List<T>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = ToSelectCommand()
                };
                SQLiteDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        output.Add(ReaderToObject(reader));
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    log.Error("Couldn't load data from database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                    log.Debug("Data was loaded.");
                }
            }
            return output;
        }

        /// <summary>
        /// select all columns belong to a specific board
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>list of columns (will be only one board in the list) </returns>
        public List<T> Select(int boardId) {
            List<T> output = new List<T>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = ToSelectCommand(boardId)
                };
                SQLiteDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        output.Add(ReaderToObject(reader));
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    log.Error("Couldn't load data from database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                    log.Debug("Data was loaded.");
                }
            }
            return output;
        }

        /// <summary>
        /// select all tasks belong to a specific column
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="columnId"></param>
        /// <returns>list of tasks</returns>
        public List<T> Select(int boardId, int columnId)
        {
            List<T> output = new List<T>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = ToSelectCommand(boardId, columnId)
                };
                SQLiteDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        output.Add(ReaderToObject(reader));
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    log.Error("Couldn't load data from database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                    log.Debug("Data was loaded.");
                }
            }
            return output;
        }

        ///update methods
        /// <summary>
        /// update user string property
        /// </summary>
        /// <param name="id"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns>true if succeeded and false if not</returns>
        public bool Update(int id,string attributeName,string attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = ToUpdateCommand(attributeName, id)
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter("@" + attributeName, attributeValue));
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    log.Error("couldn't update " + attributeName + ": " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                
            }
            return res > 0;
        }

        /// <summary>
        /// update column string property
        /// </summary>
        /// <param name="User_id"></param>
        /// <param name="id"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns>true if succeeded and false if not</returns>
        public bool Update(int boardId, int id,  string attributeName, string attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = ToUpdateCommand(attributeName, id, boardId)
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter("@" + attributeName, attributeValue));
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    log.Error("couldn't update " + attributeName + ": " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }

        /// <summary>
        /// update user int property
        /// </summary>
        /// <param name="id"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns>true if succeeded and false if not</returns>
        public bool Update(int id, string attributeName, int attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = ToUpdateCommand(attributeName, id)
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter("@" + attributeName, attributeValue));
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    log.Error("couldn't update " + attributeName + ": " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }

        /// <summary>
        /// update column or task int property
        /// </summary>
        /// <param name="User_id"></param>
        /// <param name="id"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns>true if succeeded and false if not</returns>
        public bool Update(int boardId, int id, string attributeName, int attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = ToUpdateCommand(attributeName, id, boardId)
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter("@" + attributeName, attributeValue));
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    log.Error("couldn't update " + attributeName + ": " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }

        /// <summary>
        /// creates a command update string by keys and attribute name
        /// </summary>
        /// <param name="attributeName">the attribute which is updated</param>
        /// <param name="keys"></param>
        /// <returns>the command string</returns>
        private string ToUpdateCommand(string attributeName, params int[] keys) {
            string output = $"UPDATE {_tableName} SET [{attributeName}]=@{attributeName} WHERE {DalObject<T>.IdColumnName}={keys[0]} ";
            switch (keys.Length)
            {
                case 1:
                    return output;
                case 2:
                    return output + $" AND {DalColumn.BoardIdColumnName} = {keys[1]}";
                default:
                    return output;
            }   
        }

        /// <summary>
        /// creates a command select string by keys
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        private string ToSelectCommand(params int[] attributes) {
            string output = $"SELECT * FROM {_tableName} ";
            switch (attributes.Length)
            {
                case 1:
                    return output + $"WHERE {DalColumn.BoardIdColumnName}={attributes[0]}";
                case 2:
                    return output + $"WHERE {DalTask.BoardIdColumnName}={attributes[0]} AND {DalTask.ColumnIdColumnName}={attributes[1]} ";
                default:
                    return output;
            }
        }

    }
}

