using System.Collections;
using System.Collections.Generic;

namespace TacticalBoard
{
	public static class Debug
	{
		public static void Log(string msg)
		{
			#if UNITY_EDITOR
			UnityEngine.Debug.Log("[TacticalBoard] " + msg);
			#else
			System.Console.WriteLine("[TacticalBoard] " + msg);
			#endif
		}

		public static void LogError(string msg)
		{
			#if UNITY_EDITOR
			UnityEngine.Debug.LogError("[TacticalBoard] " + msg);
			#else
			System.Console.WriteLine("[TacticalBoard] ERROR " + msg);
			#endif
		}
	}
}

