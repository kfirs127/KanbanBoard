using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Presentaion.ViewModel
{
    public class RegisterViewModel : Notifiable
    {
        private readonly BackendController _controller;
        public BackendController Controller { get; set; }
        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controller"></param>
        public RegisterViewModel(BackendController controller)
        {
            Controller = controller;
            Email = "";
            Password = "";
            ConfirmPassword = "";
            Nickname = "";
            HostEmail = "";
        }

        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value;
                RaisePropertyChanged("Email");
                IsRegisterEnable = true;
            }
        }

        private string password;
        public string Password
        {
            get => password;
            set
            {
                password = value;
                RaisePropertyChanged("Password");
                IsRegisterEnable = true;
            }

        }

        private string _confirmPassword;
        public string ConfirmPassword {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                RaisePropertyChanged("ConfirmPassword");
                IsRegisterEnable = true;
            }
        }

        private string nickname;
        public string Nickname
        {
            get => nickname;
            set
            {
               nickname = value;
                RaisePropertyChanged("Nickname");
                IsRegisterEnable = true;
            }
        }

        private string hostEmail;
        public string HostEmail
        {
            get => hostEmail;
            set
            {
                hostEmail = value;
                RaisePropertyChanged("HostEmail");
            }
        }

        private bool _isRegisterEnable = false;
        public bool IsRegisterEnable
        {
            get => _isRegisterEnable;
            set
            {
                bool last = _isRegisterEnable;
                _isRegisterEnable = (!String.IsNullOrEmpty(Password) & !String.IsNullOrEmpty(Email) & !String.IsNullOrEmpty(ConfirmPassword) & !String.IsNullOrEmpty(Nickname));
                if (last != _isRegisterEnable)
                    RaisePropertyChanged("IsRegisterEnable");
            }
        }

        /// <summary>
        /// register
        /// </summary>
        /// <returns></returns>
        public bool Register()
        {
            if (Password.Equals(ConfirmPassword))
            {
                try
                {
                    Controller.Register(Email, Password, Nickname, HostEmail);
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Password confirmation does not match.");
                return false;
            }
        }
    }
}
