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
            //                    数据库名     ip       端口  数据库用户名和密码
            if (!DbManager.Connect("game", "127.0.0.1", 3306, "root", "123456"))
            {
                return;
            }

            NetManager.StartLoop(8888);
        }
    }
}
