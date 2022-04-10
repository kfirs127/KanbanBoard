using IntroSE.Kanban.Backend.DataAccessLayer.DalControllers;
using IntroSE.Kanban.Backend.DataAccessLayer.DalObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer.BoardPackage
{
    /// <summary>
    /// has a list of the boars in the system
    /// </summary>
    public class BoardController
    {
        private Dictionary<int, Board> _boards;
        public Dictionary<int, Board> Boards { get => _boards; set => _boards = value; }
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //board id
        private int _counter = 0;
        public int Counter {
            get => _counter;
            set
            {
                if(value > _counter)
                    _counter = value;
            }
        }

        /// <summary>
        /// returns a board by it's id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>board</returns>
        public Board GetBoard(int id)
        {
            if (!Boards.ContainsKey(id))
            {
                log.Warn("attempt to load board which does not exist.");
                throw new KanbanException("Board does not exist in the system.");
            }
            return _boards[id];
        }

        /// <summary>
        /// loads all the boards into the list
        /// </summary>
        public void LoadData() {
            BoardDalController bc = new BoardDalController();
            bool hasBoard = false;
            List<DalBoard> dalBoards = bc.LoadData();
            Boards = new Dictionary<int, Board>();
            foreach (DalBoard toAdd in dalBoards)
            {
                Counter = toAdd.Id;
                Boards.Add(toAdd.Id, new Board(toAdd));
                hasBoard = true;
            }
            if (hasBoard)
                Counter = Counter + 1;
        }

        /// <summary>
        /// creates a new board and adds in to the boards list
        /// </summary>
        /// <param name="creator"></param>
        /// <returns></returns>
        public void CreateBoard(string creator) {
            Board newBoard = new Board(Counter, creator);
            Boards.Add(Counter, newBoard);
            Counter = Counter + 1;
        }

        /// <summary>
        /// returns a boaed by it's creator's email
        /// </summary>
        /// <param name="emailHost"></param>
        /// <returns></returns>
        internal int GetBoard(string emailHost)
        {
            foreach (KeyValuePair<int, Board> board in Boards)
            {
                if (board.Value.Creator.Equals(emailHost))
                    return board.Key;
            }
            throw new KanbanException("There is no such board with this email as a creator.");
        }
    }
}
