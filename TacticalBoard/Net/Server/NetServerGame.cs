using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class NetServerGame : Game
	{
		public NetServerGame(uint id) : base(id)
		{
			this.LoadRandomLevel();
			this.State = GameState.WaitingToConnect;
		}

		public override long GetTime()
		{
			return (System.DateTime.UtcNow.Ticks/System.TimeSpan.TicksPerMillisecond);
		}

		public void AddPlayer(NetServerPlayer p)
		{
			PlayerTeam team = (this.Players.Count == 0) ? PlayerTeam.TeamA : PlayerTeam.TeamB;

			Debug.Log("Player " + p.Id + " has joined game " + this.Id + " as " + team.ToString() + " Entities=" + p.Entities.Length);

			//Send this first so the player joining won't get it
			PlayerJoin msg = new PlayerJoin(p.Id, team, p.Entities);
			this.SendToPlayers(msg, Hazel.SendOption.Reliable);

			this.AddPlayer(p, team);

			p.Game = this;
			p.SetState(NetServerPlayer.ConnectionState.Connected);

			GameJoined gj = new GameJoined(this.Id, p.Id, p.Entities, this.LevelId, team, this.GetTime());

			p.Send(gj, Hazel.SendOption.Reliable);

			if (this.State == GameState.WaitingToConnect)
			{
				this.State = GameState.WaitingForPlayers;
			}
		}

		public void SendToPlayers(NetMessage msg, Hazel.SendOption sendOption = Hazel.SendOption.Reliable)
		{
			//Do we need to make this thread safe?
			for (int i=0; i<this.Players.Count; i++)
			{
				NetServerPlayer p = this.Players[i] as NetServerPlayer;
				if (p.IsConnected())
				{
					p.Send(msg, sendOption);
				}
			}
		}

		public void SendToPlayers(byte [] data, Hazel.SendOption sendOption = Hazel.SendOption.Reliable)
		{
			//Do we need to make this thread safe?
			for (int i=0; i<this.Players.Count; i++)
			{
				NetServerPlayer p = this.Players[i] as NetServerPlayer;
				if (p.IsConnected())
				{
					p.Send(data, sendOption);
				}
			}
		}

		public override void Update()
		{
			base.Update();

			if (this.State == GameState.WaitingForPlayers)
			{
				if (this.Players.Count == Game.MaxPlayers)
				{
					long now = this.GetTime();
					this.StartTime = now + TacticalBoard.Data.MillisecondsDelayStart;
					this.State = GameState.WaitingForStart;

					Debug.Log("MaxPlayers Reached, Starting Game at " + this.StartTime);	
					GameStart gs = new GameStart(this.StartTime);
					this.SendToPlayers(gs, Hazel.SendOption.Reliable);
				}
			}
		}

		public void OnData(NetServerPlayer p, object obj, Hazel.DataReceivedEventArgs arg)
		{
			NetMessageType type = NetMessage.GetType(arg.Bytes);

			switch (type)
			{
				case NetMessageType.Handshake:
				{
					break;
				}

				default:
				{
					//Don't know this message, ignoring
					this.SendToPlayers(arg.Bytes, arg.SendOption);
					break;
				}
			}
		}

		public void OnDisconnect(NetServerPlayer p, object obj, Hazel.DisconnectedEventArgs arg)
		{
			Debug.Log("Player " + p.Id + " has disconnected and left game " + this.Id );
			PlayerLeave msg = new PlayerLeave(p.Id);
			this.Players.Remove(p);

			this.SendToPlayers(msg, Hazel.SendOption.Reliable);
		}
	}
}


