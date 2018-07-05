using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerClient
{
    public static class Client
    {
        //synchronously sends data
        public static void SendMessage(String message, String ip, int port = 8880)
        {
            message += "<EOF>";
            try
            {
                TcpClient client = new TcpClient(ip, port);
                NetworkStream ns = client.GetStream();

                byte[] byteMessage;// = new byte[4096];

                byteMessage = Encoding.UTF8.GetBytes(message);
                ns.Write(byteMessage, 0, byteMessage.Length);

                ns.Close();
                client.Close();
            }
            catch (Exception)
            {

            }
        }

        //used as a new thread
        private static void SendMessage(Object obj)
        {
            try
            {
                SingleSubscriberInfoWithMessage s = (SingleSubscriberInfoWithMessage)obj;
                Client.SendMessage(s.Message, s.Ip, s.Port);
            }
            catch(Exception)
            {

            }
        }

        public static void SendToMultiple(SubscriberInfo info)
        {
            SingleSubscriberInfoWithMessage threadMessage;
            Thread t;

            // set message once outside the loop then change ip and port for each client
            foreach (SingleSubscriberInfo s in info.SubscriberList)
            {
                t = new Thread(new ParameterizedThreadStart(SendMessage));

                //this has to be new because of memory sharing
                threadMessage = new SingleSubscriberInfoWithMessage(s.Ip, s.Port, info.Message);

                t.Start(threadMessage);
            }
        }

        //should probably delete this
        public static void SendToMultiple(String message, List<String> ips)
        {
            foreach (String ip in ips)
            {
                //start new thread instead
                SendMessage(message, ip);
            }
        }
    }
}
