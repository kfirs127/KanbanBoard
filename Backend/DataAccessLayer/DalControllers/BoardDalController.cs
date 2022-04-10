using IntroSE.Kanban.Backend.DataAccessLayer.DalObjects;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DalControllers
{
    class BoardDalController : DalController <DalBoard>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        internal const string TableName = "Boards";

        /// <summary>
        /// empty constructor
        /// </summary>
        public BoardDalController() : base(TableName)
        {
        }
        /// <summary>
        /// inserts the dalboard into the database
        /// </summary>
        /// <param name="dalBoard"></param>
        /// <returns>true if succeeded and false if not</returns>
        public override bool Insert(DalBoard dalBoard)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = ToInsertCammand(DalObject<DalBoard>.IdColumnName, DalBoard.CreatorColumnName);

                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", dalBoard.Id);
                    SQLiteParameter creatorParam = new SQLiteParameter(@"creatorVal", dalBoard.Creator);

                    command.Parameters.Add(idParam);
                    command.Parameters.Add(creatorParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                    log.Debug("board was added to database");
                }
                catch(Exception e)
                {
                    log.Error("Coudn't insert the new board: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                return res>0;
            }
        }

        /// <summary>
        /// Deletes a board from the database
        /// </summary>
        /// <param name="dalBoard"></param>
        /// <returns>true if succeeded and false if not</returns>
        public override bool Delete(DalBoard dalBoard) {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = ToDeleteCommand(dalBoard.Id)
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    log.Debug("board was Deleted from database");
                }
                catch (Exception e)
                {
                    log.Error("Could't Delete board: " + e.Message);
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
        /// creats a dalboard from a row in the database
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>DalBoard</returns>
        protected override DalBoard ReaderToObject(SQLiteDataReader reader)
        {
            return new DalBoard(reader.GetInt32(0), reader.GetString(1));
        }

        /// <summary>
        /// returns a list of all boards belong to a specific user
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>list of boards</returns>
        public override List<DalBoard> LoadData(params int[] keys)
        {
            List<DalBoard> boards =  Select();
            ColumnDalController columnDalController = new ColumnDalController();
            foreach (DalBoard b in boards)
            {
                b.columns = columnDalController.LoadData(b.Id);
            }
            log.Debug("Boards loaded.");
            return boards;
        }

        /// <summary>
        /// creats a command string for inserting a new board
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        private string ToInsertCammand(params string [] attributes) {
            return $"INSERT INTO {TableName} ({attributes[0]} ,{attributes[1]}) VALUES (@idVal,@creatorVal)";
        }

        /// <summary>
        /// creats a command string to Delete a specific board by its primary keys
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private string ToDeleteCommand(params int[] keys)
        {
            return $"DELETE FROM {TableName} WHERE {DalBoard.IdColumnName}={keys[0]}";
        }

    }
}
