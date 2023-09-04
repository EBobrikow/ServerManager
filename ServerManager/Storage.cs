using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerManager
{
    public class Storage
    {
        public List<ServerProcessInfo> processList = new List<ServerProcessInfo>();
        public int serverId = 0;
        public List<IWebSocketConnection> Clients = new List<IWebSocketConnection>();
    }
}
