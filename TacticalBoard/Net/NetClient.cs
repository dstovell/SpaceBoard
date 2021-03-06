﻿using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class NetClient
	{
		private Hazel.Udp.UdpConnection Conn;
		private Hazel.NetworkEndPoint EndPoint;
		private Game ParentGame;

		public NetClient(Game game, string address, int port, Hazel.IPMode ipMode = Hazel.IPMode.IPv4)
		{
			this.ParentGame = game;
			this.EndPoint = new Hazel.NetworkEndPoint(address, port, ipMode);
			this.Conn = new Hazel.Udp.UdpClientConnection(this.EndPoint);
			this.AddListeners();
		}

		private void AddListeners()
		{
			this.Conn.DataReceived += this.OnData;
			this.Conn.Disconnected += this.OnDisconnect;
		}

		private void RemoveListeners()
		{
			this.Conn.DataReceived -= this.OnData;
		}

		public void Connect()
		{
			Handshake hs = new Handshake();
			hs.playerId = 100001;

			uint hash = Data.GetHash("Frigate_Predator_Blue");
			hs.entites = new uint[3];
			hs.entites[0] = hash;
			hs.entites[1] = hash;
			hs.entites[2] = hash;

			byte [] buffer = NetMessageHub.SerializeData(hs);
			this.Conn.Connect(buffer); 
		}

		public void SendMessage(NetMessage msg, Hazel.SendOption sendOption = Hazel.SendOption.Reliable)
		{
			byte [] buffer = NetMessageHub.SerializeData(msg);
			this.Conn.SendBytes(buffer, sendOption);
		}

		private void OnData(object obj, Hazel.DataReceivedEventArgs arg)
		{
			if (this.ParentGame != null)
			{
				NetMessageType type = NetMessage.GetType(arg.Bytes);

				switch (type)
				{
					case NetMessageType.GameJoined:
					{
						GameJoined msg = new GameJoined();
						NetMessageHub.DeSerializeData(msg, arg.Bytes, 0);
						this.ParentGame.HandleGameJoined(msg);
						break;
					}

					case NetMessageType.GameStart:
					{
						GameStart msg = new GameStart();
						NetMessageHub.DeSerializeData(msg, arg.Bytes, 0);
						this.ParentGame.HandleGameStart(msg);
						break;
					}

					case NetMessageType.GameEnd:
					{
						GameEnd msg = new GameEnd();
						NetMessageHub.DeSerializeData(msg, arg.Bytes, 0);
						this.ParentGame.HandleGameEnd(msg);
						break;
					}

					case NetMessageType.PlayerJoin:
					{
						PlayerJoin msg = new PlayerJoin();
						NetMessageHub.DeSerializeData(msg, arg.Bytes, 0);
						this.ParentGame.HandlePlayerJoin(msg);
						break;
					}

					case NetMessageType.PlayerLeave:
					{
						PlayerLeave msg = new PlayerLeave();
						NetMessageHub.DeSerializeData(msg, arg.Bytes, 0);
						this.ParentGame.HandlePlayerLeave(msg);
						break;
					}

					case NetMessageType.PlayerIntervention:
					{
						PlayerIntervention msg = new PlayerIntervention();
						NetMessageHub.DeSerializeData(msg, arg.Bytes, 0);
						this.ParentGame.HandlePlayerIntervention(msg);
						break;
					}


					default:
					{
						break;
					}
				}
			}
		}

		private void OnDisconnect(object obj, Hazel.DisconnectedEventArgs arg)
		{
			//Handle Disconnect here
		}
	}
}


//class UdpClientExample
//{
//    static void Main(string[] args)
//    {
//        using (UdpConnection connection = new UdpConnection())
//        {
//            ManualResetEvent e = new ManualResetEvent(false);
//
//            //Whenever we receive data print the number of bytes and how it was sent
//            connection.DataReceived += (object sender, DataReceivedEventArgs a) =>
//                Console.WriteLine("Received {0} bytes via {1}!", a.Bytes.Length, a.SendOption);
//
//            //When the end point disconnects from us then release the main thread and exit
//            connection.Disconnected += (object sender, DisconnectedEventArgs a) =>
//                e.Set();
//
//            //Connect to a server
//            connection.Connect(new NetworkEndPoint("127.0.0.1", 4296));
//
//            //Wait until the end point disconnects from us
//            e.WaitOne();
//        }
//    }
//}

