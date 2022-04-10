using Presentaion.Model;
using Presentaion.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Presentaion.View
{
    /// <summary>
    /// Interaction logic for EditColumnWindow.xaml
    /// </summary>
    public partial class EditColumnWindow : Window
    {
        private ColumnViewModel viewModel;
        private ColumnModel _currColumn;
        public ColumnModel CurrColumn
        {
            get => _currColumn;
            set => _currColumn = value;
        }
        private BoardModel _currBoard;
        public BoardModel CurrBoard
        {
            get => _currBoard;
            set => _currBoard = value;
        }
        private UserModel _loggedInUser;
        public UserModel LoggedInUser
        {
            get => _loggedInUser;
            set => _loggedInUser = value;
        }

        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="columnToEdit"></param>
        /// <param name="currBoard"></param>
        /// <param name="loggedInUser"></param>
        public EditColumnWindow(BackendController controller, ColumnModel columnToEdit, BoardModel currBoard, UserModel loggedInUser)
        {
            InitializeComponent();
            viewModel = new ColumnViewModel(controller,currBoard, columnToEdit);
            DataContext = viewModel;
            CurrBoard = currBoard;
            LoggedInUser = loggedInUser;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// open the board window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// submit the changes, close the window and open the board window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            bool toClose  = viewModel.Submit(LoggedInUser.Email);
            if (toClose)
                Close();
        }
    }
}
