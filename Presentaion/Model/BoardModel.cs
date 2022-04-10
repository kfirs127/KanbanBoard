using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentaion.Model
{
    public class BoardModel : NotifiableModel
    {
        private string _creator;
        public string Creator {
            get => _creator;
            set {
                _creator = value;
                RaisePropertyChanged("Creator");
            }
        }
        private ObservableCollection<ColumnModel> _columns;
        public ObservableCollection<ColumnModel> Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                RaisePropertyChanged("Columns");
            }
        }
        
        /// <summary>
        /// updating if the use can delete a column (if there are more than 2 columns
        /// </summary>
        private bool _DeleteAvialble = false;
        public bool DeleteAvialble {
            get => _DeleteAvialble;
            set {
                _DeleteAvialble = value;
                RaisePropertyChanged("DeleteAvialble");
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="backendController"></param>
        /// <param name="columns"></param>
        /// <param name="creator"></param>
        public BoardModel(BackendController backendController, ObservableCollection<ColumnModel> columns, string creator) : base(backendController) {
            Columns = columns;
            Creator = creator;
        }

        /// <summary>
        /// delete a column
        /// </summary>
        /// <param name="toDelete"></param>
        public void DeleteColumn(ColumnModel toDelete) {
            Columns.Remove(toDelete);
            RaisePropertyChanged("Columns");
        }

        /// <summary>
        /// add a new column
        /// </summary>
        /// <param name="toAdd"></param>
        public void AddColumn(ColumnModel toAdd) {
            Columns.Add(toAdd);
            RaisePropertyChanged("Columns");
        }
    }
}
