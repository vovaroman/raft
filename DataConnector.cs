using System;
using System.Threading;
using c_Raft;
namespace c_Raft
{
    public abstract class DataConnector
    {
        public abstract string GetDataFromSource();
        public abstract void WriteDataToSource(string data);

    }
}
