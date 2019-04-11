using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace NATPenetration.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = "115.21.X.X";//服务端IP地址
            int port = 555;
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //设置端口可复用
            clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //连接服务端
            clientSocket.Connect(host, port);
            Console.WriteLine("Connect：" + host + "  " + port);

            string data = "hello,Server!";
            clientSocket.Send(Encoding.ASCII.GetBytes(data));
            Console.WriteLine("Send：" + data);
            byte[] recBytes = new byte[100];
            //获取到双方的ip及端口号
            int bytes = clientSocket.Receive(recBytes, recBytes.Length, 0);
            string result = Encoding.ASCII.GetString(recBytes, 0, bytes);
            Console.WriteLine("Recv：" + result);
            clientSocket.Close();

            string[] ips = result.Split(':');
            int myPort = Convert.ToInt32(ips[1]);
            string otherIp = ips[2];
            int otherPort = Convert.ToInt32(ips[3]);


            Socket mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mySocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //绑定到之前连通过的端口号
            IPEndPoint ipe = new IPEndPoint(IPAddress.Any, Convert.ToInt32(myPort));
            mySocket.Bind(ipe);
            //尝试5次连接
            for (int j = 0; j < 5; j++)
            {
                try
                {
                    mySocket.Connect(otherIp, otherPort);
                    Console.WriteLine("Connect：成功{0},{1}", otherIp, otherPort);
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("Connect：失败");
                    // otherPort++;//如果是对称NAT，则有可能客户端的端口号已经改变，正常有规律的应该是顺序加1，可以尝试+1再试（我使用手机热点连接的时候端口号就变成+1的了）除非是碰到随机端口，那就不行了。
                }

            }
            while (true)
            {
                mySocket.Send(Encoding.ASCII.GetBytes("hello,the other client!"));

                byte[] recv = new byte[4096];
                int len = mySocket.Receive(recv, recv.Length, 0);
                result = Encoding.ASCII.GetString(recv, 0, len);
                Console.WriteLine("recv :" + result);

                Thread.Sleep(1000);
            }
        }
    }
}
