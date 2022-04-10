using IntroSE.Kanban.Backend.BusinessLayer.BoardPackage;
using System;
using IntroSE.Kanban.Backend.BusinessLayer.UserPackage;
using System.Linq;
using System.Collections.Generic;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// Security class - the connecting link between the service layer and the business layer.
    /// in this class will be the checking if the user is logged in.
    /// </summary>
    public class Security
    {
        String currEmail = "";
        private UserController userController;
        private BoardController boardController;
        private Board currBoard;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// A constructor which rests its fields
        /// </summary>
        public Security()
        {
            currEmail = "";
            userController = new UserController();
            boardController = new BoardController();
        }

        /// <summary>
        /// loads the data to the usercontroller and boardcontroller
        /// </summary>
        public void LoadData() {
            userController.LoadData();
            boardController.LoadData();
        }


        public void DeleteData()
        {
            userController.DeleteData();
        }

        /// <summary>
        /// checks if no other user is logged in. if not - calls the nethod in the userController with a new board id and creats the board
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="nickname"></param>
        public void Register(String email, String password, String nickname) {
            if (currEmail.Length > 0)
            {
                log.Error("Register attempt while anothoer user is logged in.");
                throw new KanbanException("There is anothoer user logged in.");
            }
            int boardId = boardController.Counter;
            userController.Register(email, password, nickname,boardId);
            boardController.CreateBoard(email);
        }

        /// <summary>
        /// checks if no other user is logged in. if not - calls the method in the userController with already existing board id
        /// </summary>
        /// <param name="newEmail"></param>
        /// <param name="password"></param>
        /// <param name="nickname"></param>
        /// <param name="emailHost"></param>
        internal void Register(string newEmail, string password, string nickname, string emailHost)
        {
            if (currEmail.Length > 0)
            {
                log.Error("Register attempt while anothoer user is logged in.");
                throw new KanbanException("There is anothoer user logged in.");
            }
            int boardId = boardController.GetBoard(emailHost);
            userController.Register(newEmail, password, nickname, boardId);
        }

        /// <summary>
        /// checks if no other user is logged in. if not - calls the nethod in the userController
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User Login(String email, String password)
        {
            if (currEmail.Length > 0) {
                if (currEmail.Equals(email))
                {
                    log.Error("Login attempt while the user is already logged in.");
                    throw new KanbanException("The user is already logged in.");
                }
                log.Error("Login attempt while anothoer user is logged in.");
                throw new KanbanException("There is anothoer user logged in.");
            }
            User u = userController.Login(email, password);
            currEmail = email;
            currBoard = boardController.GetBoard( userController.GetUser(email).BoardId);
            return u;
        }

        /// <summary>
        /// checking if the email is equals to the email that is logged in now, if yes - resets the fields and logging out
        /// </summary>
        /// <param name="email"></param>
        public void Logout(String email)
        {
            if (!email.Equals(currEmail)) {
                log.Warn("This user is not logged in.");
                throw new KanbanException("This user is not logged in.");
            }
            log.Info("The user " + currEmail + " logged out.");
            currEmail = "";
            currBoard = null;
        }

        public UserController GetUserController()
        {
            return userController;
        }

        public BoardController GetBoardController()
        {
            return boardController;
        }

        /// <summary>
        /// returns the board which is used at the moment if the user is loged in.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>the board which is used at the moment</returns>
        public Board GetBoard(String email)
        {
            String newEmail = email.ToLower();
            if (!currEmail.Equals(newEmail))
            {
                log.Warn("Email " + email + " is invalid.");
                throw new KanbanException("Email " + email + " is invalid.");
            }
            return currBoard;
        }
    }
}
