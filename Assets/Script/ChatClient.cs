using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using System.Net;
using UnityEngine;

public class ChatClient
{
    public ChatRoomManager RoomManager;
    TcpClient mClient = null;
    string mAddress = "";
    int mPort;

    public ChatClient()
    { 
    
    }


    public bool Connect(string _address, int _port)
    {
        mAddress = _address;
        mPort = _port;

        mClient = new TcpClient();

        try
        {
            IPHostEntry host = Dns.GetHostEntry(_address);
            IPAddress address = null;
            foreach (IPAddress h in host.AddressList)
            {
                if (h.AddressFamily == AddressFamily.InterNetwork)
                {
                    address = h;
                    break;
                }
            }
            mClient.Connect(address.ToString(), _port);
            //Debug.Log("Connected to Chat Server: " + _address + ":" + _port + "\n");

            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Exception happened: " + e.ToString());
            return false;
        }
    }

    public void SendName(string _name)
    {
        string request = "LOGINNAME:" + _name;
        byte[] requestBuffer = System.Text.Encoding.ASCII.GetBytes(request);
        mClient.GetStream().Write(requestBuffer, 0, requestBuffer.Length);
    }

    public void SendBroadcast(string _message)
    {
        string request = "BROADCAST:" + _message;
        byte[] requestBuffer = System.Text.Encoding.ASCII.GetBytes(request);
        mClient.GetStream().Write(requestBuffer, 0, requestBuffer.Length);
    }

    public void Run()
    {
        if (mClient.Available > 0)
        {
            HandleReceiveMessages(mClient);
        }
    }

    public void HandleReceiveMessages(TcpClient tcpClient)
    {
        NetworkStream tcpClientStream = tcpClient.GetStream();

        int numByte = tcpClient.Available;
        byte[] buffer = new byte[numByte];
        int byteRead = tcpClientStream.Read(buffer, 0, buffer.Length);

        string request = System.Text.Encoding.ASCII.GetString(buffer).Substring(0, byteRead);

        if (request.StartsWith("MESSAGE:", StringComparison.OrdinalIgnoreCase))
        {
            string[] aTokens = request.Split(':');
            string sName = aTokens[1];
            string sMessage = aTokens[2];
            //Debug.Log(sName + " said: " + sMessage);
            RoomManager.MessageShow(sName + " said: " + sMessage);
        }
    }
}
