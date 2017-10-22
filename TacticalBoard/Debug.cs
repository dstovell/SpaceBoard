using UnityEngine;
using System.Collections;

namespace TacticalBoard
{

	public static class Debug
	{
		public static void Log(string msg)
		{
			#if UNITY_EDITOR
			UnityEngine.Debug.Log("[TacticalBoard] " + msg);
			#endif
		}

		public static void LogError(string msg)
		{
			#if UNITY_EDITOR
			UnityEngine.Debug.LogError("[TacticalBoard] " + msg);
			#endif
		}
	}
}

