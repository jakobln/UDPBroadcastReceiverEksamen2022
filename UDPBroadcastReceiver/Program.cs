using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

namespace UDPBroadcastReceiver
{
    class Program
    {
        private static string URL = "http://localhost:14868/api/Winds";
        private const int Port = 7000;
        static void Main()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, Port);
            using (UdpClient socket = new UdpClient(ipEndPoint))
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(0, 0);

                using (HttpClient client = new HttpClient())
                {
                    while (true)
                    {
                        Console.WriteLine("Waiting for broadcast {0}", socket.Client.LocalEndPoint);
                        byte[] datagramReceived = socket.Receive(ref remoteEndPoint);
    
                        string message = Encoding.ASCII.GetString(datagramReceived, 0, datagramReceived.Length);
                        Console.WriteLine("Receives {0} bytes from {1} port {2} message {3}", datagramReceived.Length,
                            remoteEndPoint.Address, remoteEndPoint.Port, message);
                        Parse(message);

                        byte[] data = socket.Receive(ref remoteEndPoint);
                        string recieved = Encoding.UTF8.GetString(data);
                        Console.WriteLine("Server receieved: " + recieved + " From " + remoteEndPoint.Address);

                        HttpContent content = new StringContent(recieved, Encoding.UTF8, "application/json");
                        client.PostAsync(URL, content);
                    }
                }
            }
        }

        private static void Parse(string response)
        {
            string[] parts = response.Split('|');
            foreach (string part in parts)
            {
                Console.WriteLine(part);
            }
        }
    }
}
