using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public abstract class Serializable
	{
		public abstract void OnSerialize(Serializer s);
	}
	
	public abstract class Serializer
	{
		protected uint index;
		protected byte [] data;
		public Serializer(byte [] _data)
		{
			this.data = _data;
			this.index = 0;
		}

		public abstract int Serialize(int n);
		public abstract uint Serialize(uint n);
		public abstract ushort Serialize(ushort n);
		//public abstract void Serialize(float n);
	}

	public class WriteSerializer : Serializer
	{
		public WriteSerializer(byte [] _data) : base(_data)
		{
		}

		protected void WriteBytes(byte [] newData)
		{
			for (int i=0; i<newData.Length; i++)
			{
				if (this.index == this.data.Length)
				{
					return;
				}

				this.data[this.index] = newData[i];
				this.index++;
			}
		}

		public override int Serialize(int n)
		{
			this.WriteBytes( System.BitConverter.GetBytes(n) );
			return n;
		}

		public override uint Serialize(uint n)
		{
			this.WriteBytes( System.BitConverter.GetBytes(n) );
			return n;
		}

		public override ushort Serialize(ushort n)
		{
			this.WriteBytes( System.BitConverter.GetBytes(n) );
			return n;
		}
	}

	public class ReadSerializer : Serializer
	{
		public ReadSerializer(byte [] _data) : base(_data)
		{
		}

		protected void ReadBytes(byte [] newData)
		{
			for (int i=0; i<newData.Length; i++)
			{
				if (this.index == this.data.Length)
				{
					return;
				}

				newData[i] = this.data[this.index];
				this.index++;
			}
		}

		public override int Serialize(int n)
		{
			byte [] newData = new byte[4];
			this.ReadBytes(newData);
			return System.BitConverter.ToInt32(newData, 0);
		}

		public override uint Serialize(uint n)
		{
			byte [] newData = new byte[4];
			this.ReadBytes(newData);
			return System.BitConverter.ToUInt32(newData, 0);
		}

		public override ushort Serialize(ushort n)
		{
			byte [] newData = new byte[2];
			this.ReadBytes(newData);
			return System.BitConverter.ToUInt16(newData, 0);
		}
	}
}

