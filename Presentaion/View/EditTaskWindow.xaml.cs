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
    /// Interaction logic for EditTaskWindow.xaml
    /// </summary>
    public partial class EditTaskWindow : Window
    {
        private TaskViewModel viewModel;

        /// <summary>
        /// constructor for window for creating new task
        /// </summary>
        /// <param name="loggedInUser"></param>
        public EditTaskWindow(UserModel loggedInUser)
        {
            InitializeComponent();
            viewModel = new TaskViewModel(loggedInUser.Controller, viewModel.CurrBoard, loggedInUser.Email);
            DataContext = viewModel;
        }

        /// <summary>
        /// constructor for editing an existing task
        /// </summary>
        /// <param name="loggedInUser"></param>
        /// <param name="currBoard"></param>
        /// <param name="currTask"></param>
        public EditTaskWindow(UserModel loggedInUser, BoardModel currBoard, TaskModel currTask)
        {
            InitializeComponent();
            viewModel = new TaskViewModel(loggedInUser.Controller, currBoard, loggedInUser.Email, currTask);
            DataContext = viewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// submit the changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            bool toClose = viewModel.SubmitChanges();
            if (toClose)
                Close();
        }

        /// <summary>
        /// cancel and close the window and open the board window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
