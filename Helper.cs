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
    public static string ServerIP = "192.168.1.126";

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
    public static string ExternalIp()
    {
        var addresses = Dns.GetHostEntry((Dns.GetHostName()))
                .AddressList
                .Where(x => x.AddressFamily == AddressFamily.InterNetwork)
                .Select(x => x.ToString())
                .ToArray();

        // foreach(var ip in addresses)
        // {
        //     Console.WriteLine(ip);
        // }
        if(addresses.Length > 1)
            return addresses[1];
        else
            return addresses[0];
    }
}