using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using TestDll;
using System.Linq;

namespace Common
{
	public class Tranmitter
	{
		TcpClient mClient = null;
		public Message03 mMessage = null;


		SerializationManager serialManager = new SerializationManager();
		Dictionary<int, Delegate> mDelegates = new Dictionary<int, Delegate>();

		public Tranmitter(TcpClient _client = null)
		{
			mClient = _client;
			mMessage = new Message03();
		}


		public bool Connect(string _ip, int _port)
		{

			mClient = new TcpClient();
			try
			{
				IPHostEntry host = Dns.GetHostEntry(_ip);
				var address = (from h in host.AddressList where h.AddressFamily == AddressFamily.InterNetwork select h).First();

				mClient.Connect(address.ToString(), _port);
				Console.WriteLine("Connected to server: " + _ip + ":" + _port + "\n");

				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception happened: " + e.ToString());
				return false;
			}
		}

		public bool IsConnect()
		{
			if (!mClient.Connected) return false;

			try
			{
				if (mClient.Client.Poll(0, SelectMode.SelectRead))
				{
					byte[] buff = new byte[1];
					if (mClient.Client.Receive(buff, SocketFlags.Peek) == 0) return false;
				}
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		public void Close()
		{
			mClient.Close();
			mClient.Dispose();
		}

		public void Send()
		{
			serialManager.SerializeClass(mClient, mMessage);
		}

		public void Register(int _type, Action<Tranmitter, Message03> _action)
		{
			mDelegates[_type] = _action;
		}

		public void Run()
		{
			if (mClient.Available > 0)
			{
				HandleReceiveMessage();
			}
		}

		private void HandleReceiveMessage()
		{
			mMessage = serialManager.DeserializeClass(mClient);
			int num = mMessage.msgType;
			if (num == -1)
			{
				Console.WriteLine("error");
			}
			else
			{
				mDelegates[num]?.DynamicInvoke(this, mMessage);
			}
		}
	}
}

