using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bggparser
{
    class DbGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }

        public virtual ICollection<DbGameDate> DbGamesDates { get; set; }


    }
}
