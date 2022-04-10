using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DalObjects;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Board claass - the connection between the BoardService to the BoardPackage.
/// </summary>
namespace IntroSE.Kanban.Backend.BusinessLayer.BoardPackage
{
    public class Board
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private DalBoard dalBoard;
        private Dictionary<int, Column> columns;
        private int _id;
        public int Id {
            get => _id;
            set {
                dalBoard.Id = value;
                _id = value;
            }
        }
        private int _columnCounter = 0;
        public int ColumnCounter {
            get => _columnCounter;
            set {
                if (value > _columnCounter)
                    _columnCounter = value;
            }
        }
        private int _taskCounter = 0;
        public int TaskCounter {
            get => _taskCounter;
            set {
                if (value > _taskCounter)
                    _taskCounter = value;
            }
        }
        private string _creator;
        public string Creator {
            get => _creator;
            set
            {
                dalBoard.Creator = value;
                _creator = value;
            }
        }

        /// <summary>
        /// A constructor which gets an email and resets the columnCounter to 0 and adds 3 new columns to the board - backlog, inProgress and done
        /// </summary>
        /// <param name="email"> The email of the user who will be the owner of the board</param>
        public Board(int id, string creator)
        {
            dalBoard = new DalBoard();
            Creator = creator;
            Id = id;
            columns = new Dictionary<int, Column>();
            columns.Add(0, new Column("backlog", 0, columns.Count, Id));
            ColumnCounter = ColumnCounter + 1;
            columns.Add(1, new Column("in progress", 1, columns.Count,Id));
            ColumnCounter = ColumnCounter + 1;
            columns.Add(2, new Column("done", 2, columns.Count, Id));
            ColumnCounter = ColumnCounter + 1;;
            log.Debug("a board was created.");
        }

        /// <summary>
        /// constructor for tests
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="d"></param>
        /// <param name="DB"></param>
        public Board(String email, int id , Dictionary<int , Column> d , DalBoard DB)
        {
            dalBoard = DB;
            columns = d;
            Creator = email;
            Id = id;
            log.Debug("a board was created.");
        }

        /// <summary>
        /// gets a DalBoard and creats a business layer board.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="email"></param>
        public Board(DalBoard board) {
            bool hasTask = false;
            dalBoard = board;
            _id = board.Id;
            _creator = board.Creator;
            this.columns = new Dictionary<int, Column>();
            foreach (DalColumn toAdd in board.columns)
            { 
                Column c = new Column(toAdd);
                columns.Add(toAdd.ColumnOrdinal,c);
                ColumnCounter = toAdd.Id;
                TaskCounter = c.MaxTaskId;
                if (c.GetTasks().Count > 0)
                    hasTask = true;
            }
            if(columns.Count > 0)
                ColumnCounter = ColumnCounter + 1;
            if (hasTask)
                TaskCounter = TaskCounter + 1;
            log.Debug("Columns were loaded to board.");
        }


        /// <summary>
        /// returns columns by it's identifier
        /// </summary>
        /// <param name="columnOrdinal"></param>
        /// <returns>Column</returns>
        public Column GetColumn(int columnOrdinal)
        {
            if (columnOrdinal >= 0 & columnOrdinal < columns.Count)
            {
                return columns[columnOrdinal];
            }
            else
            {
                log.Warn("Column doesnt exist in board.");
                throw new KanbanException("Column doesnt exist in board.");
            }
        }

