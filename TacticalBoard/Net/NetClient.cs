using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class NetClient
	{
		private Hazel.Udp.UdpConnection Conn;
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
			this.Conn.Connect(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }); 
		}

		private void OnData(object obj, Hazel.DataReceivedEventArgs arg)
		{
		}

		private void OnDisconnect(object obj, Hazel.DisconnectedEventArgs arg)
		{
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

