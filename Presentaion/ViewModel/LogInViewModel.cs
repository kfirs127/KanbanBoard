using Presentaion.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Presentaion.ViewModel
{
    class LogInViewModel : Notifiable
    {
        private readonly BackendController _controller;
        public BackendController Controller { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public LogInViewModel()
        {
            Controller = new BackendController();
            Email = "";
            Password = "";
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controller"></param>
        public LogInViewModel(BackendController controller)
        {
            Controller = controller;
            Email = "";
            Password = "";
        }


        private string email;
        public string Email {
            get => email;
            set {
                email = value;
                RaisePropertyChanged("Email");
                IsLoginEnable = true;
            }
        }

        private string password;
        public string Password
        {
            get => password;
            set {
                password = value;
                RaisePropertyChanged("Password");
                IsLoginEnable = true;
            }
        }
        /// <summary>
        /// check if the text boxes are not empty
        /// </summary>
        private bool _isLoginEnable = false;
        public bool IsLoginEnable
        {
            get => _isLoginEnable;
            set {
                bool last = _isLoginEnable;
                _isLoginEnable = (!String.IsNullOrEmpty(Password) & !String.IsNullOrEmpty(Email));
                if(last != _isLoginEnable)
                    RaisePropertyChanged("IsLoginEnable");
            }
        }

        /// <summary>
        /// load the data
        /// </summary>
        internal void LoadData()
        {
            try
            {
                Controller.LoadData();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// login
        /// </summary>
        /// <returns></returns>
        public UserModel LogIn()
        {
            try
            {
                UserModel user = Controller.Login(Email, Password);
                return user;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
    }
}
