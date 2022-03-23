using System;

namespace KongoCache.Client
{
    class Program
    { 
        static void Main(string[] args)
        {
            Console.WriteLine("Testing Rebase");
            // TCP server address
            string address = "127.0.0.1";
            address = "40.78.90.204";

            if (args.Length > 0)
                address = args[0];
            
            
            

            // TCP server port
            int port = 65332;
            port = 6359;

            if (args.Length > 1)
                port = int.Parse(args[1]);

            Console.WriteLine($"TCP server address: {address}");
            Console.WriteLine($"TCP server port: {port}");

            Console.WriteLine();

            // Create a new TCP chat client
            var client = new KongoClient(address, port);

            // Connect the client
            Console.Write("Kongo Client connecting...");
            client.ConnectAsync();
            Console.WriteLine("Kongo Client connected!");

            Console.WriteLine("Press Enter to stop the client or '!' to reconnect the client...");

            // Perform text input
            for (; ; )
            {
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                // Disconnect the client
                if (line == "!")
                {
                    Console.Write("Client disconnecting...");
                    client.DisconnectAsync();
                    Console.WriteLine("Done!");
                    continue;
                }
                
              //  Console.WriteLine("Command: " + line);
                
                // Send the entered text to the chat server
                client.Send(line);
            }

            // Disconnect the client
            Console.Write("Client disconnecting...");
            client.DisconnectAndStop();
            
            Console.WriteLine("Done!");
            Console.WriteLine("Done!123");
        }
    }
}
