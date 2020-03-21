using SocketIOSharp.Client;
using SocketIOSharp.Common;
using System;

namespace SocketIOSharp.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketIOClient client = new SocketIOClient(SocketIOClient.Scheme.ws, "127.0.0.1", 9001);
            InitEventHandlers(client);

            client.Connect();
            Console.WriteLine("Input /exit to close connection.");

            string line;
            while (!(line = Console.ReadLine()).Equals("/exit"))
            {
                client.Emit("input", line);
                client.Emit("input array", line, line);
            }

            client.Close();

            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }

        static void InitEventHandlers(SocketIOClient client)
        {
            client.On(SocketIOEvent.CONNECTION, (Data) =>
            {
                Console.WriteLine("Connected!");
            });

            client.On(SocketIOEvent.DISCONNECT, (Data) =>
            {
                Console.WriteLine();
                Console.WriteLine("Disconnected!");
            });

            client.On("echo", (Data) =>
            {
                Console.WriteLine("Echo : " + Data[0]);
            });

            client.On("echo array", (Data) =>
            {
                Console.WriteLine("Echo1 : " + Data[0]);
                Console.WriteLine("Echo2 : " + Data[1]);
            });
        }
    }
}
