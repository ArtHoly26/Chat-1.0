using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ChatClient
{
    private static TcpClient client;
    private static NetworkStream stream;

    static void Main(string[] args)
    {
        try
        {
            client = new TcpClient("127.0.0.1", 5000);
            stream = client.GetStream();
            Console.WriteLine("Подключен к серверу...");

            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();

            SendMessages();
        }
        finally
        {
            client.Close();
        }
    }

    private static void ReceiveMessages()
    {
        byte[] buffer = new byte[1024];
        int byteCount;

        try
        {
            while ((byteCount = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                Console.WriteLine("Сообщение от сервера: " + message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при получении сообщений: " + e.Message);
        }
    }

    private static void SendMessages()
    {
        Console.WriteLine("Введите сообщение:");

        try
        {
            while (true)
            {
                string message = Console.ReadLine();
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при отправке сообщения: " + e.Message);
        }
    }
}
