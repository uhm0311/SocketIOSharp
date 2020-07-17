using EngineIOSharp.Common.Enum;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using SocketIOSharp.Common;
using System;

namespace SocketIOSharp.Example.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketIOClient client = new SocketIOClient(new SocketIOClientOption(EngineIOScheme.http, "localhost", 9001));
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
            client.On(SocketIOEvent.CONNECTION, () =>
            {
                Console.WriteLine("Connected!");
            });

            client.On(SocketIOEvent.DISCONNECT, () =>
            {
                Console.WriteLine();
                Console.WriteLine("Disconnected!");
            });

            client.On("echo", (Data) =>
            {
                Console.WriteLine("Echo : " + (Data[0].Type == JTokenType.Bytes ? BitConverter.ToString(Data[0].ToObject<byte[]>()) : Data[0]));
            });

            client.On("echo array", (Data) =>
            {
                Console.WriteLine("Echo1 : " + Data[0]);
                Console.WriteLine("Echo2 : " + Data[1]);
            });
        }
    }
}
