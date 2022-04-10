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
    /// Interaction logic for AddTaskWindow.xaml
    /// </summary>
    public partial class AddTaskWindow : Window
    {
        private TaskViewModel viewModel;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="user"></param>
        /// <param name="currBoard"></param>
        public AddTaskWindow (UserModel user, BoardModel currBoard)
        {
            InitializeComponent();
            viewModel = new TaskViewModel(user.Controller, currBoard, user.Email);
            DataContext = viewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// add the task and close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            bool toClose = viewModel.AddTask(viewModel.CurrBoard);
            if (toClose)
                Close();
        }

        /// <summary>
        /// cancel and open the board window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
