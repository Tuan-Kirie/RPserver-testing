using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Linq;
namespace RPserverTesting
{
    public class Adapter
    {
        public Socket client;
        private const string HOST = "127.0.0.1";
        private const int PORT = 22000;
        private IPEndPoint _endPoint;

        public Adapter()
        {
            _endPoint = new IPEndPoint(IPAddress.Parse(HOST), PORT);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void ConnectTeacher()
        {
            client.Connect(_endPoint);
        }

        private static byte[] NullTypeseArray(byte[] data)
        {
            byte[] nullTypedData = new byte[data.Length + 1];
            data.CopyTo(nullTypedData, 0);
            nullTypedData[nullTypedData.Length - 1] = 00;
            return nullTypedData;
        }
        
        public byte[] MessageEncoder(object message)
        {
            string messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            byte[] data = Encoding.ASCII.GetBytes(messageJson);
            byte[] nullTypedData = NullTypeseArray(data);
            return nullTypedData;
        }
        
        public StringBuilder MessageDecoder(byte[] data, int bytes)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Encoding.ASCII.GetString(data, 0, bytes));
            return builder;
        }
        
        private string[] separMessage(string message)
        {
            string[] values = message.Split('\n');
            return values;
        }

        public static JObject ParseJObject(string str)
        {
            try
            {
                return JObject.Parse(str.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public JObject AnalyzResponse(StringBuilder message)
        {
            string mes = message.ToString();
            string newmessage = mes.Replace("\0", String.Empty);
            if (String.IsNullOrEmpty(newmessage))
            {
                return null;
            }
        
            string jsonmessage = newmessage.Trim(new char[] {'\r', ' '});
            JObject jObject = ParseJObject(jsonmessage);
            Console.WriteLine(jObject);
            return jObject;
        }
        
        public JObject RecieveMessage()
        {
            int bytes = 0;
            byte[] receivedData = new byte[256];
            bytes = client.Receive(receivedData, receivedData.Length, 0);
            JObject obj = AnalyzResponse(MessageDecoder(receivedData, bytes));
            return obj;
        }

        public void checkPingPong(JObject message)
        {
            if (message?["ping"]?.ToString() == "0")
            {
                IdVObject obj = new IdVObject();
                obj.id = "RP5-00-(000)";
                obj.version = "5.6.2.83";
                SendMessageToServer(obj);
            }
        }
        
        //Ождиание определенного сообщения
        public JObject RecieveAwaytingMessage(string property, string validator)
        {
            JObject val = null;
            int timer = 0;
            do
            {
               val = RecieveMessage();
               checkPingPong(val);
               timer++;
            } while (val?[property]?.ToString() != validator || timer == 1000);
            return val;
        }
        
        //Главный метод для отправки сообщении серверу 
        public void SendMessageToServer(Object message)
        {
            string messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            byte[] data = Encoding.ASCII.GetBytes(messageJson);
            byte[] nullTypedData = NullTypeseArray(data);
            client.Send(nullTypedData);
        }
        
    }
}