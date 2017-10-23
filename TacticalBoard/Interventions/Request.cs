using UnityEngine;
using System.Collections;

namespace TacticalBoard
{
	public class Request
	{
		public delegate void RequestDelegate(Request r);
		public RequestDelegate OnRequestComplete;
		public RequestDelegate OnRequestAction;

		public void HandleRequestComplete()
		{
			if (this.OnRequestComplete != null)
			{
				this.OnRequestComplete(this);
			}

			if (this.Result == ResultType.Failure)
			{
				this.Delivered = true;
			}
		}

		public void HandleRequestAction()
		{
			if ((this.OnRequestAction != null) && (this.Result == ResultType.Success))
			{
				this.OnRequestAction(this);
			}
			this.Delivered = true;
		}

		public Request(InterventionType type, long turn, ushort entityId = 0, ushort gridNodeId = 0)
		{
			this.Type = type;
			this.TurnRequested = turn;
			this.EntityId = entityId;
			this.GridNodeId = gridNodeId;
		}

		public long TurnRequested;
		public long Turn;
		public uint RequestId;
		public ushort EntityId;
		public ushort GridNodeId;
		public InterventionType Type;

		public ResultType Result;
		public bool Delivered;
	}
}

