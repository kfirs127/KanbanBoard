using Presentaion.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentaion.Model
{
    public class UserModel : NotifiableModel
    {
        private string email;
        public string Email
        {
            get => email;
            set
            {
                this.email = value;
                RaisePropertyChanged("Email");
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="Email"></param>
        public UserModel(BackendController controller , string Email) : base(controller)
        {
            this.email = Email;
        }
    }
}
