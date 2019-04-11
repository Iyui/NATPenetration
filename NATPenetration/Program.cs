using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace NATPenetration
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 555;
            IPEndPoint ipe = new IPEndPoint(IPAddress.Any, port);
            Socket sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sSocket.Bind(ipe);
            sSocket.Listen(100);
            Console.WriteLine("监听已经打开，请等待");

            while (true)
            {
                Socket serverSocket1 = sSocket.Accept();
                Console.WriteLine("连接已经建立");
                string recStr = "";
                byte[] recByte = new byte[4096];
                int bytes = serverSocket1.Receive(recByte);
                IPEndPoint ep1 = (IPEndPoint)serverSocket1.RemoteEndPoint;
                Console.WriteLine(" from {0}", ep1.ToString());
                recStr = Encoding.ASCII.GetString(recByte, 0, bytes);
                Console.WriteLine("客户端1:{0}", recStr);

                Socket serverSocket2 = sSocket.Accept();
                bytes = serverSocket2.Receive(recByte);
                IPEndPoint ep2 = (IPEndPoint)serverSocket2.RemoteEndPoint;
                Console.WriteLine(" from {0}", ep2.ToString());
                recStr = Encoding.ASCII.GetString(recByte, 0, bytes);
                Console.WriteLine("客户端2:{0}", recStr);


                byte[] sendByte = Encoding.ASCII.GetBytes(ep1.ToString() + ":" + ep2.ToString());
                serverSocket1.Send(sendByte, sendByte.Length, 0);

                sendByte = Encoding.ASCII.GetBytes(ep2.ToString() + ":" + ep1.ToString());
                serverSocket2.Send(sendByte, sendByte.Length, 0);

                serverSocket1.Close();
                serverSocket2.Close();
            }

        }
    }
}
