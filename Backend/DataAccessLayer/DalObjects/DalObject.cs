using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DalObjects
{
    /// <summary>
    /// DalObject class is an abstract class holds the constant field of Id column nae in the database
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DalObject<T> where T : DalObject<T>
    {
        protected DalController<T> _controller;
        public const string IdColumnName = "Id";
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dalController"></param>
        protected DalObject (DalController<T> dalController){
            _controller = dalController;
        }
         
        public abstract void Delete();

    }
}
