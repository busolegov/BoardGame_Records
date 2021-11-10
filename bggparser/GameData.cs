using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bggparser
{
    [Serializable]
    public class GameData
    {
        public GameData()
        {

        }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }


    }
}
