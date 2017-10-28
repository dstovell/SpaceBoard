using UnityEngine;
using System.Collections;

namespace TacticalBoard
{
	public class NetManager
	{
		public static bool Verbose = false;
		public static TacticalBoard.NetManager Instance = null;

		private NetClient Client = null;
		private NetServer Server = null;

		public enum Flow
		{
			Client,
			Server
		}
		public Flow FlowType;

		public static void Init(Flow ft)
		{
			if (TacticalBoard.NetManager.Instance == null)
			{
				TacticalBoard.NetManager.Instance = new TacticalBoard.NetManager(ft);
			}
		}

		public bool IsClient()
		{
			return (this.FlowType == Flow.Client);
		}

		public bool IsServer()
		{
			return (this.FlowType == Flow.Server);
		}

		public NetManager(Flow ft)
		{
			this.FlowType = ft;

			if (this.IsClient())
			{
			}
			else if (this.IsServer())
			{
				//this.Server = new NetServer();
			}
		}
	}
}