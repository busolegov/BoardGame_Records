using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bggparser
{
    class DbGameDate
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int DbGameId { get; set; }

        public DbGame DbGame { get; set; }
    }
}
