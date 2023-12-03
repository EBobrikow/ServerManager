using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



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
        const string ServerManagerUrl = "ServerManagerUrl";
        const string Port = "Port";
        const string UEServerPath = "UEServerPath";

        static void Main(string[] args)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Config.json";
           
           

            string adress = "ws://127.0.0.1";
            string Ip = "127.0.0.1";
            int defaultPort = 3030;
            string UEServerLocation = "F:\\SkillUpProjects\\RunCast\\win\\WindowsServer\\RunCastServer.exe";

            string readRes = "";
            if (File.Exists(path))
            {
                readRes = File.ReadAllText(path);
                JObject jMsg = JObject.Parse(readRes);
                if (jMsg.TryGetValue(ServerManagerUrl, out var servUrl))
                {
                    Ip = servUrl.ToString();
                    adress = "ws://" + Ip;
                }
                if (jMsg.TryGetValue(Port, out var port))
                {
                    defaultPort = Int32.Parse(port.ToString());
                }
                if (jMsg.TryGetValue(UEServerPath, out var uepath))
                {
                    UEServerLocation = uepath.ToString();
                }
            }

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
                wt.UEServerExecuteblePath = UEServerLocation;
                Thread thrd = new Thread(new ThreadStart(wt.Run));
                thrd.Start();

            });

            Console.WriteLine("Server started");
            Console.ReadKey();
            //server.Stop();
        }
    }
}
