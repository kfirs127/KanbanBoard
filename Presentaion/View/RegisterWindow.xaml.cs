using Presentaion.ViewModel;
using System.Windows;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace Presentaion.View
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        RegisterViewModel rvm;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controller"></param>
        public RegisterWindow(BackendController controller)
        {
            rvm = new RegisterViewModel(controller);
            InitializeComponent();
            this.DataContext = rvm;
        }

        /// <summary>
        /// register the user and open the main window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            bool toClose = rvm.Register();
            if(toClose)
                Close();
        }

        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            rvm.Password = PasswordTextBox.Password;
        }

        private void ConfirmPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            rvm.ConfirmPassword = ConfirmPasswordTextBox.Password;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
