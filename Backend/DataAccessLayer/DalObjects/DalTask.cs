using IntroSE.Kanban.Backend.DataAccessLayer.DalControllers;
using System;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DalObjects
{
    /// <summary>
    /// DalTask class - extens DalObject and reprsent the Task class in business layer in data access layer
    /// </summary>
    public class DalTask : DalObject<DalTask>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// an empty constructor
        /// </summary>
        public DalTask() : base(new TaskDalController()) { _id = -1; }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="boardId"></param>
        /// <param name="columnId"></param>
        /// <param name="dueDate"></param>
        /// <param name="creationTime"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        public DalTask(int id, int boardId, int columnId, string title, string description, DateTime dueDate, DateTime creationTime, string assigneeEmail ) : base (new TaskDalController())
        {
            _id = id;
            _boardId = boardId;
            _columnId = columnId;
            _dueDate = dueDate;
            _creationTime = creationTime;
            _title = title;
            _description = description;
            _assigneeEmail = assigneeEmail;
        }
        public const String ColumnIdColumnName = "Column_Id";
        public const String BoardIdColumnName = "Board_Id";
        public const String DueDateColumnName = "DueDate";
        public const String CreationTimeColumnName = "Creation_Time";
        public const String TitleColumnName = "Title";
        public const String DescriptionColumnName = "Description";
        public const String AssigneeColumnName = "Assignee"; 
        
        private int _id; 
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                if (!_controller.Insert(this))
                {
                    log.Error("Couldn't insert the new task.");
                    throw new Exception("Couldn't insert the new task.");
                }
            }
        }
        private int _columnId;
        public int ColumnId
        {
            get => _columnId;
            set
            {
                if (_id != -1)
                    if (!_controller.Update(BoardId, Id, ColumnIdColumnName, value))
                    {
                        log.Error("Couldn't update task ColumnId in database.");
                        throw new Exception("Couldn't update task ColumnId in database.");
                    }
                _columnId = value;
            }
        }
        private int _boardId;
        public int BoardId
        {
            get => _boardId;
            set => _boardId = value;
        }
        private DateTime _creationTime; 
        public DateTime CreationTime
        {
            get => _creationTime;
            set => _creationTime = value;
        }
        private DateTime _dueDate; 
        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                if (_id != -1)
                    if (!_controller.Update(BoardId, Id, DueDateColumnName, value.ToString()))
                    {
                        log.Error("Couldn't update task duedate in database.");
                        throw new Exception("Couldn't update task duedate in database.");
                    }
                _dueDate = value;
            }
        }
        private String _title;
        public String Title
        {
            get => _title;
            set
            {
                if (_id != -1)
                    if (!_controller.Update(BoardId, Id, TitleColumnName, value))
                    {
                        log.Error("Couldn't update task title in database");
                        throw new Exception("Couldn't update task title in database.");
                    }
                _title = value;
            }
        }
        private String _description;
        public String Description
        {
            get => _description;
            set
            {
                if (_id != -1)
                    if(!_controller.Update(BoardId, Id, DescriptionColumnName, value))
                    {
                        log.Error("Couldn't update task description in database.");
                        throw new Exception("Couldn't update task description in database.");
                    }
                _description = value;
            }
        }
        private string _assigneeEmail;
        public string AssigneeEmail {
            get => _assigneeEmail;
            set {
                if (_id != -1)
                    if (!_controller.Update(BoardId, Id, AssigneeColumnName, value))
                    {
                        log.Error("Couldn't update task assignee in database.");
                        throw new Exception("Couldn't update task assignee in database.");
                    }
                _assigneeEmail = value;
            }
        }

        /// <summary>
        /// Deletes the task from the database
        /// </summary>
        public override void Delete() {
            if (!_controller.Delete(this))
            {
                log.Error("Couldn't Delete task from database.");
                throw new Exception("Couldn't Delete task from database.");
            }
        }
    }
}
