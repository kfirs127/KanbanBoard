using IntroSE.Kanban.Backend.DataAccessLayer.DalObjects;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DalControllers
{
    class UserDalController : DalController<DalUser>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        internal const string TableName = "Users";

        /// <summary>
        /// an empty constructor
        /// </summary>
        public UserDalController() : base(TableName)
        {
        }

        /// <summary>
        /// inserts the daluser to the database
        /// </summary>
        /// <param name="dalUser"></param>
        /// <returns>true if succeeded and false if not</returns>
        public override bool Insert(DalUser dalUser)
        {
            
            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = ToInsertCammand(DalObject<DalUser>.IdColumnName, DalUser.EmailColumnName,
                        DalUser.PasswordColumnName, DalUser.NicknameColumnName, DalUser.BoardIdColumnName);

                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", dalUser.Id);
                    SQLiteParameter emailParam = new SQLiteParameter(@"emailVal", dalUser.Email);
                    SQLiteParameter passwordParam = new SQLiteParameter(@"passwordVal", dalUser.Password);
                    SQLiteParameter nicknameParam = new SQLiteParameter(@"nicknameVal", dalUser.Nickname);
                    SQLiteParameter boardIdParam = new SQLiteParameter("@boardIdVal", dalUser.BoardId);

                    command.Parameters.Add(idParam);
                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(passwordParam);
                    command.Parameters.Add(nicknameParam);
                    command.Parameters.Add(boardIdParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                    log.Debug("user was added to database");
                }
                catch (Exception e)
                {
                    log.Error("Coudn't insert the new user: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                return res > 0;
            }
        }

        /// <summary>
        /// Deletes a specific daluser from the database
        /// </summary>
        /// <param name="dalUser"></param>
        /// <returns></returns>
        public override bool Delete(DalUser dalUser)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = ToDeleteCommand(dalUser.Id)
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    log.Debug("user was Deleted from database");
                }
                catch (Exception e)
                {
                    log.Error("Could't Delete user: " + e.Message);
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
        /// loads all users from the database
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>the list of dalusers from the database</returns>
        public override List<DalUser> LoadData(params int[] keys)
        {
            List<DalUser> users = Select();
            log.Debug("Users loaded.");
            return users;
        }

        /// <summary>
        /// creates a daluser from a row in database
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>the daluser</returns>
        protected override DalUser ReaderToObject(SQLiteDataReader reader)
        {
            return new DalUser(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4));
        }

        /// <summary>
        /// creates a command string to insert a new user to database
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns>the command string</returns>
        private string ToInsertCammand(params string[] attributes)
        {
            return $"INSERT INTO {TableName} ({attributes[0]}, {attributes[1]}, {attributes[2]}, {attributes[3]}, {attributes[4]}) " +
                "VALUES (@idVal,@emailVal, @passwordVal, @nicknameVal, @boardIdVal)";
        }

        /// <summary>
        /// createss a command string to Delete a specific user
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>the command string</returns>
        private string ToDeleteCommand(params int[] keys) {
            return $"DELETE FROM {TableName} WHERE {DalUser.IdColumnName}={keys[0]}";
        }

    }
}
