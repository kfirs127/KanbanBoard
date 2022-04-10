using Presentaion.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Presentaion.ViewModel
{
    public class TaskViewModel : Notifiable
    {
        private BackendController _controller;
        public BackendController Controller {
            get => _controller;
            private set { _controller = value; } 
        }
        private BoardModel _currBoard;
        public BoardModel CurrBoard {
            get => _currBoard;
            set => _currBoard = value;
        }
        private TaskModel _currTask;
        public TaskModel CurrTask {
            get => _currTask;
            set => _currTask = value;
        }
        private string _loggedInUser;
        public string LoggedInUser {
            get => _loggedInUser;
            set => _loggedInUser = value;
        }
        private string _title;
        public string Title {
            get => _title;
            set {
                _title = value;
                IsAddEnable = true;
                IsSubmitEnable = true;
                RaisePropertyChanged("Title");
            }
        }
        private string _description = "";
        public string Description {
            get => _description;
            set {
                _description = value;
                IsAddEnable = true;
                RaisePropertyChanged("Description");
            }
        }
        private DateTime _dueDate;
        public DateTime DueDate {
            get => _dueDate;
            set {
                _dueDate = value;
                RaisePropertyChanged("DueDaate");
            }
        }
        private DateTime _creationTime;
        public DateTime CreationTime {
            get => _creationTime;
            set => _creationTime = value;
        }

        private string _emailAssignee;
        public string EmailAssignee
        {
            get => _emailAssignee;
            set {
                _emailAssignee = value;
                IsSubmitEnable = true;
                RaisePropertyChanged("AssigneeEmail");
            }
        }
        private bool _isAddEnable = false;
        public bool IsAddEnable {
            get => _isAddEnable;
            set {
                _isAddEnable = (!string.IsNullOrEmpty(Title));
                RaisePropertyChanged("IsAddEnable");
            }
        }
        private int _columnOrdinal;
        public int ColumnOrdinal {
            get => _columnOrdinal;
            set => _columnOrdinal = value;
        }
        private bool _isSubmitEnable;
        public bool IsSubmitEnable {
            get => _isSubmitEnable;
            set
            {
                _isSubmitEnable = (!string.IsNullOrEmpty(Title) & !string.IsNullOrEmpty(EmailAssignee));
                RaisePropertyChanged("IsSubmitEnable");
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="currBoard"></param>
        /// <param name="loggedInUser"></param>
        public TaskViewModel(BackendController controller, BoardModel currBoard, string loggedInUser) {
            Controller = controller;
            LoggedInUser = loggedInUser;
            CurrBoard = currBoard;
        }

        /// <summary>
        /// constructor for an existing task to show
        /// </summary>
        /// <param name="toShow"></param>
        public TaskViewModel(TaskModel toShow)
        {
            Title = toShow.Title;
            Description = toShow.Description;
            DueDate = toShow.DueDate;
            CreationTime = toShow.CreationTime;
            EmailAssignee = toShow.EmailAssignee;
            ColumnOrdinal = toShow.ColumnOrdinal;
        }

        /// <summary>
        /// constructor for editing a task
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="currBoard"></param>
        /// <param name="loggedInUser"></param>
        /// <param name="currTask"></param>
        public TaskViewModel(BackendController controller, BoardModel currBoard, string loggedInUser, TaskModel currTask)
        {
            Controller = controller;
            EmailAssignee  = currTask.EmailAssignee;
            Title = currTask.Title;
            Description = currTask.Description;
            DueDate = currTask.DueDate;
            CreationTime = currTask.CreationTime;
            ColumnOrdinal = currTask.ColumnOrdinal;
            CurrTask = currTask;
            CurrBoard = currBoard;
            LoggedInUser = loggedInUser;
        }

        /// <summary>
        /// add the task
        /// </summary>
        /// <param name="currBoard"></param>
        /// <returns></returns>
        public bool AddTask(BoardModel currBoard) {
            try
            {
                TaskModel newTask = Controller.AddTask(LoggedInUser, Title, Description, DueDate);
                if (!string.IsNullOrEmpty(EmailAssignee))
                    Controller.UpdateAssignee(LoggedInUser, 0, newTask.Id, EmailAssignee);
                currBoard.Columns[0].Tasks.Add(newTask);
                
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        /// <summary>
        /// submit the changes after editing
        /// </summary>
        /// <returns></returns>
        internal bool SubmitChanges()
        {
            try
            {
                if (!CurrTask.Title.Equals(Title))
                {
                    Controller.UpdateTitle(LoggedInUser, CurrTask.ColumnOrdinal, CurrTask.Id, Title);
                    CurrTask.Title = Title;
                }
                if (CurrTask.Description != null && !CurrTask.Description.Equals(Description))
                {
                    Controller.UpdateDescription(LoggedInUser, CurrTask.ColumnOrdinal, CurrTask.Id, Description);
                    CurrTask.Description = Description;
                }
                if (CurrTask.DueDate.CompareTo(DueDate) != 0)
                {
                    Controller.UpdateDueDate(LoggedInUser, CurrTask.ColumnOrdinal, CurrTask.Id, DueDate);
                    CurrTask.DueDate = DueDate;
                }
                if (!CurrTask.EmailAssignee.Equals(EmailAssignee))
                {
                    Controller.UpdateAssignee(LoggedInUser, CurrTask.ColumnOrdinal, CurrTask.Id, EmailAssignee);
                    CurrTask.EmailAssignee = EmailAssignee;
                    CurrTask.IsEditable = false;
                }
                CurrBoard.Columns[ColumnOrdinal].Tasks.Remove(CurrTask);
                CurrBoard.Columns[ColumnOrdinal].Tasks.Add(CurrTask);
                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

        }
    }
}
