
using IntroSE.Kanban.Backend.BusinessLayer.BoardPackage;
using IntroSE.Kanban.Backend.DataAccessLayer.DalObjects;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.BusinessLayer.UserPackage
{
    public class User
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private DalUser dalUser;
        private int _id;
        public int Id{
            get { return _id; }
            set
            { 
                dalUser.Id = value;
                _id = value;
            }
        }
        private String _email;
        public String Email{
            get => _email;
            set {
                if(value == null)
                {
                    log.Warn("Cannot register with a null email.");
                    throw new KanbanException("Cannot register with a null email.");
                }
                if(!IsValidEmail(value))
                {
                    log.Warn("The email is illegal.");
                    throw new KanbanException("The email is illegal.");
                }
                dalUser.Email = value;
                _email = value;
            }
        }
        private String _nickname;
        public String Nickname {
            get => _nickname;
            set {
                if (value == null || value.Length < 1)
                {
                    log.Warn("A nickname is missing.");
                    throw new KanbanException("A nickname is missing.");
                }
                dalUser.Nickname = value;
                _nickname = value;
            }
        }
        private String _password;
        public String Password {
            get => _password;
            set{
                if (!PassIsLegal(value))
                {
                    log.Warn("The password is illegal.");
                    throw new KanbanException("The password is illegal.");
                }
                dalUser.Password = value;
                _password = value;
            }
        }
        private int _boardId;
        public int BoardId{
            get => _boardId;
            set => _boardId = value;
        }

        /// <summary>
        /// a constructor which checks the legality of it's values
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="nickname"></param>
        /// <param name="boardId"></param>
        /// <param name="id"></param>
        public User(String email, String password, String nickname, int id, int boardId)
        {
            dalUser = new DalUser();
            Password = password;
            Email = email;
            Nickname = nickname;
            BoardId = boardId;
            Id = id;
            log.Debug("A user was created.");
        }

        /// <summary>
        /// gets a dalUser and create a business layer user
        /// </summary>
        /// <param name="user"></param>
        public User(DalUser user) {
            dalUser = user;
            _password = user.Password;
            _email = user.Email;
            _nickname = user.Nickname;
            _id = user.Id;
            _boardId = user.BoardId;
        }
        

        /// <summary>
        /// check if the email is legal
        /// </summary>
        /// <param name="email"></param>
        /// <returns>true if it's legal and false if not</returns>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
                string DomainMapper(Match match)
                {
                    var idn = new IdnMapping();
                    var domainName = idn.GetAscii(match.Groups[2].Value);
                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }


        /// <summary>
        /// check if the password is legal
        /// </summary>
        /// <param name="password"></param>
        /// <returns>true if its legal and else if not</returns>
        private bool PassIsLegal(String password)
        {
            if (password == null)
                return false;
            if (password.Length > 25 | password.Length < 5) return false;
            bool low = false;
            bool up = false;
            bool num = false;
            for (int i = 0; i < password.Length & (!low | !up | !num ); i++)
            {
                if (System.Char.IsLower(password[i]))
                    low = true;
                else if (System.Char.IsUpper(password[i]))
                    up = true;
                else {
                    try
                    {
                        int check = int.Parse("" + password[i]);
                        if (check <= 9 & check >= 0)
                            num = true;
                    }
                    catch (Exception) { };
                }
            }
            return low & up & num;
        }

        /// <summary>
        /// Log in an existing user
        /// </summary>
        /// <param name="password">The password of the user to login</param>
        /// <returns>The user that logged in</returns>
        public User login(String password)
        {
            if (!password.Equals(_password))
            {
                log.Warn("The password is incorrect.");
                throw new KanbanException("The password is incorrect.");
            }
            log.Debug("A user logged in.");
            return this;
        }

        /// <summary>
        /// Deletes the user from database and all of its boards
        /// </summary>
        public void Delete() {
            dalUser.Delete();
        }

    }
}
