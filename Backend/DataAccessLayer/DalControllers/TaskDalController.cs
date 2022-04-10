using IntroSE.Kanban.Backend.DataAccessLayer.DalObjects;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DalControllers
{
    class TaskDalController : DalController<DalTask>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        internal const string TableName = "Tasks";

        /// <summary>
        /// an empty constructor
        /// </summary>
        public TaskDalController() : base(TableName)
        {
        }

        /// <summary>
        /// inserts the daltask to the database
        /// </summary>
        /// <param name="dalTask"></param>
        /// <returns></returns>
        public override bool Insert(DalTask dalTask)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = ToInsertCammand(DalObject<DalTask>.IdColumnName, DalTask.BoardIdColumnName,
                        DalTask.ColumnIdColumnName, DalTask.TitleColumnName, DalTask.DescriptionColumnName, DalTask.DueDateColumnName,
                        DalTask.CreationTimeColumnName, DalTask.AssigneeColumnName);

                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", dalTask.Id);
                    SQLiteParameter boardIdParam = new SQLiteParameter(@"boardIdVal", dalTask.BoardId);
                    SQLiteParameter colmnIdParam = new SQLiteParameter(@"columnIdVal", dalTask.ColumnId);
                    SQLiteParameter titleParam = new SQLiteParameter(@"titleVal", dalTask.Title);
                    SQLiteParameter descriptionParam = new SQLiteParameter(@"descriptionVal", dalTask.Description);
                    SQLiteParameter dueDateParam = new SQLiteParameter(@"dueDateVal", dalTask.DueDate);
                    SQLiteParameter creationTimeParam = new SQLiteParameter(@"creationTimeVal", dalTask.CreationTime);
                    SQLiteParameter assigneeEmailParam = new SQLiteParameter(@"assigneeEmailVal", dalTask.AssigneeEmail);

                    command.Parameters.Add(idParam);
                    command.Parameters.Add(boardIdParam);
                    command.Parameters.Add(colmnIdParam);
                    command.Parameters.Add(titleParam);
                    command.Parameters.Add(descriptionParam);
                    command.Parameters.Add(dueDateParam);
                    command.Parameters.Add(creationTimeParam);
                    command.Parameters.Add(assigneeEmailParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                    log.Debug("task was added to database");
                }
                catch(Exception e)
                {
                    log.Error("Coudn't insert the new task: " + e.Message);
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
        /// Deletes a specific daltask from the database
        /// </summary>
        /// <param name="dalTask"></param>
        /// <returns>true if succeeded and false if not</returns>
        public override bool Delete(DalTask dalTask)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = ToDeleteCommand(dalTask.Id, dalTask.BoardId)
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    log.Debug("task was Deleted from database");
                }
                catch (Exception e)
                {
                    log.Error("Could't Delete task: " + e.Message);
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
        /// creates a daltask from a row in database
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>the daltask which was created</returns>
        protected override DalTask ReaderToObject(SQLiteDataReader reader)
        {
            string description = null;
            if (!reader.IsDBNull(4))
                description = reader.GetString(4);
            return new DalTask(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetString(3), description,
            DateTime.Parse(reader.GetString(5)), DateTime.Parse(reader.GetString(6)), reader.GetString(7));
        }

        /// <summary>
        /// loads the daltasks from the database belong to a specific column
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>the list of daltasks from the database</returns>
        public override List<DalTask> LoadData(params int[] keys)
        {
            return Select(keys[0], keys[1]);
        }

        /// <summary>
        /// creates a command string to insert a new task
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns>the command string</returns>
        private string ToInsertCammand(params string[] attributes)
        {
            string output = $"INSERT INTO {TableName} ({attributes[0]}, {attributes[1]}, {attributes[2]}, {attributes[3]}, {attributes[4]}," +
                $" {attributes[5]}, {attributes[6]}, {attributes[7]}) "
                + $"VALUES (@idVal, @boardIdVal, @columnIdVal, @titleVal, @descriptionVal, @dueDateVal, @creationTimeVal, @assigneeEmailVal)";
            log.Debug(output);
            return output;

        }

        /// <summary>
        /// creates a command string to Delete a specific task
        /// </summary>
        /// <param name="keys"></param>
        /// <returns> the command string</returns>
        private string ToDeleteCommand(params int[] keys)
        {
            return $"DELETE FROM {TableName} WHERE {DalTask.IdColumnName}={keys[0]} AND {DalTask.BoardIdColumnName}={keys[1]}";
        }
    }
}
