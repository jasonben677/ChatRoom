using System;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using TestDll;

public class ChatClient
{
    public delegate void MessageProcess();
    public MessageProcess messageProcess;
    SerializationManager serialManager = new SerializationManager();
    TcpClient mClient = null;

    public ChatClient()
    {
    }


    public bool Connect(string _address, int _port)
    {
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

    public void SendAccount(string _account, string _password)
    {
        Message c_message = new Message();
        c_message.msgType = 0;
        c_message.username = _account;
        c_message.password = _password;
        serialManager.SerializeClass(mClient, c_message);
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
        Message c_message = serialManager.DeserializeClass(tcpClient);
        switch ((MessageType)c_message.msgType)
        {
            case MessageType.Login:
                messageProcess?.Invoke();
                break;

            default:
                break;
        }
    }

}
