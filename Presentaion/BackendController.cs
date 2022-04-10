using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presentaion.Model;
using IntroSE.Kanban.Backend.ServiceLayer;
using System.Collections.ObjectModel;
using Task = IntroSE.Kanban.Backend.ServiceLayer.Task;
using System.Collections.Specialized;

namespace Presentaion
{

    public class BackendController
    {

        private Service _service;
        public Service Service
        {
            get => _service;
            set => _service = value; 
        }

        public BackendController()
        {
            Service = new Service();
        }

        internal void LoadData()
        {
            Response r = Service.LoadData();
            if (r.ErrorOccured) {
                throw new Exception(r.ErrorMessage);
            }
        }

        public UserModel Login(string email, string password)
        {
            Response<User> user = Service.Login(email, password);
            if (user.ErrorOccured)
            {
                throw new Exception(user.ErrorMessage);
            }
            return new UserModel(this, email);
        }

        public void Register(string username, string password,string nickname, string hostEmail)
        {
            Response user;
            if (hostEmail.Length == 0)
                user = Service.Register(username, password, nickname);
            else
                user = Service.Register(username, password, nickname, hostEmail);
            if (user.ErrorOccured)
            {
                throw new Exception(user.ErrorMessage);
            }
        }

        internal ObservableCollection<TaskModel> GetTasks(string email, string name, int columnOrdinal, int lastColumn)
        {
            Response<Column> column = Service.GetColumn(email, name);
            if (column.ErrorOccured)
            {
                throw new Exception(column.ErrorMessage);
            }
            ObservableCollection<TaskModel> tasks = new ObservableCollection<TaskModel>();
            foreach (Task task in column.Value.Tasks)
            {
                TaskModel toAdd;
                toAdd = new TaskModel(this, task.Title, task.Description, task.DueDate, task.CreationTime, task.emailAssignee, task.Id, columnOrdinal, email, columnOrdinal < lastColumn);
                tasks.Add(toAdd);
            }
            return tasks;
        }

        internal void UpdateTitle(string loggedInUser, int columnOrdinal, int id, string title)
        {
            Response r = Service.UpdateTaskTitle(loggedInUser, columnOrdinal, id, title);
            if (r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

        internal void UpdateDescription(string loggedInUser, int columnOrdinal, int id, string description)
        {
            Response r = Service.UpdateTaskDescription(loggedInUser, columnOrdinal, id, description);
            if (r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

        internal void UpdateDueDate(string loggedInUser, int columnOrdinal, int id, DateTime dueDate)
        {
            Response r = Service.UpdateTaskDueDate(loggedInUser, columnOrdinal, id, dueDate);
            if (r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

        internal void UpdateAssignee(string loggedInUser, int columnOrdinal, int id, string assignee)
        {
            Response r = Service.AssignTask(loggedInUser, columnOrdinal, id, assignee);
            if (r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

        internal void ChangeLimit(string loggenInUser, int columnOrdinal, int limit)
        {
            Response r = Service.LimitColumnTasks(loggenInUser, columnOrdinal, limit);
            if (r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

        internal void ChangeColumnName(string loggedInUser, int columnOrdinal, string name)
        {
            Response r = Service.ChangeColumnName(loggedInUser, columnOrdinal, name);
            if (r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

        internal void AddColumn(string creator, ColumnModel columnToAdd)
        {
            Response<Column> r = Service.AddColumn(creator, columnToAdd.ColumnOrdinal, columnToAdd.Name);
            if (r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

        internal void MoveColumnRight(string loggenInUser, ColumnModel selectedColumn)
        {
            Response<Column> r = Service.MoveColumnRight(loggenInUser, selectedColumn.ColumnOrdinal);
            if (r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

        internal void MoveColumnLeft(string loggenInUser, ColumnModel selectedColumn)
        {
            Response<Column> r = Service.MoveColumnLeft(loggenInUser, selectedColumn.ColumnOrdinal);
            if (r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

        internal ObservableCollection<ColumnModel> GetColumns(BoardModel board)
        {
            Response<Board> currBoard = Service.GetBoard(board.Creator);
            if (currBoard.ErrorOccured)
            {
                throw new Exception(currBoard.ErrorMessage);
            }
            ObservableCollection<ColumnModel> columns = new ObservableCollection<ColumnModel>();
            int ordinal = 0;
            foreach (string columnName in currBoard.Value.ColumnsNames)
            {
                Response<Column> c = Service.GetColumn(board.Creator, columnName);
                ColumnModel toAdd;
                toAdd = new ColumnModel(this, board.Creator, columnName, ordinal++, c.Value.Limit, currBoard.Value.ColumnsNames.Count - 1);
                columns.Add(toAdd);
            }
            return columns;
        }

        internal TaskModel AddTask(string loggedInUser, string title, string description, DateTime dueDate)
        {
            Response<Task> newTask = Service.AddTask(loggedInUser, title, description, dueDate);
            if (newTask.ErrorOccured)
                throw new Exception(newTask.ErrorMessage);
            TaskModel output = new TaskModel(this, newTask.Value.Title, newTask.Value.Description, newTask.Value.DueDate, newTask.Value.CreationTime, newTask.Value.emailAssignee, newTask.Value.Id, 0, loggedInUser, true);
            return output;
        }

        internal void DeleteColumn(string creator, ColumnModel columnToDelete)
        {
            Response r = Service.RemoveColumn(creator, columnToDelete.ColumnOrdinal);
            if (r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

        internal BoardModel GetBoard(UserModel user)
        {
            Response<Board> board = Service.GetBoard(user.Email);
            if (board.ErrorOccured)
            {
                throw new Exception(board.ErrorMessage);
            }
            int ordinal = 0;
            ObservableCollection<ColumnModel> columns = new ObservableCollection<ColumnModel>();

            foreach (string columnName in board.Value.ColumnsNames) {
                ColumnModel toAdd;
                Column c = Service.GetColumn(user.Email, columnName).Value;
                toAdd = new ColumnModel(this, user.Email, c.Name, ordinal++, c.Limit , board.Value.ColumnsNames.Count - 1);
                columns.Insert(ordinal-1, toAdd);
            }
            return new BoardModel(this, columns, board.Value.emailCreator);
        }

        internal void DeleteTask(string loggedInUser, TaskModel selectedTask, int ordinal)
        {
            Response r = Service.DeleteTask(loggedInUser, ordinal, selectedTask.Id);
            if (r.ErrorOccured)
                throw new Exception(r.ErrorMessage);
        }

        internal void Logout(string email)
        {
            Response r = Service.Logout(email);
            if (r.ErrorOccured)
            {
                throw new Exception(r.ErrorMessage);
            }
        }

        internal void AdvanceTask(string email, TaskModel selectedTask, int columnOrdinal)
        {
            Response r = Service.AdvanceTask(email, columnOrdinal, selectedTask.Id);
            if (r.ErrorOccured)
            {
                throw new Exception(r.ErrorMessage);
            }
        }
    }
}
