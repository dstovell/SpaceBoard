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
		}

		public override long GetTime()
		{
			return (System.DateTime.UtcNow.Ticks/System.TimeSpan.TicksPerMillisecond);
		}

		public void AddPlayer(uint playerId, Hazel.Connection conn)
		{
			NetServerPlayer player = new NetServerPlayer(playerId, conn);
			this.AddPlayer(player);
		}

		public void AddPlayer(NetServerPlayer p)
		{
			if (this.Players.Contains(p))
			{
				return;
			}

			//Send this first so the player joining won't get it
			PlayerJoin msg = new PlayerJoin(p.Id);
			this.SendToPlayers(msg, Hazel.SendOption.Reliable);

			PlayerTeam team = (this.Players.Count == 0) ? PlayerTeam.TeamA : PlayerTeam.TeamB;
			this.Players.Add(p);
			p.AssignToGame(this.Id, team, this);

			Debug.Log("Player " + p.Id + " has joined game " + this.Id + " as " + team.ToString() );

			GameJoined gj = new GameJoined(this.Id, p.Id, this.LevelId, team, this.GetTime());
			p.Send(gj, Hazel.SendOption.Reliable);
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


