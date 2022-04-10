using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentaion.Model
{
    public class ColumnModel : NotifiableModel
    {
        private ObservableCollection<TaskModel> _tasks;
        public ObservableCollection<TaskModel> Tasks {
            get => _tasks;
            set {
                _tasks = value;
                RaisePropertyChanged("Tasks");
            }
        }
        private ObservableCollection<TaskModel> _tasksBySearch;
        public ObservableCollection<TaskModel> TasksBySearch
        {
            get => _tasksBySearch;
            set
            {
                _tasksBySearch = value;
                RaisePropertyChanged("TasksBySearch");
            }
        }
        private string _name;
        public string Name {
            get => _name;
            set {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private int _limit;
        public int Limit {
            get => _limit;
            set {
                _limit = value;
                RaisePropertyChanged("Limit");
            }
        }
        private int _columnOrdinal;
        public int ColumnOrdinal {
            get => _columnOrdinal;
            set {
                _columnOrdinal = value;
                RaisePropertyChanged("ColumnOrdinal");
            }
        }
        private bool _isColumnEditableLeft;
        public bool IsColumnEditableLeft
        {
            get => _isColumnEditableLeft;
            set {
                _isColumnEditableLeft = value;
                RaisePropertyChanged("IsColumnEditableLeft");
            }
        }

        private bool _isColumnEditableRight;
        public bool IsColumnEditableRight
        {
            get => _isColumnEditableRight;
            set
            {
                _isColumnEditableRight = value;
                RaisePropertyChanged("IsColumnEditableRight");
            }
        }
        private bool _isColumnEditable;
        public bool IsColumnEditable
        {
            get => _isColumnEditable;
            set
            {
                _isColumnEditable = value;
                RaisePropertyChanged("IsColumnEditable");
            }
        }

        /// <summary>
        /// a constructor for an existing column - to edit
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="limit"></param>
        /// <param name="lastColumn"></param>
        public ColumnModel(BackendController controller, string email, string name, int columnOrdinal, int limit, int lastColumn) : base(controller)
        {
            Name = name;
            Tasks = Controller.GetTasks(email, name, columnOrdinal, lastColumn);
            TasksBySearch = new ObservableCollection<TaskModel>(Tasks);
            ColumnOrdinal = columnOrdinal;
            Limit = limit;
        }

        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="controller"></param>
        public ColumnModel(BackendController controller) : base(controller)
        {
            Name = "";
            Tasks = new ObservableCollection<TaskModel>();
            TasksBySearch = new ObservableCollection<TaskModel>(Tasks);
            Limit = 100;
        }

        /// <summary>
        /// a copy constructor
        /// </summary>
        /// <param name="toCopy"></param>
        public ColumnModel(ColumnModel toCopy) : base(toCopy.Controller)
        {
            Name = toCopy.Name;
            Tasks = toCopy.Tasks;
            TasksBySearch = toCopy.TasksBySearch;
            Limit = toCopy.Limit;
            ColumnOrdinal = toCopy.ColumnOrdinal;
        }

        /// <summary>
        /// updating the taskBySearch list by the search textbox
        /// </summary>
        /// <param name="value"></param>
        internal void SetTaskBySearch(string value)
        {
            if (string.IsNullOrEmpty(value))
                TasksBySearch = new ObservableCollection<TaskModel>(Tasks);
            else
            {
                ObservableCollection<TaskModel> updated = new ObservableCollection<TaskModel>();
                foreach (TaskModel task in Tasks)
                {
                    if (task.Title.Contains(value) | (task.Description != null && task.Description.Contains(value)))
                        updated.Add(task);
                    task.BackgroundColor = "white";
                }
                TasksBySearch = updated;
            }
        }

        /// <summary>
        /// sort the tasks by it's duedate
        /// </summary>
        internal void Sort()
        {
            List<TaskModel> sorted = new List<TaskModel>(Tasks);
            sorted.Sort((x, y) => DateTime.Compare(x.DueDate, y.DueDate));
            Tasks = new ObservableCollection<TaskModel>(sorted);
        }
    }
}
