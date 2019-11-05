using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using c_Raft;

namespace c_Raft
{
    
    public class UDPServer
    {
        private static UdpClient udpServer = new UdpClient(Helper.UdpPort);

        public static void SendElection()
        {
            foreach(var client in Node.Nodes)
            {
                var dataToSend = new Dictionary<string, object>();
                dataToSend.Add("action","Election");
                var message = Newtonsoft.Json.JsonConvert.SerializeObject(dataToSend);
                byte[] data = Encoding.UTF8.GetBytes(message);

                udpServer.Send(data, data.Length, client.IP, client.Port);

            }

        }

        public void SendMyIP()
        {
            var dataToSend = new Dictionary<string, object>();
            dataToSend.Add("action","GetClients");
            var node = new NodeModel(){
                IP = Helper.ExternalIp(),
                Port = ((IPEndPoint)udpServer.Client.LocalEndPoint).Port
            };
            dataToSend.Add("clients",node);
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(dataToSend);
            
            byte[] data = Encoding.UTF8.GetBytes(message);
            udpServer.Send(data, data.Length, Helper.ServerIP, Helper.ServerPort);
        }
        public void ListenUDP()
        {
            while (true)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(
                    IPAddress.Any,
                    Helper.UdpPort
                    );
                Byte[] receiveBytes = udpServer.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                JObject data = new JObject();
                data = JObject.Parse(returnData);
                // Console.WriteLine(data);
                Enum.TryParse(data["action"].ToString(), out ServerActions action);

                switch(action)
                {
                    case ServerActions.Ping:
                        Node.Ping = true;
                        break;
                    case ServerActions.GetClients:
                        var clients = data["clients"];
                        foreach(var client in clients)
                        {
                            if(Node.Nodes.FirstOrDefault(x => x.IP == client["IP"].ToString() && x.Port == int.Parse(client["Port"].ToString())) == null)
                            Node.Nodes.Add(
                                new NodeModel(){
                                    IP = client["IP"].ToString(),
                                    Port = int.Parse(client["Port"].ToString())
                                }
                            );
                        }
                        Node.Nodes.RemoveAll(x => x.IP == Helper.ExternalIp() && x.Port == ((IPEndPoint)udpServer.Client.LocalEndPoint).Port
                        );
                        break;
                    case ServerActions.Election:
                        Console.WriteLine("ping");
                        Node.Ping = false;
                        break;
                }
            }
        }
    }
}
