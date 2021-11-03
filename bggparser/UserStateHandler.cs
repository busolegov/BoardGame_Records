using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bggparser
{
    public delegate void UserStateHandler(object sender, UserEventArgs e);
    public class UserEventArgs 
    {
        public string Message { get; private set; }
        public UserEventArgs(string mes)
        {
            Message = mes;
        }
    }
}
