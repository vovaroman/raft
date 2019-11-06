using System;
using System.IO;
using System.Text;
using System.Threading;
using c_Raft;
namespace c_Raft
{
    public class FileConnector : DataConnector
    {
        private string path = @".\datasource.txt";

        public override string GetDataFromSource()
        {
            string readText = File.ReadAllText(path);
            return readText;
        }

        public override void WriteDataToSource(string data)
        {
            File.AppendAllText(path, data, Encoding.UTF8);
        }
    }
}
