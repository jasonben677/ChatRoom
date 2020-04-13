using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using TestDll;

public class ChatClient
{
    public delegate void MessageProcess(Message _player);
    public MessageProcess messageProcess;
    Message send;
    Message receive;
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
        send = new Message();
        send.msgType = 0;
        send.username = _account;
        send.password = _password;
        serialManager.SerializeClass(mClient, send);
    }

    public void SendPos(float _x, float _y, float _z)
    {
        send = (send != null) ? send : new Message();
        send.msgType = 2;
        send.x = _x;
        send.y = _y;
        send.z = _z;
        serialManager.SerializeClass(mClient, send);
    }

    public void Run()
    {
        if (mClient.Available > 0)
        {
            HandleReceiveMessages(mClient);
        }
    }

    private void HandleReceiveMessages(TcpClient tcpClient)
    {
        receive = serialManager.DeserializeClass(tcpClient);
        messageProcess?.Invoke(receive);
    }

}
