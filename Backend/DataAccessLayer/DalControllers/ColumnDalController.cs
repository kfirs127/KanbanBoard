using IntroSE.Kanban.Backend.DataAccessLayer.DalObjects;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DalControllers
{
    class ColumnDalController : DalController<DalColumn>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        internal const string TableName = "Columns";

        /// <summary>
        /// an empty constructor
        /// </summary>
        public ColumnDalController() : base(TableName)
        {
        }

        /// <summary>
        /// insert the dalcolumn into the database
        /// </summary>
        /// <param name="dalColumn"></param>
        /// <returns>true if succeeded and false if not</returns>
        public override bool Insert(DalColumn dalColumn)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = ToInsertCammand(DalObject<DalColumn>.IdColumnName, DalColumn.BoardIdColumnName,
                        DalColumn.ColumnOrdinalColumnName, DalColumn.NameColumnName, DalColumn.LimitColumnName);

                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", dalColumn.Id);
                    SQLiteParameter boardIdParam = new SQLiteParameter(@"boardIdVal", dalColumn.BoardId);
                    SQLiteParameter columnOrdinalParam = new SQLiteParameter(@"columnOrdinalVal", dalColumn.ColumnOrdinal);
                    SQLiteParameter nameParam = new SQLiteParameter(@"nameVal", dalColumn.Name);
                    SQLiteParameter taskLimitParam = new SQLiteParameter(@"taskLimitVal", dalColumn.Limit);

                    command.Parameters.Add(idParam);
                    command.Parameters.Add(boardIdParam);
                    command.Parameters.Add(columnOrdinalParam);
                    command.Parameters.Add(nameParam);
                    command.Parameters.Add(taskLimitParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                    log.Debug("column was added to database");
                }
                catch(Exception e)
                {
                    log.Error("Coudn't insert the new column: " + e.Message);
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
        /// Deletes a specific column from the database
        /// </summary>
        /// <param name="dalColumn"></param>
        /// <returns>true if succeeded and false if not</returns>
        public override bool Delete(DalColumn dalColumn)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = ToDeleteCommand(dalColumn.Id, dalColumn.BoardId)
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    log.Debug("column was Deleted from database");
                }
                catch (Exception e)
                {
                    log.Error("Could't Delete column: " + e.Message);
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
        /// creates a dalcolumn from a row in the database
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>the dalcolumn which was created</returns>
        protected override DalColumn ReaderToObject(SQLiteDataReader reader)
        {
            return new DalColumn(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetString(3), 
                reader.GetInt32(4));
        }

        /// <summary>
        /// loads all columns belong to a specific board
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>the list of columns from the database</returns>
        public override List<DalColumn> LoadData(params int[] keys)
        {
            List<DalColumn> columns = Select(keys[0]);
            TaskDalController taskDalController = new TaskDalController();
            foreach (DalColumn c in columns)
            {
                c.tasks = taskDalController.LoadData(c.BoardId, c.Id);
            }
            log.Debug("Columns loaded.");
            return columns;
        }

        /// <summary>
        /// creates a command string to insert a new column to the database
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns>the string command</returns>
        private string ToInsertCammand(params string[] attributes)
        {
            string output = $"INSERT INTO {TableName} ({attributes[0]}, {attributes[1]}, {attributes[2]}, {attributes[3]}, {attributes[4]}) VALUES " +
                $"(@idVal, @boardIdVal, @columnOrdinalVal, @nameVal, @taskLimitVal);";
            log.Debug(output);
            return output;

        }

        /// <summary>
        /// creates a command string to Delete a specific board by its primary keys
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>the string command</returns>
        private string ToDeleteCommand(params int[] keys)
        {
            return $"DELETE FROM {TableName} WHERE {DalColumn.IdColumnName}={keys[0]} AND {DalColumn.BoardIdColumnName}={keys[1]}";
        }
    }
}