        /// <summary>
        /// Returns a column given it's name
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns>Column</returns>
        public Column GetColumn(String columnName)
        {
            foreach (KeyValuePair <int, Column> column in columns)
                if (column.Value.Name.Equals(columnName))
                {
                    return column.Value;
                }
            log.Debug("Column doesnt exist in board.");
            throw new KanbanException("Column doesnt exist in board.");
        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        
		/// <param name="emailAssignee">Email of the user to assign to task to</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        internal void AssignTask(string email, int columnOrdinal, int taskId, string emailAssignee)
        {
            if (!columns.ContainsKey(columnOrdinal))
            {
                log.Warn("attempt to update a task in a column which does not exist.");
                throw new KanbanException("The column does not exist in the board.");
            }
            columns[columnOrdinal].AssignTask(email, taskId, emailAssignee);
        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="columnOrdinal"></param>
        /// <param name="limit"></param>
        public void LimitColumnTasks(int columnOrdinal, int limit, string email)
        {
            if (!email.Equals(Creator)) {
                log.Warn("Attempt to change limit while not creator.");
                throw new KanbanException("Only board's creator can change column's llimit.");
            }
            columns[columnOrdinal].Limit = limit;
            log.Debug("Column task limit updated.");
        }

        /// <summary>
        /// Add a new task
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="dueDate"></param>
        /// <returns>returns the id of the task</returns>
        public int AddTask(string email, String title, String description, DateTime dueDate)
        {
            columns[0].addTask(TaskCounter, title, description, dueDate, email);
            TaskCounter = TaskCounter + 1;
            log.Debug("A task was added.");
            return TaskCounter - 1;
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        		
        internal void DeleteTask(int columnOrdinal, int taskId, string email)
        {
            GetColumn(columnOrdinal).DeleteTask(taskId, email);
        }

        /// <summary>
        /// Returns task by it's id
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns>task</returns>
        public Task GetTask(int taskId)
        {
            int containingColumn = TaskInColumn(taskId);
            if (containingColumn == -1)
            {
                log.Warn("Task doesn't exist in Board.");
                throw new KanbanException("Task doesn't exist in Board.");
            }
            return columns[containingColumn].GetTask(taskId);
        }

        /// <summary>
        /// check existence of a task by it's id
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns>the number of the column which contains the task or -1 if the task does not exist in the board</returns>
        private int TaskInColumn(int taskId)
        { 
            for (int i = 0; i < columns.Count; i++)
            {
                try
                {
                    Task t = (columns[i].GetTask(taskId));
                    return i;
                }
                catch { }
            }
            return -1;
        }

        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="columnOrdinal"></param>
        /// <param name="taskId"></param>
        public void AdvanceTask(int columnOrdinal, int taskId, string email)
        {
            if (columnOrdinal == columns.Count - 1)
            {
                log.Warn("asked to advance from the last column.");
                throw new KanbanException("Cannot advance from the last column.");
            }
            Task temp = GetColumn(columnOrdinal).GetTask(taskId);
            GetColumn(columnOrdinal).removeTask(taskId, email);
            GetColumn(columnOrdinal + 1).addTask(temp);
            log.Debug("A task was advenced.");
        }

        /// <summary>
        /// updating column's name by it's ordinal
        /// </summary>
        /// <param name="email"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="newName"></param>
        internal void ChangeColumnName(string email, int columnOrdinal, string newName)
        {
            if (email != Creator)
            {
                log.Warn("Not creator attempt to change column's name.");
                throw new KanbanException("Only Board's creator can update column's name.");
            }
            Column toChange = GetColumn(columnOrdinal);
            foreach (KeyValuePair<int, Column> toCheck in columns)
            {
                if (toCheck.Value.Name.Equals(newName))
                {
                    log.Warn("Attempt to change column's name to an existing one.");
                    throw new KanbanException("This name is already exists in board.");
                }
            }
            toChange.Name = newName;
        }

        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="columnOrdinal"></param>
        /// <param name="taskId"></param>
        /// <param name="dueDate"></param>
        public void UpdateTaskDueDate(int columnOrdinal, int taskId, DateTime dueDate, string email)
        {
            columns[columnOrdinal].UpdateTaskDueDate(taskId, dueDate, email);
            log.Debug("A Task due-date was updated.");
        }

        /// <summary>
        /// Update task title
        /// </summary>
        /// <param name="columnOrdinal"></param>
        /// <param name="taskId"></param>
        /// <param name="newTitle"></param>
        public void EditTitle(int columnOrdinal, int taskId, String newTitle, string email)
        {
            if (TaskInColumn(taskId) == -1)
            {
                log.Warn("Attempt to update task which not exist.");
                throw new KanbanException("The task does not exist in the board.");
            }
            columns[columnOrdinal].EditTitle(taskId, newTitle, email);
            log.Debug("A task title was updated.");
        }

        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="columnOrdinal"></param>
        /// <param name="taskId"></param>
        /// <param name="newDescription"></param>
        public void EditDescription(int columnOrdinal, int taskId, String newDescription, string email)
        {
            if (TaskInColumn(taskId) == -1)
            {
                log.Warn("Attempt to update task which not exist.");
                throw new KanbanException("The task does not exist in the board.");
            }
            if (columnOrdinal == columns.Count - 1)
            {
                log.Warn("Attempt to update task in the last column.");
                throw new KanbanException("Cannot edit a task in the last column.");
            }
            columns[columnOrdinal].EditDescription(taskId, newDescription, email);
            log.Debug("A task description was updated.");
        }

        /// <summary>
        /// Adds a new column, given it's name and a location to place it.
        /// The first column is identified by 0, the ID increases by 1 for each column 
        /// </summary>
        /// <param name="columnOrdinal">Location to place to column</param>
        /// <param name="name">new Column name</param>
        public Column AddColumn(int columnOrdinal, String name, string email)
        {
            if (!email.Equals(Creator))
            {
                log.Warn("attept to add a column to the board not by it's creator.");
                throw new KanbanException("Only board's creator can add a column to the board.");
            }
            foreach (KeyValuePair <int,Column> column in columns)
            {
                if (column.Value.Name.Equals(name))
                {
                    log.Warn("new column with an existing name");
                    throw new KanbanException("Column name is already exists.");
                }
            }
            if (columnOrdinal > columns.Count)
            {
                log.Error("new column with column ordinal higher the the amount of columns");
                throw new KanbanException("The columnOridinal is higher then the amount of columns");
            }
            Column newColumn = new Column(name, columnOrdinal, ColumnCounter, Id);
            ColumnCounter = ColumnCounter + 1;
            for(int i = columns.Count; i > columnOrdinal; i = i - 1){
                Column temp = columns[i-1];
                columns.Remove(i-1);
                temp.ColumnOrdinal = i;
                columns.Add(i, temp);
            }
            columns.Add(columnOrdinal, newColumn);
            return newColumn;
        }

        /// <summary>
        /// Removes a column given it's identifier.
        /// The first column is identified by 0, the ID increases by 1 for each column 
        /// </summary>
        /// <param name="columnOridinal"></param>
        public void RemoveColumn(int columnOridinal, string email)
        {
            if (!email.Equals(Creator))
            {
                log.Warn("attept to remove a column from the board not by it's creator.");
                throw new KanbanException("Only board's creator can remove a column from the board.");
            }
            if (columns.Count() == 2)
            {
                log.Warn("attempt to Delete with 2 columns");
                throw new Exception("Cannot remove column with only 2 columns exist.");
            }
            if (columnOridinal == 0)
            {
                if (columns[1].GetTasks().Count + columns[0].GetTasks().Count > columns[1].Limit & columns[1].Limit != -1)
                {
                    log.Warn("attempt to remove a column while the other cannot recieve the tasks");
                    throw new KanbanException("Cannot remove the column when the next column unable to recieve the tasks.");
                }
                foreach (Task task in columns[0].GetTasks())
                {
                    columns[1].addTask(task);
                    columns[0].removeTask(task.Id, task.AssigneeEmail);
                }
            }
            else
            {
                if (columns[columnOridinal - 1].GetTasks().Count + columns[columnOridinal].GetTasks().Count > columns[columnOridinal - 1].Limit & columns[columnOridinal - 1].Limit != -1)
                {
                    log.Warn("attempt to remove a column while the other cannot recieve the tasks");
                    throw new KanbanException("Cannot remove the column when the next column unable to recieve the tasks.");
                }
                foreach (Task task in columns[columnOridinal].GetTasks())
                {
                    columns[columnOridinal - 1].addTask(task);
                    columns[columnOridinal].removeTask(task.Id, task.AssigneeEmail);
                }
            }
            Column toDelete = columns[columnOridinal];
            toDelete.Delete();
            columns.Remove(columnOridinal);
            for (int i = columnOridinal; i < columns.Count; i++)
            {
                Column change = columns[i + 1];
                columns.Remove(i + 1);
                columns.Add(i, change);
                columns[i].ColumnOrdinal = i;
            }
        }

        /// <summary>
        /// /// Moves a column to the right, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column
        /// </summary>
        /// <param name="columnOridinal">Current location of the column</param>
        public Column MoveColumnRight(int columnOridinal, string email)
        {
            if (!email.Equals(Creator))
            {
                log.Warn("attept to update a column in the board not by it's creator.");
                throw new KanbanException("Only board's creator can update a column in the board.");
            }
            if (columnOridinal == columns.Count - 1)
            {
                log.Warn("attempt to move the last column right.");
                throw new KanbanException("Cannot move the last column to the right.");
            }
            Column toLeft = columns[columnOridinal + 1];
            Column toRight = columns[columnOridinal];
            columns.Remove(columnOridinal);
            columns.Remove(columnOridinal + 1);
            columns.Add(columnOridinal, toLeft);
            columns.Add(columnOridinal + 1, toRight);
            columns[columnOridinal].ColumnOrdinal = columnOridinal;
            columns[columnOridinal + 1].ColumnOrdinal = columnOridinal + 1;
            return toRight;
        }

        /// <summary>
        /// /// Moves a column to the left, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column.
        /// </summary>
        /// <param name="columnOridinal">Current location of the column</param>
        public Column MoveColumnLeft(int columnOridinal, string email)
        {
            if (!email.Equals(Creator))
            {
                log.Warn("attept to update a column in the board not by it's creator.");
                throw new KanbanException("Only board's creator can update a column in the board.");
            }
            if (columnOridinal == 0)
            {
                log.Warn("attempt to move the first column left.");
                throw new KanbanException("Cannot move the first column to the left.");
            }
            Column toRight = columns[columnOridinal - 1];
            Column toLeft = columns[columnOridinal];
            columns.Remove(columnOridinal);
            columns.Remove(columnOridinal - 1);
            columns.Add(columnOridinal, toRight);
            columns.Add(columnOridinal - 1, toLeft);
            columns[columnOridinal].ColumnOrdinal = columnOridinal;
            columns[columnOridinal - 1].ColumnOrdinal = columnOridinal - 1;
            return toLeft;
        }

        /// <summary>
        /// Deletes the board
        /// </summary>
        public void Delete() {
            foreach (KeyValuePair<int, Column> toDelete in columns)
                toDelete.Value.Delete();
            dalBoard.Delete();
        }
     

        public Dictionary<int,Column> GetColumns()
        {
            return columns;
        }
    }
}
