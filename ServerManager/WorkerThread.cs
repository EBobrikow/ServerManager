using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using static ServerManager.WorkerThread;

namespace ServerManager
{
    public class WorkerThread
    {
      
        public IWebSocketConnection socket;
        bool done;
        public string adress;
        public string Ip;
        public int forbidenPort;
        public Storage storageRef;

        public void Run()
        {
            socket.OnOpen = () => OnOpen();
            socket.OnClose = () => OnClose();
            socket.OnMessage = message => OnMessage(message);
            socket.OnError = message => OnError(message);


            done = false;
            while (!done) 
            {
                //Console.WriteLine("Thread tick");
                Thread.Sleep(100);
            }

            Console.WriteLine("Socket closed");
            socket.Close();
        }

        protected void OnOpen()
        {
            Console.WriteLine("Open connection \n");
            //base.OnOpen();
            SendResponce("Connected");
        }

        protected void OnClose()
        {
            storageRef.Clients.Remove(socket);
            
            Console.WriteLine("Connection closed \n");
            done = true;
            //base.OnClose(e);
        }

        protected void OnError(System.Exception e)
        {
            // base.OnError(e);
            Console.WriteLine("Error: \n" + e.Message);
        }

        protected void OnMessage(string msg)
        {
            //base.OnMessage(e);
            ProcessMessage(msg);
        }


        private void ProcessMessage(string msg)
        {
            Console.WriteLine("Message: \n" + msg);

            JObject jMsg = JObject.Parse(msg);
            if (jMsg.TryGetValue("Action", out var message))
            {
                if (message.ToString() == "ReqestServer")
                {
                    string newServerName = "";
                    if (jMsg.TryGetValue("ServerName", out var servname))
                    {
                        newServerName = servname.ToString();
                    }
                    RunNewServer(newServerName);
                    return;
                }
                else if (message.ToString() == "GetServersList")
                {
                    ReturnServersList();
                }
              
            };
             
        }

        private void ReturnServersList()
        {
            List<ServerInfo> serverList = new List<ServerInfo>();
            for (int i = 0; i < storageRef.processList.Count; i++)
            {
                serverList.Add(storageRef.processList[i].serverInfo);
            }
            string json = JsonConvert.SerializeObject(serverList);
            SendResponce(json);
            return;
        }

        private void RunNewServer(string serverName)
        {
            ServerProcessInfo servInfo = StartNewServer(serverName);
            string json = JsonConvert.SerializeObject(servInfo.serverInfo);
            SendResponce(json);
            return;
        }

        private void SendResponce(string msg)
        {
            Console.WriteLine("Responce to client: \n" + msg);
            socket.Send(msg);

        }

        private int GetAvailablePort(int startingPort)
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();

           
            var tcpConnectionPorts = properties.GetActiveTcpConnections()
                                .Where(n => n.LocalEndPoint.Port >= startingPort)
                                .Select(n => n.LocalEndPoint.Port);

            
            var tcpListenerPorts = properties.GetActiveTcpListeners()
                                .Where(n => n.Port >= startingPort)
                                .Select(n => n.Port);

           
            var udpListenerPorts = properties.GetActiveUdpListeners()
                                .Where(n => n.Port >= startingPort)
                                .Select(n => n.Port);

            var port = Enumerable.Range(startingPort, ushort.MaxValue)
                .Where(i => !tcpConnectionPorts.Contains(i))
                .Where(i => !tcpListenerPorts.Contains(i))
                .Where(i => !udpListenerPorts.Contains(i))
                .FirstOrDefault();

            return port;
        }

        private ServerProcessInfo StartNewServer(string servName)
        {
            ServerProcessInfo resInfo = new ServerProcessInfo();

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "F:\\SkillUpProjects\\RunCast\\win\\WindowsServer\\RunCastServer.exe";

            bool properPort = false;
            int port = 0;
            while (!properPort) 
            {
                port = GetAvailablePort(1001);
                if (port != forbidenPort)
                {
                    if (storageRef.processList.Count == 0)
                    {
                        properPort = true;
                        break;
                    }
                    for (int i = 0; i < storageRef.processList.Count; i++)
                    {
                        if (storageRef.processList[i].serverInfo.port == port)
                        {
                            break;
                        }

                        if (i == storageRef.processList.Count - 1)
                        {
                            properPort = true;
                        }
                    }
                }
            }
           

            startInfo.Arguments = "-log -Port=" + port;
            process.StartInfo = startInfo;
            process.Start();

            resInfo.process = process;
            resInfo.processStatus = ProcessStatus.Running;
            resInfo.serverInfo.port = port;
            resInfo.serverInfo.host = Ip;
            resInfo.serverInfo.serverName = servName;
            resInfo.serverInfo.Id = ++storageRef.serverId;
            resInfo.serverInfo.matchStatus = MatchStatus.Entrance;
            storageRef.processList.Add(resInfo);

            return resInfo;
        }
    }


}
