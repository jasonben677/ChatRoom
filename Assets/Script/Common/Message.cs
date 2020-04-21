using System;

namespace Common
{
	[Serializable]
	public class Message
	{
		public MessageType MessageType = MessageType.None;

		public string username;
		public string password;

		public bool success;

		public Message()
		{
		}
	}
}

