using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public class GenericData
	{
		public uint Id;
		public string StringId;

		public GenericData(string stringId)
		{
			this.SetId(stringId);	
		}

		public uint SetId(string stringId)
		{
			this.StringId = stringId;
			this.Id = Data.GetHash(this.StringId);
			return this.Id;
		}
	}

	public class LevelParams : GenericData
	{
		public class EntityPlacement
		{
			public EntityPlacement(uint id, int x, int y)
			{
				this.Id = id;
				this.X = x;
				this.Y = y;
			}

			public uint Id;
			public int X;
			public int Y;
		}

		public int SizeX = 4;
		public int SizeY = 7;

		public List<EntityPlacement> StaticPlacements;

		public LevelParams(string stringId, int x, int y) : base(stringId)
		{
			this.SizeX = x;
			this.SizeY = y;

			this.StaticPlacements = new  List<EntityPlacement>();
		}

		public void AddPlacement(uint id, int x, int y)
		{
			this.StaticPlacements.Add( new EntityPlacement(id, x, y) );
		}
	}

	public static class EntityClass
	{
		public enum Type
		{
			None,
			Inanimate,
			Interceptor,
			Bomber,
			Corvette,
			Frigate,
			Destroyer,
			Carrier,
			Unknown
		}

		public static void ApplyDefaults(Type type, EntityParams p)
		{
			p.type = type;

			switch(type)
			{
				case Type.Inanimate:
				{
					p.attack = 0;
					p.move = 0;
					p.attackRange = 0.0f;
					p.shield = 0;
					break;
				}

				case Type.Frigate:
				{
					p.attack = 4;
					p.move = 1;
					p.attackRange = 1.5f;
					p.shield = 4;
					break;
				}

				default:
				{
					break;
				}

			}
		}
	}

	public class EntityParams : GenericData
	{
		public enum DamageType
		{
			Kinetic,
			Explosive,
			Electrical,
			Light
		}

		public enum LightType
		{
			None,
			Blue,
			Green,
			Purple,
			Red
		}

		public EntityParams(string stringId) : base(stringId)
		{
		}

		public EntityParams(EntityParams p) : base(p.StringId)
		{
			this.Copy(p);
		}

		public void Copy(EntityParams p)
		{
			this.move = p.move;
			this.maneuverability = p.maneuverability;
			this.attackRange = p.attackRange;
			this.attack = p.attack;
			this.armour = p.armour;
			this.shield = p.shield;

			this.type = p.type;
			this.attackType = p.attackType;
			this.attackLightType = p.attackLightType;
			this.shieldLightType = p.shieldLightType;

			this.damageSpillOver = p.damageSpillOver;

			this.SizeOnGridX = p.SizeOnGridX;
			this.SizeOnGridY = p.SizeOnGridY;
		}

		public int cost
		{
			get
			{
				return (int)( System.Math.Floor((double)(this.attack + this.armour + this.shield) / 3) );
			}
		}

		public int move = 1;
		public int maneuverability = 1;
		public float attackRange = 1.0f;
		public int attack = 1;
		public int armour = 1;
		public int shield = 1;

		public EntityClass.Type type = EntityClass.Type.Unknown;
		public DamageType attackType = DamageType.Kinetic;
		public LightType attackLightType = LightType.None;
		public LightType shieldLightType = LightType.None;

		public bool damageSpillOver = false;

		public int SizeOnGridX = 0;
		public int SizeOnGridY = 0;
	}


	public static class Data
	{
		public static uint GetHash(string s) { return ((uint)s.GetHashCode()); }

		public static Dictionary<uint,EntityParams> Entites;
		public static Dictionary<uint,LevelParams> Levels;

		public static long MillisecondsPerTurn = 2000;
		public static int DefaultSizeX = 4;
		public static int DefaultSizeY = 7;

		public static void Init()
		{
			Data.Entites = EntitiesData.Init();
			Data.Levels = LevelsData.Init();
		}
	}
}
