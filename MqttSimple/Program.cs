using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttSimple
{
    class Program
    {
        static MqttClient mqttClient;
        static string clientId;
        static string ip;
        static string[] topic;
        static Random random;
        public class MqttMessage
        {
            public string message;
            public string datetime;
            public MqttMessage()
            {
                this.datetime =DateTime.Now.ToString();
            }
        }
        static void connect()
        {
            try
            {
                ip = "192.168.20.128";
                mqttClient = new MqttClient(IPAddress.Parse(ip), 1883, false, null, null, MqttSslProtocols.TLSv1_2);
                mqttClient.ConnectionClosed += MqttClient_ConnectionClosed;
                clientId = "publisher";
                random = new Random();
                mqttClient.Connect(clientId,"furkan","101294",false,(ushort)60);
                if (mqttClient.IsConnected)
                {
                    while (mqttClient.IsConnected)
                    {
                        var value = random.Next().ToString();
                        MqttMessage mqttMessage = new MqttMessage();
                        mqttMessage.message = value;
                        value = JsonConvert.SerializeObject(mqttMessage);
                        var message = mqttClient.Publish("temprature", Encoding.UTF8.GetBytes(value), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                        Console.WriteLine("send message: {0}", value);
                        Console.WriteLine(message);
                        Thread.Sleep(1500);
                    }
                }
                else
                {
                    Console.WriteLine("Connection error..Please check username and password");
                    Thread.Sleep(1000);
                    connect();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                connect();
            }
        }

        private static void MqttClient_ConnectionClosed(object sender, EventArgs e)
        {
            Console.WriteLine("no connection!");
            connect();
        }

        static void Main(string[] args)
        {
            connect();

        }
        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("topic : " + e.Topic + " Qos lvl : " + e.QosLevel + " Message : " + Encoding.UTF8.GetString(e.Message));
        }

    }
}
