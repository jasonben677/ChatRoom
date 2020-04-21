using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Common;
using TestDll;

public class ChatClient
{
    public Message03 clientMessage;
    public Tranmitter tranmitter;
    public ChatClient()
    {
    }


    public bool Connect(string _address, int _port)
    {
        tranmitter = new Tranmitter(new TcpClient());

        if (tranmitter.Connect(_address, _port))
        {
            clientMessage = tranmitter.mMessage;
            return true;
        }
        else
        {
            clientMessage = null;
            return false;
        }

    }

    public void SendAccount(string _account, string _password)
    {
        clientMessage.msgType = 0;
        clientMessage.username = _account;
        clientMessage.password = _password;
        tranmitter.Send();
    }

    public void SendPos(float _x, float _y, float _z)
    {
        clientMessage.msgType = 2;
        clientMessage.x = _x;
        tranmitter.Send();
    }

    public void Run()
    {
        tranmitter.Run();
    }

}
