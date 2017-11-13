using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class Request : Serializable
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

		public Request(uint playerId, InterventionType type, long turn, uint entityId = 0, ushort gridNodeId = 0)
		{
			this.PlayerId = playerId;
			this.Type = type;
			this.TurnRequested = turn;
			this.EntityId = entityId;
			this.GridNodeId = gridNodeId;
			if (InterventionsManager.Verbose)
			{
				TacticalBoard.Debug.Log("Request " + this.Type.ToString() + " turn=" + turn + " entityId=" + entityId + " gridNodeId=" + gridNodeId);
			}
		}

		public Request()
		{
		}

		public void Update(Request updated)
		{
			this.TurnRequested = updated.TurnRequested;
			this.Turn = updated.Turn;
			this.GridNodeId = updated.GridNodeId;
			this.Result = updated.Result;
		}

		public override void OnSerialize(Serializer s)
		{
			this.TurnRequested = s.Serialize(this.TurnRequested);
			this.Turn = s.Serialize(this.Turn);
			this.RequestId = s.Serialize(this.RequestId);
			this.RequestId = s.Serialize(this.RequestId);
			this.PlayerId = s.Serialize(this.PlayerId);
			this.EntityId = s.Serialize(this.EntityId);
			this.GridNodeId = s.Serialize(this.GridNodeId);

			this.Type = (InterventionType)s.Serialize((int)this.Type);
			this.Result = (ResultType)s.Serialize((int)this.Result);
		}

		public long TurnRequested;
		public long Turn;
		public uint RequestId;
		public uint PlayerId;
		public uint EntityId;
		public ushort GridNodeId;
		public InterventionType Type;

		public ResultType Result;
		public bool Delivered;
	}
}

