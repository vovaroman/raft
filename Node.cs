using System;
using System.Collections.Generic;
using System.Threading;
using c_Raft;
namespace c_Raft
{
    public class Node
    {
        public NodeState State = NodeState.Follower;

        public static bool Ping = true;
        public static List<NodeModel> Nodes = new List<NodeModel>();
        private int _electionTime = new Random().Next(150,301) * 10;
        public int ElectionTime
        {
            get{
                return _electionTime;
            }
        }

        public void LeaderElection()
        {
            Console.WriteLine(ElectionTime);
            while(true)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(ElectionTime);
                switch(State)
                {
                    case NodeState.Follower:
                        
                    break;
                    case NodeState.Candidate:
                    break;
                    case NodeState.Leader:
                    break;
                }

            }
            // while(true)
            // {
            //     if(Node.Nodes.Count != 0)
            //     {
            //
            //         if(Node.Ping)
            //         {
            //             foreach(var node in Node.Nodes)
            //             {
            //                 UDPServer.SendElection();
            //                 Console.WriteLine("IM LEADER");
            //             }
            //         }
            //     }
            //     Node.Ping = true;

            // }

        }


        public void Init()
        {
            var udpServer = new UDPServer();
            udpServer.SendMyIP();
            var listenUDP = new Thread(() => udpServer.ListenUDP());
            var leaderElection = new Thread(() => LeaderElection());
            listenUDP.Start();
            leaderElection.Start();

        }

    }
}
