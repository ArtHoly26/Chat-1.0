using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ChatServer
{
    private static List<TcpClient> clients = new List<TcpClient>();
    private static TcpListener listener;
    private static readonly object lockObj = new object();

    static void Main(string[] args)
    {
        listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Сервер запущен...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            lock (lockObj)
            {
                clients.Add(client);
            }
            Console.WriteLine("Клиент подключен...");
            Thread thread = new Thread(HandleClient);
            thread.Start(client);
        }
    }

    private static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int byteCount;

        try
        {
            while ((byteCount = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                Console.WriteLine("Получено сообщение: " + message);
                Broadcast(message, client);
            }
        }
        finally
        {
            lock (lockObj)
            {
                clients.Remove(client);
            }
            client.Close();
        }
    }

    private static void Broadcast(string message, TcpClient excludeClient)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);

        lock (lockObj)
        {
            foreach (var client in clients)
            {
                if (client != excludeClient)
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }
}