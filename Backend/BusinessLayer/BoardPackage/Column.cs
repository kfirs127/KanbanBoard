using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DalObjects;
using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.BusinessLayer.BoardPackage
{
    /// <summary>
    /// Column claass - the connection between the Board to the tasks
    /// </summary>
    public class Column
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<int, Task> tasks;
        private DalColumn dalColumn;
        private int _maxTaskId = -1;
        public int MaxTaskId{
            get => _maxTaskId;
            set {
                if (value > _maxTaskId)
                    _maxTaskId = value;
            }
        }
        private String _name;
        public String Name
        {
            get => _name;
            set
            {
                if (value == null)
                {
                    log.Warn("null name");
                    throw new KanbanException("The name is null");
                }
                if (value.Length == 0 || value.Length > 15)
                {
                    log.Warn("The length of the name of the column is illegal");
                    throw new KanbanException("The length of the name of the column is illegal");
                }
                dalColumn.Name = value;
                _name = value;
            }
        }
        private int _limit;
        public int Limit
        {
            get => _limit;
            set
            {
                if (value < tasks.Count)
                {
                    log.Warn("Limit is lower than the amount of tasks.");
                    throw new KanbanException("Limit is lower than the amount of tasks.");
                }
                dalColumn.Limit = value;
                _limit = value;
            }
        }
        private int _columnOrdinal;
        public virtual int  ColumnOrdinal
        {
            get => _columnOrdinal;
            set
            {
                dalColumn.ColumnOrdinal = value;
                _columnOrdinal = value;
            }
        }
        private int _boardId;
        public int BoardId{
            get => _boardId;
            set{
                dalColumn.BoardId = value;
                _boardId = value;
            }
        }
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                dalColumn.Id = value;
                _id = value;
            }
        }
        

        /// <summary>
        /// A constructor which copy the values and puts 0 in the number of tasks and -1 as limit
        /// </summary>
        /// <param name="name"></param>
        /// <param name="columnOrdinal"></param>
        public Column(String name, int columnOrdinal, int id, int boardId)
        {
            dalColumn = new DalColumn();
            tasks = new Dictionary<int, Task>();
            Name = name;
            ColumnOrdinal = columnOrdinal;
            Limit = 100;
            BoardId = boardId;
            Id = id;
        }


        /// <summary>
        /// constructor for tests
        /// </summary>
        /// <param name="name"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="id"></param>
        /// <param name="boardId"></param>
        /// <param name="userId"></param>
        /// <param name="d"></param>
        /// <param name="DC"></param>
        public Column(String name, int columnOrdinal, int id, int boardId, Dictionary<int, Task> d , DalColumn DC)
        {
            dalColumn = DC;
            tasks = d;
            Name = name;
            ColumnOrdinal = columnOrdinal;
            Limit = -1;
            BoardId = boardId;
            Id = id;
        }

        /// <summary>
        /// constructor for test
        /// </summary>
        public Column()
        {
            tasks = new Dictionary<int, Task>();
        }

        /// <summary>
        /// gets a dal board and creats a business layer board
        /// </summary>
        /// <param name="column"></param>
        public Column(DalColumn column) {
            dalColumn = column;
            _id = column.Id;
            _boardId = column.BoardId;
            _name = column.Name;
            _columnOrdinal = column.ColumnOrdinal;
            _limit = column.Limit;
            tasks = new Dictionary<int, Task>();
            foreach (DalTask toAdd in column.tasks)
            {
                tasks.Add(toAdd.Id, new Task(toAdd));
                MaxTaskId = toAdd.Id;
            }
            log.Debug("Tasks were loaded to column.");
        }

        /// <summary>
        /// creats a new task and adds it to the column
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="dueDate"></param>
        public virtual void addTask(int taskId, string title, string description, DateTime dueDate, string assigneeEmail)
        {
            if (tasks.Count == _limit)
            {
                log.Warn("The number of tasks exceeds the limit.");
                throw new KanbanException("The number of tasks exceeds the limit.");
            }
            Task task = new Task(taskId, title, description, dueDate, assigneeEmail, Id, BoardId);
            tasks.Add(taskId, task);
        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        
		/// <param name="emailAssignee">Email of the user to assign to task to</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        internal void AssignTask(string email, int taskId, string emailAssignee)
        {
            if (tasks.ContainsKey(taskId))
            {
                if (!GetTask(taskId).AssigneeEmail.Equals(email))
                {
                    log.Warn("Attempt to update task assignee not by it's assignee");
                    throw new KanbanException("Only task's assignee can update task's assignee.");
                }
                tasks[taskId].AssigneeEmail = emailAssignee;
            }
            else
            {
                log.Warn("attempt to edit a task which does not exist in column.");
                throw new KanbanException("The task does not exist in the column.");
            }
        }

        /// <summary>
        /// adds the task to the column
        /// </summary>
        /// <param name="task"></param>
        public virtual void addTask(Task task)
        {
            if (tasks.Count == _limit)
            {
                log.Warn("The number of tasks exceeds the limit.");
                throw new KanbanException("The number of tasks exceeds the limit.");
            }
            task.ColumnId = Id;
            tasks.Add(task.Id, task);
        }

        /// <summary>
        /// removes a task from the column (used to advance task)
        /// </summary>
        /// <param name="taskId"></param>
        public void removeTask(int taskId, string email)
        {
            if (tasks.ContainsKey(taskId))
            {
                if (!GetTask(taskId).AssigneeEmail.Equals(email))
                {
                    log.Warn("Attempt to advance task not by it's assignee");
                    throw new KanbanException("Only task's assignee can advance task.");
                }
                tasks.Remove(taskId);
            }
            else
            {
                log.Warn("Task is not in system.");
                throw new KanbanException("Entered task ID is not in system.");
            }
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        		
        internal void DeleteTask(int taskId, string email)
        {
            if (tasks.ContainsKey(taskId))
            {
                Task toDelete = tasks[taskId];
                if (!toDelete.AssigneeEmail.Equals(email))
                {
                    log.Warn("attept to Delete a task not by it's assignee.");
                    throw new KanbanException("Only a task's assignee can Delete the task.");
                }
                tasks.Remove(taskId);
                toDelete.Delete();
            }
            else
            {
                log.Warn("Task is not in system.");
                throw new KanbanException("Entered task ID is not in this column.");
            }
        }

        /// <summary>
        /// updates a task due-date
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="dueDate"></param>
        public void UpdateTaskDueDate(int taskId, DateTime dueDate, string email)
        {
            if (!GetTask(taskId).AssigneeEmail.Equals(email))
            {
                log.Warn("Attempt to update task not by it's assignee");
                throw new KanbanException("Only task's assignee can update task.");
            }
            GetTask(taskId).DueDate = dueDate;
        }

        /// <summary>
        /// update a task's description
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="newDescription"></param>
        public void EditDescription(int taskId, String newDescription, string email)
        {
            if (!GetTask(taskId).AssigneeEmail.Equals(email))
            {
                log.Warn("Attempt to update task not by it's assignee");
                throw new KanbanException("Only task's assignee can update task.");
            }
            GetTask(taskId).Description = newDescription;
        }

        /// <summary>
        /// update a tasks's title
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="newTitle"></param>
        public void EditTitle(int taskId, String newTitle, string email)
        {
            if (!GetTask(taskId).AssigneeEmail.Equals(email))
            {
                log.Warn("Attempt to update task not by it's assignee");
                throw new KanbanException("Only task's assignee can update task.");
            }
            GetTask(taskId).Title = newTitle;
        }


        public Task GetTask(int taskId){
            if(tasks.ContainsKey(taskId))
                return tasks[taskId];
            log.Warn("task does not exist");
            throw new KanbanException("The task doent exist in the column.");
        }

        public List<Task> GetTasks()
        {
            List < Task > output = new List<Task>();
            foreach (KeyValuePair<int, Task> task in tasks)
                output.Add(task.Value);
            return output;
        }

        /// <summary>
        /// delete all tasks in column and the column itself
        /// </summary>
        public void Delete() {
            foreach (KeyValuePair<int, Task> toDelete in tasks)
                toDelete.Value.Delete();
            dalColumn.Delete();
        }
    }
}
