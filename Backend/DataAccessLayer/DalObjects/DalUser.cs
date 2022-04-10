using IntroSE.Kanban.Backend.DataAccessLayer.DalControllers;
using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DalObjects
{
    /// <summary>
    /// DalUser class - extens DalObject and reprsent the User class in business layer in data access layer
    /// </summary>
    public class DalUser : DalObject<DalUser>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// an empty constructor
        /// </summary>
        public DalUser() : base(new UserDalController()) { _id = -1; }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="nickname"></param>
        public DalUser(int id, string email, string password, string nickname, int boardId) : base(new UserDalController()) {
            _id = id;
            _email = email;
            _password = password;
            _nickname = nickname;
            _boardId = boardId;
        }

        public const String EmailColumnName = "Email";
        public const String PasswordColumnName = "Password";
        public const String NicknameColumnName = "Nickname";
        public const String BoardIdColumnName = "Board_Id";


        private int _id;
        public int Id
        {
            get => _id;
            set {
                _id = value;
                if (!_controller.Insert(this))
                {
                    log.Error("Couldn't save user in database.");
                    throw new Exception("Couldn't save user in database");
                }
            }
        }
        private String _email; 
        public String Email
        {
            get => _email;
            set => _email = value;
        }
        private String _nickname;
        public String Nickname
        {
            get => _nickname;
            set {
                if (_id != -1)
                {
                    if (!_controller.Update(Id, NicknameColumnName, value))
                    {
                        log.Error("Couldn't update nickname in database.");
                        throw new Exception("Couldn't update nickname in database.");
                    }
                }
                _nickname = value;
            }
        }
        private String _password;
        public String Password
        {
            get => _password;
            set {
                if (_id != -1)
                    if (!_controller.Update(Id, PasswordColumnName, value))
                    {
                        log.Error("Couldn't update password in database.");
                        throw new Exception("Couldn't update password in database.");
                    }
                _password = value;
            }
        }
        private int _boardId;
        public int BoardId {
            get => _boardId;
            set {
                if (_id != -1)
                    if (!_controller.Update(Id, BoardIdColumnName, value))
                    {
                        log.Error("Couldn't update password in database.");
                        throw new Exception("Couldn't update password in database.");
                    }
            }
        }

        /// <summary>
        /// Deletes the user from the database
        /// </summary>
        public override void Delete()
        {
            if (!_controller.Delete(this))
            {
                log.Error("Couldn't Delete user from database.");
                throw new Exception("Couldn't Delete user from database.");
            }
        }


    }
}
