using System;

namespace c_Raft
{
    public enum ServerActions
    {
        Ping,
        GetClients,
        
        Election,

        VoteForLeader
    }
}
