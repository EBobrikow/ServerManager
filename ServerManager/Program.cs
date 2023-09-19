using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;



namespace ServerManager
{

    public enum ProcessStatus
    {
        Running = 0,
        Shutdown = 1
    }

    public enum MatchStatus
    {
        Entrance = 0,
        Lobby = 1,
        PickStage = 2,
        Runing = 3,
        Finished = 4
    }

    public struct ServerProcessInfo
    {
        public System.Diagnostics.Process process;
        public ServerInfo serverInfo;
        public ProcessStatus processStatus;
    }

    public struct ServerInfo
    {
        public int Id;
        public string serverName;
        public int port;
        public string host;
        public MatchStatus matchStatus;
        public string matchType;
        public string mapName;
        public int currentPlayers;
        public int maxPlayers;

    }

    internal class Program
    {
        static void Main(string[] args)
        {
            string adress = "ws://127.0.0.1";
            string Ip = "127.0.0.1";
            int defaultPort = 3030;

            Storage storage = new Storage();
            WebSocketServer server = new WebSocketServer(adress + ":" + defaultPort);
            


            server.Start(socket =>
            {
                storage.Clients.Add(socket);
                WorkerThread wt = new WorkerThread();
                wt.socket = socket;
                wt.adress = adress;
                wt.forbidenPort = defaultPort;
                wt.Ip = Ip;
                wt.storageRef = storage;
                Thread thrd = new Thread(new ThreadStart(wt.Run));
                thrd.Start();

            });

            Console.WriteLine("Server started");
            Console.ReadKey();
            //server.Stop();
        }
    }
}
