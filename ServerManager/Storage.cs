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

        public ServerInfo GetServerInfoByID(int id)
        {
            ServerInfo local = new ServerInfo();
            if (processList.Count > 0)
            {
                for (int i = 0; i < processList.Count; i++)
                {
                    if (processList[i].serverInfo.Id == id)
                    {
                        local = processList[i].serverInfo;
                        break;
                    }
                }
            }
            return local;
        }

        public void CloseServer(int Id)
        {
            int processIndx = -1;
            if (processList.Count > 0)
            {
                for (int i = 0; i < processList.Count; i++)
                {
                    if (processList[i].serverInfo.Id == Id)
                    {
                        ServerProcessInfo localProces = processList[i];
                        processIndx = i;
                        break;
                    }
                }
            }

            if (processIndx != -1)
            {
                processList[processIndx].process.Kill();
                processList[processIndx].process.Close();
               
                processList.RemoveAt(processIndx);
                
            }
        }

        public ServerInfo UpdateServerInfo(ServerInfo info)
        {
            ServerInfo local = new ServerInfo();
            if (processList.Count > 0)
            {
                for (int i = 0; i < processList.Count; i++)
                {
                    if (processList[i].serverInfo.Id == info.Id)
                    {
                        ServerProcessInfo localProces = processList[i];
                        local = processList[i].serverInfo;
                        local.matchStatus = info.matchStatus;
                        local.mapName = info.mapName;
                        local.currentPlayers = info.currentPlayers;
                        local.maxPlayers = info.maxPlayers;
                        local.matchType = info.matchType;
                        localProces.serverInfo = local;
                        processList.Remove(processList[i]);
                        processList.Add(localProces);
                        return local;
                    }
                }
            }

            return local;
        }

        public ServerInfo GetServerInfoByPort(int port) 
        {
            if (processList.Count > 0)
            {
                for (int i = 0; i < processList.Count; i++)
                {
                    if (processList[i].serverInfo.port == port)
                    {
                        return processList[i].serverInfo;
                    }
                }
            }
           
            return new ServerInfo(); 
        }

        public List<ServerInfo> GetServersList()
        {
            List<ServerInfo> serverList = new List<ServerInfo>();
            for (int i = 0; i < processList.Count; i++)
            {
                serverList.Add(processList[i].serverInfo);
            }

            return serverList;
        }

        public void ConsoleWrite(string msg)
        { 
            Console.WriteLine(msg);
        }

    }
}
