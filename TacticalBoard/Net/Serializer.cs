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
		protected int index;
		protected byte [] data;
		public Serializer(byte [] _data, int start = 0)
		{
			this.data = _data;
			this.index = start;
		}

		public abstract int Serialize(int n);
		public abstract uint Serialize(uint n);
		public abstract uint [] Serialize(uint [] n);

		public abstract short Serialize(short n);
		public abstract ushort Serialize(ushort n);

		public abstract long Serialize(long n);
		public abstract ulong Serialize(ulong n);
	}

	public class WriteSerializer : Serializer
	{
		public WriteSerializer(byte [] _data, int start = 0) : base(_data, start)
		{
		}

		protected virtual void WriteBytes(byte [] newData)
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

		public byte [] GetData()
		{
			return this.data;
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

		public override uint [] Serialize(uint [] n)
		{
			ushort length = (ushort)n.Length;
			this.Serialize(length);
			for (ushort i=0; i<length; i++)
			{
				this.Serialize(n[i]);
			}
			return n;
		}

		public override short Serialize(short n)
		{
			this.WriteBytes( System.BitConverter.GetBytes(n) );
			return n;
		}

		public override ushort Serialize(ushort n)
		{
			this.WriteBytes( System.BitConverter.GetBytes(n) );
			return n;
		}

		public override long Serialize(long n)
		{
			this.WriteBytes( System.BitConverter.GetBytes(n) );
			return n;
		}

		public override ulong Serialize(ulong n)
		{
			this.WriteBytes( System.BitConverter.GetBytes(n) );
			return n;
		}
	}

	public class ReadSerializer : Serializer
	{
		public ReadSerializer(byte [] _data, int start = 0) : base(_data, start)
		{
		}

		public override int Serialize(int n)
		{
			int value = System.BitConverter.ToInt32(this.data, this.index);
			this.index += sizeof(int);
			return value;
		}

		public override uint Serialize(uint n)
		{
			uint value = System.BitConverter.ToUInt32(this.data, this.index);
			this.index += sizeof(uint);
			return value;
		}

		public override uint [] Serialize(uint [] n)
		{
			ushort length = 0;
			length = this.Serialize(length);
			n = new uint[length];
			for (ushort i=0; i<length; i++)
			{
				this.Serialize(n[i]);
			}
			return n;
		}

		public override short Serialize(short n)
		{
			short value = System.BitConverter.ToInt16(this.data, this.index);
			this.index += sizeof(short);
			return value;
		}

		public override ushort Serialize(ushort n)
		{
			ushort value = System.BitConverter.ToUInt16(this.data, this.index);
			this.index += sizeof(ushort);
			return value;
		}


		public override long Serialize(long n)
		{
			long value = System.BitConverter.ToInt64(this.data, this.index);
			this.index += sizeof(long);
			return value;
		}

		public override ulong Serialize(ulong n)
		{
			ulong value = System.BitConverter.ToUInt64(this.data, this.index);
			this.index += sizeof(ulong);
			return value;
		}
	}

	public class SizeSerializer : WriteSerializer
	{
		public SizeSerializer(int start = 0) : base(null, start)
		{
		}

		protected override void WriteBytes(byte [] newData)
		{
			for (int i=0; i<newData.Length; i++)
			{
				this.index++;
			}
		}

		public int GetSize()
		{
			return this.index;
		}
	}
}

