using UnityEngine;
using System.Collections;

namespace TacticalBoard
{
	public class NetClient
	{
		private Hazel.Udp.UdpClientConnection Conn;
		private Hazel.NetworkEndPoint EndPoint;

		public NetClient(string address, int port, Hazel.IPMode ipMode = Hazel.IPMode.IPv4)
		{
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
			//this.Conn.Connect(this.EndPoint);
		}

		private void OnData(object obj, Hazel.DataReceivedEventArgs arg)
		{
		}

		private void OnDisconnect(object obj, Hazel.DisconnectedEventArgs arg)
		{
		}
	}
}
