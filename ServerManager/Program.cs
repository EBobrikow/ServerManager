using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;



namespace ServerManager
{

    //public class Manager : WebSocketBehavior
    //{
    //    protected override void OnOpen()
    //    {
    //        Console.WriteLine("Open connection \n");
    //        //base.OnOpen();
    //        SendResponce("Connected");
    //    }

    //    protected override void OnClose(CloseEventArgs e)
    //    {
    //        Console.WriteLine("Connection closed \n");
    //        //base.OnClose(e);
    //    }

    //    protected override void OnError(ErrorEventArgs e)
    //    {
    //       // base.OnError(e);
    //        Console.WriteLine("Error: \n" +  e.Message);
    //    }

    //    protected override void OnMessage(MessageEventArgs e)
    //    {
    //        //base.OnMessage(e);
    //        ProcessMessage(e.Data);
    //    }


    //    private void ProcessMessage(string msg)
    //    {
    //        Console.WriteLine("Message: \n" + msg);
    //        SendResponce("Responde !!!!");
    //    }

    //    private void SendResponce(string msg)
    //    {
    //        Console.WriteLine("Responce to client: \n" + msg);
    //        Send(msg);

    //    }
    //}

    internal class Program
    {
        static void Main(string[] args)
        {
            string adress = "ws://127.0.0.1";
            int defaultPort = 3030;
            WebSocketServer server = new WebSocketServer(adress + ":" + defaultPort);



            server.Start(socket =>
            {
                WorkerThread wt = new WorkerThread();
                wt.socket = socket;
                wt.adress = adress;
                wt.forbidenPort = defaultPort;
                Thread thrd = new Thread(new ThreadStart(wt.Run));
                thrd.Start();

            });

            Console.WriteLine("Server started");
            Console.ReadKey();
            //server.Stop();
        }
    }
}
