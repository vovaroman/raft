using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;

public static class Helper
{
    public static int UdpPort = 0;//GetAvailablePort(1000);

    public static int ServerPort = 616;
    public static string ServerIP = "10.35.1.82";

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

    }

}