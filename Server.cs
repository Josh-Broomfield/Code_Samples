using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerClient
{
    // This class listens on a separate thread and when it receives a
    // message will call the deligate with the contents of that message.
    // I don't know how to kill the listening thread. Thread.Abort doesn't work.
    public class Server
    {
        private TcpListener Listener;       // socket to be used for listening
        private Thread ListenThread;        // thread for listening and handling clients
        private Action<String> dispatch;    // callback function

        public int Port { get; set; }

        public bool IncludeIp { get; set; }

        public Server(Action<String> callback, bool includeIp = false)
        {
            dispatch = callback;
            IncludeIp = includeIp;
            Port = 7999;
        }

        public void StartListening()
        {
            ListenThread = new Thread(new ThreadStart(ListenMethod));

            ListenThread.Start();
        }

        private void ListenMethod()
        {
            try
            {
                Listener = new TcpListener(IPAddress.Any, Port);

                Listener.Start();

                TcpClient client;

                Thread clientHandleThread;

                while (true)
                {
                    client = Listener.AcceptTcpClient();

                    //new thread so the server can start listening as soon as possible
                    clientHandleThread = new Thread(new ParameterizedThreadStart(HandleClient));
                    clientHandleThread.Start(client);
                }
            }
            catch(Exception)
            {

            }

            Listener.Stop();
        }

        // Receives message and calls dispatch passing the message
        private void HandleClient(Object obj)
        {
            TcpClient client = (TcpClient)obj;

            NetworkStream ns = client.GetStream();
            byte[] byteMessage = new byte[4096];
            String message = "";
            String clientIP = "";

            try
            {
                ns.Read(byteMessage, 0, byteMessage.Length);

                message = Encoding.UTF8.GetString(byteMessage, 0, byteMessage.Length);
                message = message.Substring(0, message.LastIndexOf("<EOF>"));

                clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                if (IncludeIp)
                {
                    message += "|" + clientIP;
                }
            }
            catch (Exception)
            {

            }

            ns.Close();
            client.Close();

            try
            {
                dispatch(message);
            }
            catch (ObjectDisposedException)
            {

            }
        }

        public void StopListening()
        {
            Listener.Stop();
        }
    }
}
