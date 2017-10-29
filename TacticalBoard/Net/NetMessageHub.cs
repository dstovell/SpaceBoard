using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class NetMessageHub
	{
		public static void SendDataToConn(NetMessage msg, Hazel.Connection conn, Hazel.SendOption sendOption = Hazel.SendOption.Reliable)
		{
			byte [] buffer = NetMessageHub.SerializeData(msg);
			conn.SendBytes(buffer, sendOption);
		}

		public static byte [] SerializeData(NetMessage msg, byte [] buffer = null)
		{
			//No buffer provided, figure out how big it needs to be dynamically
			if (buffer == null)
			{
				SizeSerializer ss = new SizeSerializer();
				msg.OnSerialize(ss);
				int dataSize = ss.GetSize();

				buffer = new byte[dataSize];
			}

			WriteSerializer ws = new WriteSerializer(buffer);

			msg.OnSerialize(ws);

			return ws.GetData();
		}

		public static bool DeSerializeData(NetMessage msg, byte [] buffer, int start = 0)
		{
			if ((msg == null) || (buffer == null))
			{
				return false;
			}

			ReadSerializer rs = new ReadSerializer(buffer, start);
			msg.OnSerialize(rs);
			return true;
		}
	}

}