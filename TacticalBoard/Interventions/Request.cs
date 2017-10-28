using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class Request
	{
		public delegate void RequestDelegate(Request r);
		public RequestDelegate OnRequestComplete;
		public RequestDelegate OnRequestAction;

		public void HandleRequestComplete()
		{
			if (InterventionsManager.Verbose)
			{
				TacticalBoard.Debug.Log("HandleRequestComplete " + this.Type.ToString());
			}
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
			if (InterventionsManager.Verbose)
			{
				TacticalBoard.Debug.Log("HandleRequestAction " + this.Type.ToString());
			}
			if ((this.OnRequestAction != null) && (this.Result == ResultType.Success))
			{
				this.OnRequestAction(this);
			}
			this.Delivered = true;
		}

		public Request(InterventionType type, long turn, uint entityId = 0, ushort gridNodeId = 0)
		{
			this.Type = type;
			this.TurnRequested = turn;
			this.EntityId = entityId;
			this.GridNodeId = gridNodeId;
			if (InterventionsManager.Verbose)
			{
				TacticalBoard.Debug.Log("Request " + this.Type.ToString() + " turn=" + turn + " entityId=" + entityId + " gridNodeId=" + gridNodeId);
			}
		}

		public long TurnRequested;
		public long Turn;
		public uint RequestId;
		public uint EntityId;
		public ushort GridNodeId;
		public InterventionType Type;

		public ResultType Result;
		public bool Delivered;
	}
}

