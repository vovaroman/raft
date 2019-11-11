using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using c_Raft;
using Newtonsoft.Json.Linq;

namespace c_Raft
{
    public class FileConnector : DataConnector
    {
        private static string path = @".\datasource.txt";

        public static  string LastID = GetLastId();

        private static string GetLastId() {
            if (File.Exists(path)) {
                var message = File.ReadLines(path).Reverse().Take(4).ToString();
                if (message == String.Empty) { return String.Empty; }
                JObject data = new JObject();
                data = JObject.Parse(message);
                return data["id"].ToString();
            }
           return String.Empty;
        } 
        public override string GetDataFromSource()
        {
            if (File.Exists(path)) {
                string readText = File.ReadAllText(path);
                return readText;
            }
            return string.Empty;
        }

        public override void WriteDataToSource(string data)
        {
            File.AppendAllText(path, data, Encoding.UTF8);
        }

    }
}
