using IntroSE.Kanban.Backend.BusinessLayer;
using System;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Implements the Board, Column and Task methods
    /// </summary>
    public class UserService
    {
        private Security security;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// A constructor which gets Security from the service
        /// The value in the Security field in this class will be the same as in BoardService class and will be recieved from the Service class
        /// </summary>
        /// <param name="s"></param>
        public UserService(Security s) { security = s; }

        /// <summary>        
        /// Loads the data. Intended be invoked only when the program starts
        /// </summary>
        /// <returns>A response object, or an error message in case of an error.</returns>
        public Response LoadData()
        {
            try
            {
                security.GetUserController().LoadData();
                return new Response();
            }
            catch (KanbanException e)
            {
                return new Response(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response("Error: " + e.Message);
            }
        }

        ///<summary>Remove all persistent data.</summary>
        public Response DeleteData()
        {
            try
            {
                BusinessLayer.UserPackage.UserController uc = security.GetUserController();
                uc.DeleteData();
                return new Response();
            }
            catch (KanbanException e)
            {
                return new Response<Board>(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response<Board>("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="email">The email address of the user to register</param>
        /// <param name="password">The password of the user to register</param>
        /// <param name="nickname">The nickname of the user to register</param>
        /// <returns>A response object, or an error message in case of an error<returns>
        public Response Register(string email, string password, string nickname)
        {
            try
            {
                String newEmail = email.ToLower();
                security.Register(newEmail, password, nickname);
                return new Response();
            }
            catch (KanbanException e)
            {
                return new Response(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Registers a new user and joins the user to an existing board.
        /// </summary>
        /// <param name="email">The email address of the user to register</param>
        /// <param name="password">The password of the user to register</param>
        /// <param name="nickname">The nickname of the user to register</param>
        /// <param name="emailHost">The email address of the host user which owns the board</param>
        /// <returns>A response object. The response should contain a error message in case of an error<returns>
        internal Response Register(string email, string password, string nickname, string emailHost)
        {
            try
            {
                String newEmail = email.ToLower();
                security.Register(newEmail, password, nickname, emailHost.ToLower());
                return new Response();
            }
            catch (KanbanException e)
            {
                return new Response(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Log in an existing user
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response object with a value set to the user, or an error message in case of an error</returns>
        public Response<User> Login(string email, string password)
        {
            try
            {
                String newEmail = email.ToLower();
                BusinessLayer.UserPackage.User u = security.Login(newEmail, password);
                User output = new User(u.Email, u.Nickname);
                return new Response<User>(output);
            }
            catch (KanbanException e)
            {
                return new Response<User>(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response<User>("Error: " + e.Message);
            }
        }


        /// <summary>        
        /// Log out an logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>A response object, or an error message in case of an error</returns>
        public Response Logout(string email)
        {
            String newEmail = email.ToLower();
            try
            {
                security.Logout(newEmail);
                return new Response();
            }
            catch (KanbanException e)
            {
                return new Response(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new Response("Error: " + e.Message);
            }
        }
    }
}
