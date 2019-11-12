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
        private static FileConnector fileConnector = new FileConnector();
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

        public static void SendSignal(object action)
        {
            lock(Node.Nodes)
            {
                foreach(var client in Node.Nodes)
                {
                    var dataToSend = new Dictionary<string, object>();
                    dataToSend.Add("action", action);
                    var message = Newtonsoft.Json.JsonConvert.SerializeObject(dataToSend);
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    udpServer.Send(data, data.Length, client.IP, client.Port);
                }
            }
        }

        public static void SendSignal(object action, string data)
        {
            lock(Node.Nodes)
            {
                foreach(var client in Node.Nodes)
                {
                    var dataToSend = new Dictionary<string, object>();
                    dataToSend.Add("action", action);
                    dataToSend.Add("data", data);
                    var message = Newtonsoft.Json.JsonConvert.SerializeObject(dataToSend);
                    byte[] dataBytes = Encoding.UTF8.GetBytes(message);
                    udpServer.Send(dataBytes, dataBytes.Length, client.IP, client.Port);
                }
            }
            
        }

        public static void SendSignal(object action, string IP, int Port)
        {
            var dataToSend = new Dictionary<string, object>();
            dataToSend.Add("action", action);
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(dataToSend);
            byte[] data = Encoding.UTF8.GetBytes(message);
            udpServer.Send(data, data.Length, IP, Port);
        }

         public static void SendData(object data, string IP, int Port)
         {
            var dataToSend = new Dictionary<string, object>();
            dataToSend.Add("action", "SendToLeader");
            dataToSend.Add("data", data);
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(dataToSend);
            byte[] byteData = Encoding.UTF8.GetBytes(message);
            udpServer.Send(byteData, byteData.Length, IP, Port);
         }

        public void SendMyIP()
        {
            var dataToSend = new Dictionary<string, object>();
            dataToSend.Add("action","GetClients");
            var node = new NodeModel(){
                IP = Helper.GetLocalIPAddress(),
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
                        Node.KeepFollower = true;
                        break;
                    case ServerActions.GetClients:
                        var clients = data["clients"];
                        lock(Node.Nodes)
                        {
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
                            Node.Nodes.RemoveAll(x => x.IP == Helper.GetLocalIPAddress() && x.Port == ((IPEndPoint)udpServer.Client.LocalEndPoint).Port);
                        }
                        break;
                    case ServerActions.Election:
                        Console.WriteLine("ping");
                        Node.KeepFollower = true;
                        break;
                    case ServerActions.VoteForLeader:
                        lock(Node.KeepFollower)
                        {
                            if(Node.State == NodeState.Follower)
                            {
                                Node.KeepFollower = true;
                                SendSignal(ServerActions.Vote, RemoteIpEndPoint.Address.ToString(), RemoteIpEndPoint.Port);
                            }
                        }
                        
                        break;

                    case ServerActions.Vote:
                        Console.WriteLine("I VOTE FOR LEADER");
                        Node.VoteCount++;
                        break;
                    case ServerActions.KeepFollower:
                        Console.Write($"\rI GOT HEARTBEAT FROM LEADER {DateTime.Now.ToString()}");
                        Node.KeepFollower = true;
                        Node.State = NodeState.Follower;
                        var dataFromLeader = data["data"].ToString();
                        if(dataFromLeader != string.Empty) 
                        {
                            // JObject deserializedData = new JObject();
                            // deserializedData = JObject.Parse(dataFromLeader);
                            // var currentDataFromFile = FileConnector.GetAllMessagesAsText();
                            FileConnector.ClearFile();
                            fileConnector.WriteDataToSource(dataFromLeader);

                            // if (deserializedData["id"].ToString() != FileConnector.LastID)
                            // {
                            //     FileConnector.LastID = deserializedData["id"].ToString();
                            //     fileConnector.WriteDataToSource('\n'+dataFromLeader+'\n');
                            // }
                        }
                        break;
                    case ServerActions.GetFromLeader:
                        var output = new FileConnector().GetDataFromSource();
                        SendData(output, Helper.ServerIP, Helper.ServerPort);
                        break;
                    case ServerActions.SendToLeader:
                        var dataToWrite = data["data"].ToString();
                        JObject deserializedDataFromLeader = new JObject();
                        deserializedDataFromLeader = JObject.Parse(dataToWrite);
                        var currentData = FileConnector.GetAllMessagesAsText();

                        FileConnector.ClearFile();
                        new FileConnector().WriteDataToSource(currentData + deserializedDataFromLeader.ToString());
                        // FileConnector.LastID = deserializedDataFromLeader["id"].ToString();
                        // new FileConnector().WriteDataToSource('\n' + dataToWrite.ToString() + '\n');
                        break;
                    
                }
            }
        }
    }
}
