using Presentaion.Model;
using Presentaion.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Presentaion.View
{
    /// <summary>
    /// Interaction logic for BoardWindow.xaml
    /// </summary>
    public partial class BoardWindow : Window
    {
        private BoardViewModel viewModel;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="user"></param>
        public BoardWindow(UserModel user)
        {
            InitializeComponent();
            viewModel = new BoardViewModel(user);
            this.DataContext = viewModel;
        }

        /// <summary>
        /// login the user and open the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Logout();
            MainWindow mainWindow = new MainWindow(viewModel.Controller);
            mainWindow.Show();
            Close();
        }

        /// <summary>
        /// open the add task window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTaskClick(object sender, RoutedEventArgs e)
        {
            Hide();
            new AddTaskWindow(viewModel.LoggedInUserModel, viewModel.Board).ShowDialog();
            viewModel.setTasksBySearch();
            ShowDialog();
        }

        /// <summary>
        /// delete he column where the button is
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteColumnClick(object sender, RoutedEventArgs e)
        {
            viewModel.DeleteColumn((sender as Button).DataContext as ColumnModel);
        }

        /// <summary>
        /// open edit column window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditColumnClick(object sender, RoutedEventArgs e)
        {
            Hide();
            new EditColumnWindow(viewModel.Controller, (sender as Button).DataContext as ColumnModel, viewModel.Board, viewModel.LoggedInUserModel).ShowDialog();
            ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                viewModel.LogoutWithExitButton();
                MainWindow mainWindow = new MainWindow(viewModel.Controller);
                mainWindow.Show();
            }
            catch{}
        }

        /// <summary>
        /// add a new column by the text boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddColumnClick(object sender, RoutedEventArgs e)
        {
            viewModel.AddColumn();
            viewModel.IsColumnDeleteble = true;
        }

        /// <summary>
        /// move the column where the button is right
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveRichtCLick(object sender, RoutedEventArgs e)
        {
            viewModel.MoveColumnRight((sender as Button).DataContext as ColumnModel);
        }

        /// <summary>
        /// move the column where the button is left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveLeftCLick(object sender, RoutedEventArgs e)
        {
            viewModel.MoveColumnLeft((sender as Button).DataContext as ColumnModel);
        }

        /// <summary>
        /// open the edit task window for the task where he button is
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTaskClick(object sender, RoutedEventArgs e) {
            Hide();
            TaskModel toEdit = (sender as Button).DataContext as TaskModel;
            new EditTaskWindow(viewModel.LoggedInUserModel, viewModel.Board, toEdit).ShowDialog();
            ShowDialog();
        }

        /// <summary>
        /// delete the task where the button is
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteTaskClick(object sender, RoutedEventArgs e) {
            viewModel.DeleteTask((sender as Button).DataContext as TaskModel);
        }

        /// <summary>
        /// advance the task where the button is
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanceTaskClick(object sender, RoutedEventArgs e) {
            viewModel.AdvanceTask((sender as Button).DataContext as TaskModel);
        }

        /// <summary>
        /// sort the tasks by the due date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SortTasksClick(object sender, RoutedEventArgs e)
        {
            viewModel.Sort();
        }

        /// <summary>
        /// searc tasks by filter 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchClick(object sender, RoutedEventArgs e)
        {
            viewModel.setTasksBySearch();
        }

        /// <summary>
        /// open the task details by ckicking twice on the task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OpenTask(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2) {
                Hide();
                TaskModel toShow = (sender as StackPanel).DataContext as TaskModel;
                new TaskViewWindow(toShow).ShowDialog();
                ShowDialog();
            }
        }
    }
}
