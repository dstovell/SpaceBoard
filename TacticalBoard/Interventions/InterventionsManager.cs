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

		public Game ParentGame;
		public List<Request> Requests;
		public List<Request> Results;

		private uint RequestCount = 0;

		public NetServerGame ServerGame
		{
			get
			{
				return (this.FlowType == Flow.Server) ? (this.ParentGame as NetServerGame) : null;
			}
		}

		public uint GenerateRequestId(uint playerId)
		{
			this.RequestCount++;

			string hashableString = string.Format("{0}_{1}", playerId, this.RequestCount);
			return (uint)Data.GetHash(hashableString);
		}

		public void RequestIntervention(Request r)
		{
			r.RequestId = this.GenerateRequestId(this.ParentGame.LocalPlayer.Id);
			this.AddRequest(r);
			if (this.FlowType == Flow.Client)
			{
				this.ParentGame.SendInterventionRequest(r);
			}
		}

		public void HandleIntervention(Request r)
		{
			if (this.FlowType == Flow.Client)
			{
				this.AddRequest(r);
			}
			else if (this.FlowType == Flow.Server)
			{
				//Auto Success for now!
				r.Result = ResultType.Success;
				this.ServerGame.SendInterventionResult(r);
				this.AddRequest(r);
			}
		}

		public void AddRequest(Request r)
		{
			int oldRequestIndex = this.Requests.FindIndex(delegate(Request other) {
						               return (other.RequestId == r.RequestId);
						            });

			if (oldRequestIndex == -1)
			{
				this.Requests.Add(r);
				return;
			}

			this.Requests[oldRequestIndex].Update(r);
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
					Debug.Log("Interventions Request=" + req.RequestId + " " + req.Result.ToString());
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
				if (res.Turn >= turnCount)
				{
					res.HandleRequestAction();
				}
			}

			this.Results.RemoveAll(InterventionsManager.RemoveDelivered);
		}

		public InterventionsManager(Flow ft, Game parentGame)
		{
			this.FlowType = ft;
			this.ParentGame = parentGame;
			this.Requests = new List<Request>();
			this.Results = new List<Request>();
		}
	}
}
