using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!DbManager.Connect("game", "127.0.0.1", 3306, "root", ""))
            {
                return;
            }

            NetManager.StartLoop(8888);
        }
    }
}
