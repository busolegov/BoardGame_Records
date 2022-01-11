using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bggparser
{
    class BoardGameContex : DbContext
    {
        public BoardGameContex() : base ("DbConnectionString")
        {

        }

        public DbSet<DbGame> DbGames { get; set; }
        public DbSet<DbGameDate> DbGamesDates { get; set; }
    }


}
