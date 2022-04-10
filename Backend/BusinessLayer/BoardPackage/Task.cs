using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DalObjects;
using System;

namespace IntroSE.Kanban.Backend.BusinessLayer.BoardPackage
{
    /// <summary>
    /// Task claass - the connection between the Board to the tasks
    /// </summary>
    public class Task
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private DalTask dalTask;
        private int _id;
        public int Id {
            get => _id;
            set {
                dalTask.Id = value;
                _id = value;
            }
        }
        private DateTime _creationTime;
        public  DateTime CreationTime {
            get => _creationTime;
            set {
                _creationTime = DateTime.Now;
                dalTask.CreationTime = _creationTime;
            }
        }
        private DateTime _dueDate;
        public  DateTime DueDate {
            get => _dueDate;
            set{
                if (value.CompareTo(DateTime.Now) < 0)
                {
                    log.Warn("The due date is illegal.");
                    throw new KanbanException("The due date is illegal.");
                }
                dalTask.DueDate = value;
                _dueDate = value;
            }
        }
        private String _title;
        public String Title {
            get => _title;
            set{
                if (value.Length > 50 | value.Length == 0)
                {
                    log.Warn("Title is illegal.");
                    throw new KanbanException("The title is illegal.");
                }
                dalTask.Title = value;
                _title = value;
            }
        }
        private String _description;
        public String Description {
            get{return _description;}
            set{
                if (value != null && value.Length > 300)
                {
                    log.Warn("The description is too long.");
                    throw new KanbanException("The description is too long.");
                }
                dalTask.Description = value;
                _description = value;
            }
        }
        private int _columnId;
        public int ColumnId{
            get => _columnId;
            set{
                dalTask.ColumnId = value;
                _columnId = value;
            }
        }
        private string _asdigneeEmail;
        public string AssigneeEmail {
            get => _asdigneeEmail;
            set {
                dalTask.AssigneeEmail = value;
                _asdigneeEmail = value;
            }
        }
        private int _boardId;
        public int BoardId {
            get => _boardId;
            set => _boardId = value;
        }


        /// <summary>
        /// A constructor which checks the legality of it's values
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="dueDate"></param>
        public Task(int taskId, String title, String description, DateTime dueDate, string assigneeEmail, int columnId, int boardId)
        {
            dalTask =  new DalTask();
            CreationTime = DateTime.Now;
            Title = title;
            Description = description;
            DueDate = dueDate;
            AssigneeEmail = assigneeEmail;
            ColumnId = columnId;
            BoardId = boardId;
            Id = taskId;
        }

        /// <summary>
        /// constructor for tests
        /// </summary>
        public Task()
        {
            CreationTime = DateTime.Now;
        }

        /// <summary>
        /// gets a dal task and creats a business layer task
        /// </summary>
        /// <param name="task"></param>
        public Task(DalTask task) {
            dalTask = task;
            _id = task.Id;
            _boardId = task.BoardId;
            _columnId = task.ColumnId;
            _title = task.Title;
            _description = task.Description;
            _dueDate = task.DueDate;
            _creationTime = task.CreationTime;
            _asdigneeEmail = task.AssigneeEmail;
        }

        public void Delete() { dalTask.Delete(); }

        public DateTime GetCreationTime() { return _creationTime; }
    }
}
