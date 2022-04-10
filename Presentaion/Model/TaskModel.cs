using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Presentaion.Model
{
    public class TaskModel : NotifiableModel
    {
        private string _loggedIn;
        public string LoggedIn {
            get => _loggedIn;
            set => _loggedIn = value;
        }
        private string _title;
        public string Title {
            get => _title;
            set {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }
        private string _description;
        public string Description {
            get => _description;
            set {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }
        private DateTime _dueDate;
        public DateTime DueDate {
            get => _dueDate;
            set {
                _dueDate = value;
                RaisePropertyChanged("DueDate");
            }
        }
        private DateTime _creationTime;
        public DateTime CreationTime {
            get => _creationTime;
            set {
                _creationTime = value;
                RaisePropertyChanged("CraetionTime");
            }
        }
        private string _emailAssignee;
        public string EmailAssignee {
            get => _emailAssignee;
            set {
                _emailAssignee = value;
                RaisePropertyChanged("EmailAssignee");
            }
        }
        private int _id;
        public int Id {
            get => _id;
            set => _id = value;
        }
        private int _columnOrdinal;
        public int ColumnOrdinal {
            get => _columnOrdinal;
            set => _columnOrdinal = value;
        }
        private bool _isEditable;
        public bool IsEditable {
            get => _isEditable;
            set
            {
                _isEditable = value;
                RaisePropertyChanged("IsEditable");
            }
        }
        private bool _isAdvaneble;
        public bool IsAdvanceble
        {
            get => _isAdvaneble;
            set
            {
                _isAdvaneble = value;
                RaisePropertyChanged("IsAdvanceble");
            }
        }
        /// <summary>
        /// backgroung of the task by the time passing
        /// </summary>
        private string _backgroundColor;
        public string BackgroundColor {
            get => _backgroundColor;
            set {
                TimeSpan fromCreationTillNow = DateTime.Now.Subtract(CreationTime);
                TimeSpan fromCreationTillDueDate = DueDate.Subtract(CreationTime);
                if (fromCreationTillDueDate <= fromCreationTillNow)
                    _backgroundColor = "red";
                else if (fromCreationTillNow.TotalMinutes / fromCreationTillDueDate.TotalMinutes >= 0.75)
                    _backgroundColor = "orange";
                else
                    _backgroundColor = "white";
            }
        }
        /// <summary>
        /// border of the task by the assignee - if it's the logged in user its blue
        /// </summary>
        public string BorderColor
        {
            get
            {
                return (EmailAssignee.Equals(LoggedIn) ? "Blue" : "gray");
            }
        }
        
        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="backendController"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="dueDate"></param>
        /// <param name="creationTime"></param>
        /// <param name="emailAssignee"></param>
        /// <param name="id"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="loggedInUser"></param>
        /// <param name="isAdvanceble"></param>
        public TaskModel(BackendController backendController, string title, string description, DateTime dueDate, DateTime creationTime, string emailAssignee, int id, int columnOrdinal, string loggedInUser, bool isAdvanceble) : base(backendController)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            CreationTime = creationTime;
            EmailAssignee = emailAssignee;
            Id = id;
            ColumnOrdinal = columnOrdinal;
            LoggedIn = loggedInUser;
            IsEditable = LoggedIn.Equals(EmailAssignee);
            IsAdvanceble = isAdvanceble;
            BackgroundColor = "white";
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="toCopy"></param>
        public TaskModel(TaskModel toCopy) : base (toCopy.Controller)
        {
            Title = toCopy.Title;
            Description = toCopy.Description;
            DueDate = toCopy.DueDate;
            CreationTime = toCopy.CreationTime;
            EmailAssignee = toCopy.EmailAssignee;
            Id = toCopy.Id;
            ColumnOrdinal = toCopy.ColumnOrdinal;
            LoggedIn = toCopy.LoggedIn;
            IsEditable = toCopy.IsEditable;
            IsAdvanceble = toCopy.IsAdvanceble;
            BackgroundColor = "white";
        }

    }
}
