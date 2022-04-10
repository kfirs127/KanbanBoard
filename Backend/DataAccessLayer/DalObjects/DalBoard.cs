using IntroSE.Kanban.Backend.DataAccessLayer.DalControllers;
using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DalObjects
{
    /// <summary>
    /// DalBoard class - extens DalObject and reprsent the Board class in business layer in data access layer
    /// </summary>
    public class DalBoard : DalObject<DalBoard>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DalBoard() : base(new BoardDalController()) {
            _id = 1;
        }
        public const String CreatorColumnName = "Creator";

        public DalBoard(int id, string creator) : base(new BoardDalController())
        {
            _id = id;
            _creator = creator;
        }
        public List<DalColumn> columns { get; set; }
        private int _id; 
        public virtual int Id
        {
            get => _id;
            set
            {
                _id = value;
                if (!_controller.Insert(this))
                {
                    log.Error("Couldn't save board in database");
                    throw new Exception("Couldn't save board in database");
                }
            }
        }
        private string _creator;
        public string Creator
        {
            get => _creator;
            set => _creator = value;
        }

        /// <summary>
        /// Deletes the board from database
        /// </summary>
        public override void Delete()
        {
            if (!_controller.Delete(this))
            {
                log.Error("Couldn't Delete board from database");
                throw new Exception("Couldn't Delete board from database.");
            }   
        }
    }
}
