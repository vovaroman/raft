using System;
using System.Collections.Generic;
using System.Threading;
using c_Raft;
namespace c_Raft
{
    public class Node
    {
        public static NodeState State = NodeState.Follower;

        public static int VoteCount = 0;
        public static bool KeepFollower = true;
        public static List<NodeModel> Nodes = new List<NodeModel>();
        private int _electionTime = 5000;//new Random().Next(150,301) * 10;
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
                if(State == NodeState.Leader){
                    Console.WriteLine("I AM LEADER");
                    new System.Threading.ManualResetEvent(false).WaitOne(1000);
                    UDPServer.SendSignal(ServerActions.KeepFollower);
                    continue;
                }
                new System.Threading.ManualResetEvent(false).WaitOne(ElectionTime);
                switch(State)
                {
                    case NodeState.Follower:

                        if(!Node.KeepFollower)
                        {
                            Console.WriteLine("I WANT TO BE LEADER");
                            UDPServer.SendSignal(ServerActions.VoteForLeader);
                            VoteCount++;
                            State = NodeState.Leader;
                            continue;
                        }
                        Console.WriteLine("IM Follower");

                    break;
                    case NodeState.Candidate:
                    break;
                    case NodeState.Leader:
                    break;
                }
                KeepFollower = false;

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
