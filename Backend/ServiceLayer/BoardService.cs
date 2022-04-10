using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.BusinessLayer.BoardPackage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Implements the Board, Column and Task methods
    /// </summary>
    public class BoardService
    {
        private Security security;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// A constructor which gets Security from the service
        /// The value in the Security field in this class will be the same as in UserService class and will be recieved from the Service class         /// </summary>
        /// <param name="s"></param>
        /// 
        public BoardService(Security s)
        {
            security = s;
        }


        /// <summary>
        /// Returns the board of a user. The user must be logged in
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>A response object with a value set to the board, or an error message in case of an error</returns>
        public Response<Board> GetBoard(string email)
        {
            try
            {
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                var stringBoard = board.GetColumns().OrderBy(Key => Key.Key);
                var sorted = stringBoard.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
                List<string> output = new List<string>();
                foreach (var entry in sorted)
                {
                    output.Add(entry.Value.Name);
                }
                log.Debug("Board was loaded.");
                return new Response<Board>(new Board(output.AsReadOnly(), board.Creator));
            }
            catch (KanbanException e)
            {
                return new Response<Board>(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response<Board>("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>A response object, or an error message in case of an error</returns>
        public Response LimitColumnTasks(string email, int columnOrdinal, int limit)
        {
            try
            {
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                board.LimitColumnTasks(columnOrdinal, limit, email);
                return new Response();
            }
            catch (KanbanException e)
            {
                return new Response(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        
		/// <param name="emailAssignee">Email of the user to assign to task to</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AssignTask(string email, int columnOrdinal, int taskId, string emailAssignee)
        {
            try
            {
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                board.AssignTask(email, columnOrdinal, taskId, emailAssignee);
                return new Response();
            }
            catch (KanbanException e)
            {
                return new Response(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response("Error: " + e.Message);
            }
        }
    

        /// <summary>
        /// Add a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>A response object with a value set to the Task, or an error message in case of an error</returns>
        public Response<Task> AddTask(string email, string title, string description, DateTime dueDate)
        {
            try
            {
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                int id = board.AddTask(email, title, description, dueDate);
                Task output = new Task(id, DateTime.Now, dueDate, title, description, email);
                return new Response<Task>(output);
            }
            catch (KanbanException e)
            {
                return new Response<Task>(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response<Task>("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Deletes a task
        /// </summary>
        /// <param name="email"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        internal Response DeleteTask(string email, int columnOrdinal, int taskId)
        {
            try
            {
                security.GetBoard(email).DeleteTask(columnOrdinal, taskId, email);
                log.Debug("A task was Deleted.");
                return new Response();
            }
            catch (KanbanException e)
            {
                log.Warn(e.Message);
                return new Response(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <returns>A response object, or an error message in case of an error</returns>
        public Response UpdateTaskDueDate(string email, int columnOrdinal, int taskId, DateTime dueDate)
        {
            try
            {
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                board.UpdateTaskDueDate(columnOrdinal, taskId, dueDate, email);
                return new Response();
            }
            catch (KanbanException e)
            {
                return new Response(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Change the name of a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="newName">The new name.</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        internal Response ChangeColumnName(string email, int columnOrdinal, string newName)
        {
            try
            {
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                board.ChangeColumnName(email, columnOrdinal, newName);
                return new Response();
            }
            catch (KanbanException e)
            {
                return new Response(e.Message);
            }
            catch (Exception e)
            {
                return new Response("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Update task title
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <returns>A response object, or an error message in case of an error</returns>
        public Response UpdateTaskTitle(string email, int columnOrdinal, int taskId, string title)
        {
            try
            {
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                board.EditTitle(columnOrdinal, taskId, title, email);
                return new Response();
            }
            catch (KanbanException e)
            {
                return new Response(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response("Error: " + e.Message);
            }
        }

        

        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <returns>A response object, or an error message in case of an error</returns>
        public Response UpdateTaskDescription(string email, int columnOrdinal, int taskId, string description)
        {
            try
            {
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                board.EditDescription(columnOrdinal, taskId, description, email);
                return new Response();
            }
            catch (KanbanException e)
            {
                return new Response(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns>A response object, or an error message in case of an error</returns>
        public Response AdvanceTask(string email, int columnOrdinal, int taskId)
        {
            try
            {
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                board.AdvanceTask(columnOrdinal, taskId, email);
                return new Response();
            }
            catch (KanbanException e)
            {
                return new Response(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Returns a column given it's name
        /// </summary>
        public Response<Column> GetColumn(string email, string columnName)
        {
            try
            {
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                BusinessLayer.BoardPackage.Column c = board.GetColumn(columnName);
                List<Task> tasks = new List<Task>();
                foreach (BusinessLayer.BoardPackage.Task toAdd in c.GetTasks())
                    tasks.Add(new Task(toAdd.Id, toAdd.GetCreationTime(), toAdd.DueDate, toAdd.Title, toAdd.Description, toAdd.AssigneeEmail));
                Column output = new Column(tasks.AsReadOnly(), c.Name, c.Limit);
                log.Debug("Column was loaded.");
                return new Response<Column>(output);
            }
            catch (KanbanException e)
            {
                return new Response<Column>(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response<Column>("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Returns a column given it's identifier.
        /// The first column is identified by 0, the ID increases by 1 for each column
        /// </summary>
        public Response<Column> GetColumn(string email, int columnOrdinal)
        {
            try
            {
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                BusinessLayer.BoardPackage.Column c = board.GetColumn(columnOrdinal);
                List<Task> tasks = new List<Task>();
                foreach (BusinessLayer.BoardPackage.Task toAdd in c.GetTasks())
                    tasks.Add(new Task(toAdd.Id, toAdd.GetCreationTime(), toAdd.DueDate, toAdd.Title, toAdd.Description, toAdd.AssigneeEmail));
                Column output = new Column(tasks.AsReadOnly(), c.Name, c.Limit);
                log.Debug("Column was loaded");
                return new Response<Column>(output);
            }
            catch (KanbanException e)
            {
                return new Response<Column>(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response<Column>("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Removes a column given it's identifier.
        /// The first column is identified by 0, the ID increases by 1 for each column
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Column ID</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RemoveColumn(string email, int columnOrdinal)
        {
            try{
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                board.RemoveColumn(columnOrdinal, email);
                log.Debug("Column was Deleted.");
                return new Response();
            }
            catch (KanbanException e)
            {
                return new Response(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Adds a new column, given it's name and a location to place it.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Location to place to column</param>
        /// <param name="Name">new Column name</param>
        /// <returns>A response object with a value set to the new Column, the response should contain a error message in case of an error</returns>
        public Response<Column> AddColumn(string email, int columnOrdinal, string name)
        {
            try{
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                BusinessLayer.BoardPackage.Column c = board.AddColumn(columnOrdinal, name, email);
                List<Task> tasks = new List<Task>();
                foreach (BusinessLayer.BoardPackage.Task toAdd in c.GetTasks())
                    tasks.Add(new Task(toAdd.Id, toAdd.GetCreationTime(), toAdd.DueDate, toAdd.Title, toAdd.Description, toAdd.AssigneeEmail));
                Column output = new Column(tasks.AsReadOnly(), c.Name, c.Limit);
                log.Debug("Column was added.");
                return new Response<Column>(output);
            }
            catch (KanbanException e)
            {
                return new Response<Column>(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response<Column>("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Moves a column to the right, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Current location of the column</param>
        /// <returns>A response object with a value set to the moved Column, the response should contain a error message in case of an error</returns>
        public Response<Column> MoveColumnRight(string email, int columnOrdinal)
        {
            try{
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                BusinessLayer.BoardPackage.Column c = board.MoveColumnRight(columnOrdinal, email);
                List<Task> tasks = new List<Task>();
                foreach (BusinessLayer.BoardPackage.Task toAdd in c.GetTasks())
                    tasks.Add(new Task(toAdd.Id, toAdd.GetCreationTime(), toAdd.DueDate, toAdd.Title, toAdd.Description, toAdd.AssigneeEmail));
                Column output = new Column(tasks.AsReadOnly(), c.Name, c.Limit);
                log.Debug("Column was moved to right.");
                return new Response<Column>(output);
            }
            catch (KanbanException e)
            {
                return new Response<Column>(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response<Column>("Error: " + e.Message);
            }

        }

        /// <summary>
        /// Moves a column to the left, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Current location of the column</param>
        /// <returns>A response object with a value set to the moved Column, the response should contain a error message in case of an error</returns>
        public Response<Column> MoveColumnLeft(string email, int columnOrdinal)
        {
            try{
                BusinessLayer.BoardPackage.Board board = security.GetBoard(email);
                BusinessLayer.BoardPackage.Column c = board.MoveColumnLeft(columnOrdinal, email);
                List<Task> tasks = new List<Task>();
                foreach (BusinessLayer.BoardPackage.Task toAdd in c.GetTasks())
                    tasks.Add(new Task(toAdd.Id, toAdd.GetCreationTime(), toAdd.DueDate, toAdd.Title, toAdd.Description, toAdd.AssigneeEmail));
                Column output = new Column(tasks.AsReadOnly(), c.Name, c.Limit);
                log.Debug("Column was m oved to left");
                return new Response<Column>(output);
            }
            catch (KanbanException e)
            {
                return new Response<Column>(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response<Column>("Error: " + e.Message);
            }

        }
    }

}
