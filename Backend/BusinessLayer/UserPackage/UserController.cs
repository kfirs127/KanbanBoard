using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.BusinessLayer.BoardPackage;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DalControllers;
using IntroSE.Kanban.Backend.DataAccessLayer.DalObjects;
using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.BusinessLayer.UserPackage
{
    public class UserController
    {
        private Dictionary<string, User> _users;
        public Dictionary<string, User> Users { get => _users; }
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private int counter = 0;

        /// <summary>        
        /// Loads the data. Intended be invoked only when the program starts
        /// </summary>
        public void LoadData()
        {
            UserDalController dc = new UserDalController();
            List<DalUser> dalUsers =  dc.LoadData();
            _users = new Dictionary <String, User>();
            foreach (DalUser toAdd in dalUsers)
            {
                counter = Math.Max(toAdd.Id, counter);
                Users.Add(toAdd.Email, new User(toAdd));
            }
            if (Users.Count > 0)
                counter++;

        }

        /// <summary>
        /// Deletes all the data from database
        /// </summary>
        public void DeleteData()
        {
            UserDalController dc = new UserDalController();
            dc.DeleteAllData();
            log.Debug("Data was Deleted.");
        }

        /// <summary>
        /// if legal - creates a new user and adds it to the users list
        /// </summary>
        /// <param name="email">The email address of the user to create</param>
        /// <param name="password">The password of the user to create</param>
        /// <param name="nickname">The nickname of the user to create</param>
        public void Register(String email, String password, String nickname, int boardId)
        {
            User user;
            if (_users.ContainsKey(email))
            {
                log.Warn("The email is already exist in the system.");
                throw new KanbanException("The email is already exist in the system.");
            }
            user = new User(email, password, nickname, counter++, boardId);
            _users.Add(email, user);
            log.Info("A new user was registered.");
        }

        /// <summary>
        /// Log in an existing user - checks if the user exist and calls to the method in User
        /// </summary>
        /// <param name="password">The password of the user to login</param>
        /// <returns>The user that logged in</returns>
        public User Login(String Email, String Password)
        {
            User user = GetUser(Email);
            return user.login(Password);
        }

        public User GetUser(String email)
        {
            try
            {
                User user = _users[email];
                return user;
            }
            catch (KeyNotFoundException)
            {
                log.Warn("unrecognize email.");
                throw new KanbanException("The email does not exist in the system.");
            }
        }
    }
}
