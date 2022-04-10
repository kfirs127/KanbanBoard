using Presentaion.Model;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Presentaion.ViewModel
{
    public class BoardViewModel : Notifiable
    {
        private BackendController _controller;
        public BackendController Controller
        {
            get => _controller;
            set => _controller = value;
        }
        private UserModel _loggedInUserModel;
        public UserModel LoggedInUserModel {
            get => _loggedInUserModel;
            set => _loggedInUserModel = value;
        }
        private BoardModel _board;
        public BoardModel Board {
            get => _board;
            set => _board = value;
        }
        private string _searchText;
        public string SearchText {
            get => _searchText;
            set
            {
                _searchText = value;
                RaisePropertyChanged("SearchText");
            }
        }
        private TaskModel _selectedTask;
        public TaskModel SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                SelectedColumn = null;
                RaisePropertyChanged("SelectedTask");
            }
        }
        private ColumnModel _selectedColumn;
        public ColumnModel SelectedColumn
        {
            get => _selectedColumn;
            set
            {
                _selectedColumn = value;
                _selectedTask = null;
                RaisePropertyChanged("SelectedColumn");
            }
        }
        
        private string _newColumnName;
        public string NewColumnName {
            get => _newColumnName;
            set {
                _newColumnName = value;
                RaisePropertyChanged("NewColumnName");
            }
        }
        private string _newColumnOrdinal;
        public string NewColumnOrdinal
        {
            get => _newColumnOrdinal;
            set {
                _newColumnOrdinal = value;
                RaisePropertyChanged("NewColumnOrdinal");
            }
        }

        /// <summary>
        /// check if there is more than 2 columns
        /// </summary>
        private bool _isColumnDeleteble;
        public bool IsColumnDeleteble {
            get => _isColumnDeleteble;
            set {
                _isColumnDeleteble = Board.Columns.Count > 2;
                RaisePropertyChanged("IsColumnDeleteble");
            }
        }


        private bool _isColumnEditableAdd = false;
        public bool IsColumnEditableAdd
        {
            get => _isColumnEditableAdd;
            set
            {
                _isColumnEditableAdd = value;
                RaisePropertyChanged("IsColumnEditableAdd");
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="user"></param>
        public BoardViewModel(UserModel user)
        {
            LoggedInUserModel = user;
            Controller = user.Controller;
            Board = Controller.GetBoard(user);
            foreach (ColumnModel toUpdate in Board.Columns)
                UpdateBool(toUpdate);
            IsColumnDeleteble = Board.Columns.Count > 2;
            IsColumnEditableAdd = Board.Creator.Equals(LoggedInUserModel.Email);
            SearchText = "";
            SelectedTask = null;
        }


        /// <summary>
        /// sort the tasks
        /// </summary>
        internal void Sort()
        {
            foreach (ColumnModel toSort in Board.Columns)
            {
                toSort.Sort();
            }
            setTasksBySearch();
        }

        /// <summary>
        /// update the tasks by search list
        /// </summary>
        internal void setTasksBySearch()
        {
            try
            {
                foreach (ColumnModel toChange in Board.Columns)
                {
                    toChange.SetTaskBySearch(SearchText);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// log out and open the main window
        /// </summary>
        internal void LogoutWithExitButton()
        {
            Controller.Logout(LoggedInUserModel.Email);
        }

        /// <summary>
        /// add a new column
        /// </summary>
        internal void AddColumn()
        {
            try
            {
                ColumnModel toAdd = new ColumnModel(Controller);
                toAdd.Name = NewColumnName;
                toAdd.ColumnOrdinal = int.Parse(NewColumnOrdinal);
                Controller.AddColumn(LoggedInUserModel.Email, toAdd);
                UpdateBool(toAdd);
                foreach (ColumnModel toUpdate in Board.Columns)
                {
                    if (toUpdate.ColumnOrdinal >= int.Parse(NewColumnOrdinal))
                    {
                        toUpdate.ColumnOrdinal += 1;
                        foreach (TaskModel toChange in toUpdate.Tasks)
                            toChange.ColumnOrdinal += 1;
                        UpdateBool(toUpdate);
                    }
                }
                setTasksBySearch();
                Board.Columns.Insert(int.Parse(NewColumnOrdinal), toAdd);
                NewColumnOrdinal = "";
                NewColumnName = "";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// update the bool fields
        /// </summary>
        /// <param name="toUpdate"></param>
        private void UpdateBool(ColumnModel toUpdate)
        {
            if (!Board.Creator.Equals(LoggedInUserModel.Email))
            {
                toUpdate.IsColumnEditable = false;
            }
            else
                toUpdate.IsColumnEditable = true;
            toUpdate.IsColumnEditableRight = toUpdate.IsColumnEditable && (toUpdate.ColumnOrdinal < Board.Columns.Count - 1);
            toUpdate.IsColumnEditableLeft = toUpdate.IsColumnEditable && (toUpdate.ColumnOrdinal > 0);
        }

        /// <summary>
        /// move the column right
        /// </summary>
        /// <param name="toMove"></param>
        internal void MoveColumnRight(ColumnModel toMove)
        {
            try
            {
                Controller.MoveColumnRight(LoggedInUserModel.Email, toMove);
                ColumnModel copy = new ColumnModel(toMove);
                copy.ColumnOrdinal = copy.ColumnOrdinal + 1;
                Board.Columns.Remove(toMove);
                Board.Columns.Insert(copy.ColumnOrdinal, copy);
                Board.Columns[copy.ColumnOrdinal - 1].ColumnOrdinal -= 1;
                foreach (TaskModel toUpate in copy.Tasks)
                {
                    toUpate.ColumnOrdinal = toUpate.ColumnOrdinal + 1;
                    if(copy.ColumnOrdinal == Board.Columns.Count - 1)
                        toUpate.IsAdvanceble = false;
                }
                foreach (TaskModel toUpdate in Board.Columns[copy.ColumnOrdinal - 1].Tasks)
                {
                    toUpdate.ColumnOrdinal = toUpdate.ColumnOrdinal - 1;
                    toUpdate.IsAdvanceble = true;
                }
                UpdateBool(copy);
                UpdateBool(Board.Columns[copy.ColumnOrdinal - 1]);
                setTasksBySearch();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        
        /// <summary>
        /// move the column left
        /// </summary>
        /// <param name="toMove"></param>
        internal void MoveColumnLeft(ColumnModel toMove)
        {
            try
            {
                Controller.MoveColumnLeft(LoggedInUserModel.Email, toMove);
                ColumnModel copy = new ColumnModel(toMove);
                copy.ColumnOrdinal = copy.ColumnOrdinal - 1;
                Board.Columns.Remove(toMove);
                Board.Columns.Insert(copy.ColumnOrdinal, copy);
                Board.Columns[copy.ColumnOrdinal + 1].ColumnOrdinal += 1;
                foreach (TaskModel toUpate in copy.Tasks)
                {
                    toUpate.ColumnOrdinal = toUpate.ColumnOrdinal - 1;
                    toUpate.IsAdvanceble = true;
                }
                foreach (TaskModel toUpdate in Board.Columns[copy.ColumnOrdinal + 1].Tasks)
                {
                    toUpdate.ColumnOrdinal = toUpdate.ColumnOrdinal - 1;
                    if (copy.ColumnOrdinal + 1 == Board.Columns.Count - 1)
                        toUpdate.IsAdvanceble = false;
                }
                UpdateBool(copy);
                UpdateBool(Board.Columns[copy.ColumnOrdinal + 1]);
                setTasksBySearch();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// log out
        /// </summary>
        internal void Logout()
        {
            try
            {
                Controller.Logout(LoggedInUserModel.Email);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// delete the column
        /// </summary>
        /// <param name="toDelete"></param>
        internal void DeleteColumn(ColumnModel toDelete)
        {
            try
            {
                Controller.DeleteColumn(Board.Creator, toDelete);
                if (toDelete.ColumnOrdinal == 0)
                    MoveTasks(1, toDelete);
                else
                    MoveTasks(toDelete.ColumnOrdinal - 1, toDelete);
                foreach (ColumnModel toUpdate in Board.Columns)
                {
                    if (toUpdate.ColumnOrdinal > toDelete.ColumnOrdinal)
                        toUpdate.ColumnOrdinal -= 1;
                }
                Board.DeleteColumn(toDelete);
                setTasksBySearch();
                IsColumnDeleteble = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }

        /// <summary>
        /// delete the task
        /// </summary>
        /// <param name="toDelete"></param>
        internal void DeleteTask(TaskModel toDelete)
        {
            try
            {
                Controller.DeleteTask(LoggedInUserModel.Email, toDelete, toDelete.ColumnOrdinal);
                Board.Columns[toDelete.ColumnOrdinal].Tasks.Remove(toDelete);
                setTasksBySearch();
            }
            catch (Exception e) {
                MessageBox.Show(e.Message);
            }

        }

        /// <summary>
        /// advance the task
        /// </summary>
        /// <param name="toAdvance"></param>
        internal void AdvanceTask(TaskModel toAdvance)
        {
            try
            {
                Controller.AdvanceTask(LoggedInUserModel.Email, toAdvance, toAdvance.ColumnOrdinal);
                Board.Columns[toAdvance.ColumnOrdinal].Tasks.Remove(toAdvance);
                toAdvance.ColumnOrdinal = toAdvance.ColumnOrdinal + 1;
                if (toAdvance.ColumnOrdinal == Board.Columns.Count - 1)
                    toAdvance.IsAdvanceble = false;
                Board.Columns[toAdvance.ColumnOrdinal].Tasks.Add(toAdvance);
                setTasksBySearch();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// move the tasks from one column to anothe
        /// </summary>
        /// <param name="desOrdinal"></param>
        /// <param name="source"></param>
        private void MoveTasks(int desOrdinal, ColumnModel source) {
            foreach (TaskModel toAdd in source.Tasks)
            {
                Board.Columns[desOrdinal].Tasks.Add(toAdd);
                if (source.ColumnOrdinal > 0)
                {
                    toAdd.ColumnOrdinal = desOrdinal;
                }
            }
        }

    }
}
