using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class InterventionsManager 
	{
		public static bool Verbose = false;

		public enum Flow
		{
			Local,
			Client,
			Server
		}
		public Flow FlowType;

		private long LocalTurnWait = 1;

		public List<Request> Requests;
		public List<Request> Results;

		public void RequestIntervention(Request r)
		{
			this.Requests.Add(r);
		}

		private static bool RemoveDelivered(Request r)
		{
			return r.Delivered;
		}

		public void Update(long turnCount)
		{
			//Local flow to simulate server
			if (this.FlowType == Flow.Local)
			{
				for (int i=0; i<this.Requests.Count; i++)
				{
					Request r = this.Requests[i];
					if (InterventionsManager.Verbose)
					{
						Debug.Log("Request RequestId=" + r.RequestId + " EntityId=" + r.EntityId + " Result=" + r.Result.ToString());
					}
					if ((r.Result == ResultType.Pending) && (r.TurnRequested <= (turnCount - this.LocalTurnWait)))
					{
						r.Result = ResultType.Success;
						r.Turn = turnCount + 1;
					}
				}
			}

			for (int i=0; i<this.Requests.Count; i++)
			{
				Request req = this.Requests[i];
				if (req.Result != ResultType.Pending)
				{
					this.Results.Add(req);
				}
			}

			for (int i=0; i<this.Results.Count; i++)
			{
				Request res = this.Results[i];
				if (this.Requests.Contains(res))
				{
					this.Requests.Remove(res);
					res.HandleRequestComplete();
				}
			}

			for (int i=0; i<this.Results.Count; i++)
			{
				Request res = this.Results[i];
				if (res.Turn == turnCount)
				{
					res.HandleRequestAction();
				}
			}

			this.Results.RemoveAll(InterventionsManager.RemoveDelivered);
		}

		public InterventionsManager(Flow ft)
		{
			this.Requests = new List<Request>();
			this.Results = new List<Request>();
		}
	}
}
