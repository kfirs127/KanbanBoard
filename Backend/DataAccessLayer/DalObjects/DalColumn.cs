using IntroSE.Kanban.Backend.DataAccessLayer.DalControllers;
using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DalObjects
{
    /// <summary>
    /// DalColumn class - extens DalObject and reprsent the Column class in business layer in data access layer
    /// </summary>
    public class DalColumn : DalObject<DalColumn>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// an empty constructor
        /// </summary>
        public DalColumn() : base(new ColumnDalController()) {
            _id = -1;
        }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="boardId"></param>
        /// <param name="name"></param>
        /// <param name="limit"></param>
        /// <param name="columnOrdinal"></param>
        public DalColumn(int id, int boardId, int columnOrdinal, string name, int limit) : base(new ColumnDalController())
        {
            _id = id;
            _boardId = boardId;
            _name = name;
            _limit = limit;
            _columnOrdinal = columnOrdinal;
        }
    
        public const String BoardIdColumnName = "Board_Id";
        public const String NameColumnName = "Name";
        public const String LimitColumnName = "TaskLimit";
        public const String ColumnOrdinalColumnName = "ColumnOrdinal";
        public List<DalTask> tasks { get; set; }
        private int _id;
        public int Id {
            get => _id;
            set
            {
                _id = value;
                if (!_controller.Insert(this))
                {
                    log.Error("Couldn't save column in database");
                    throw new Exception("Couldn't save column in database");
                }
            }
        }
        private int _boardId;
        public int BoardId {
            get => _boardId;
            set => _boardId = value;
        }
        private String _name;
        public String Name
        {
            get => _name;
            set {
                if (_id != -1)
                    if (!_controller.Update(BoardId, Id, NameColumnName, value))
                    {
                        log.Error("Couldn't update column name in database.");
                        throw new Exception("Couldn't update column name in database.");
                    }
                _name = value;
            }
        }
        private int _limit; 
        public  int Limit
        {
            get => _limit;
            set
            {
                if (_id != -1)
                    if (!_controller.Update(BoardId, Id, LimitColumnName, value))
                    {
                        log.Error("Couldn't update column limit in database.");
                        throw new Exception("Couldn't update column limit in database.");
                    }
                _limit = value;
            }
        }
        private int _columnOrdinal;
        public virtual int ColumnOrdinal
        {
            get => _columnOrdinal;
            set
            {
                if (_id != -1)
                    if (!_controller.Update(BoardId, Id, ColumnOrdinalColumnName, value))
                    {
                        log.Error("Couldn't update column ordinal in database.");
                        throw new Exception("Couldn't update column ordinal in database.");
                    }
                _columnOrdinal = value;
            }
        }

        /// <summary>
        /// Deletes the column from the database
        /// </summary>
        public override void Delete()
        {
            if (!_controller.Delete(this))
            {
                log.Error("Couldn't Delete column from database");
                throw new Exception("Couldn't Delete column.");
            }
        }

    }
}
