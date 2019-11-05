using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public static class Helper
{
    public static int UdpPort = 0;//GetAvailablePort(1000);

    public static int ServerPort = 616;
    public static string ServerIP = "109.185.157.125";

    public static string GetLocalIPAddress()
    {
        string localIP;
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint.Address.ToString();
        }
        return localIP;
        // var host = Dns.GetHostEntry(Dns.GetHostName());
        // foreach (var ip in host.AddressList)
        // {
        //     if (ip.AddressFamily == AddressFamily.InterNetwork)
        //     {
        //         return ip.ToString();
        //     }
        // }
        // throw new Exception("No network adapters with an IPv4 address in the system!");
    }
    public static string ExternalIp()
    {
        string externalip = new WebClient().DownloadString("http://icanhazip.com");            
        return externalip;
        // var addresses = Dns.GetHostEntry((Dns.GetHostName()))
        //         .AddressList
        //         .Where(x => x.AddressFamily == AddressFamily.InterNetwork)
        //         .Select(x => x.ToString())
        //         .ToArray();

        // // foreach(var ip in addresses)
        // // {
        // //     Console.WriteLine(ip);
        // // }
        // if(addresses.Length > 1)
        //     return addresses[1];
        // else
        //     return addresses[0];
    }
}