using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend
{
    public class KanbanException: Exception
    {
       
            public KanbanException()
        {
           
        }
        public KanbanException(string message)
            : base(message)
        {
        }
        public KanbanException(string message,Exception inner)
            : base(message, inner)
        { }


    }
}
