using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("UnitTest.BoardPackage")]

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// KanbanException class - extends SystemException and helps to seperate between exception the we can predict from others
    /// </summary>
    public class KanbanException : SystemException
    {
        public KanbanException(){  }
        public KanbanException(string m)
            : base(String.Format("kanban massage: {0}", m)){  }
    }
}
