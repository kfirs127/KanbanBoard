using System;
using System.Windows;
using Presentaion.ViewModel;
using Presentaion.Model;

namespace Presentaion.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LogInViewModel lvm;
       
        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow()
        {
            lvm = new LogInViewModel();
            InitializeComponent();
            this.DataContext = lvm;
        }

        /// <summary>
        /// constructor for an existing window
        /// </summary>
        /// <param name="controller"></param>
        public MainWindow(BackendController controller)
        {
            lvm = new LogInViewModel(controller);
            InitializeComponent();
            this.DataContext = lvm;
        }

        /// <summary>
        /// login and open the board of the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserModel loggedInUser = lvm.LogIn();
                if (loggedInUser != null)
                {
                    BoardWindow boardWindow = new BoardWindow(loggedInUser);
                    boardWindow.Show();
                    this.Close();
                }
            }
            catch(Exception exception)
            {
               MessageBox.Show(exception.Message);
            }
        }

        /// <summary>
        /// open the register window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            new RegisterWindow(lvm.Controller).ShowDialog();
            ShowDialog();
        }

        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lvm.Password = PasswordTextBox.Password;
        }
    }
}
