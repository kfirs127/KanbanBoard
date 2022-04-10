using Presentaion.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Presentaion.ViewModel
{
    class ColumnViewModel : Notifiable
    {
        private BackendController _controller;
        public BackendController Controller {
            get => _controller;
            set => _controller = value;
        }
        private ColumnModel _columnToEdit;
        public ColumnModel ColumnToEdit {
            get => _columnToEdit;
            set => _columnToEdit = value;
        }

        private BoardModel _currBoard;
        public BoardModel CurrBoard
        {
            get => _currBoard;
            set => _currBoard = value;
        }
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }
        private int _limit;
        public int Limit
        {
            get => _limit;
            set
            {
                _limit = value;
                RaisePropertyChanged("Limit");
            }
        }
        private int _columnOrdinal;
        public int ColumnOrdinal {
            get => _columnOrdinal;
            set => _columnOrdinal = value;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="currBoard"></param>
        /// <param name="columnToEdit"></param>
        public ColumnViewModel(BackendController controller, BoardModel currBoard, ColumnModel columnToEdit) {
            Controller = controller;
            ColumnToEdit = columnToEdit;
            Name = columnToEdit.Name;
            Limit = columnToEdit.Limit;
            CurrBoard = currBoard;
        }

        /// <summary>
        /// constructor for a new column
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="currBoard"></param>
        public ColumnViewModel(BackendController controller, BoardModel currBoard)
        {
            Controller = controller;
            ColumnToEdit = null;
            CurrBoard = currBoard;
        }

        /// <summary>
        /// submit the changes
        /// </summary>
        /// <param name="loggenInUser"></param>
        /// <returns></returns>
        internal bool Submit(string loggenInUser)
        {
            try
            {
                if (!ColumnToEdit.Name.Equals(Name))
                {
                    Controller.ChangeColumnName(loggenInUser, ColumnToEdit.ColumnOrdinal, Name);
                    ColumnToEdit.Name = Name;
                }
                if(ColumnToEdit.Limit != Limit)
                {
                    Controller.ChangeLimit(loggenInUser, ColumnToEdit.ColumnOrdinal, Limit);
                    ColumnToEdit.Limit = Limit;
                }
                CurrBoard.Columns.Remove(ColumnToEdit);
                CurrBoard.Columns.Insert(ColumnToEdit.ColumnOrdinal, ColumnToEdit);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }
    }
}
