using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
	public enum LevelFlags
	{
		Empty = 0b000000000,
		Wall  = 0b000000001,
		Flood = 0b100000000
	}

	public static class LevelFlagsExtension
	{
		public static byte ToByte(this LevelFlags e)
		{
			return (byte) e;
		}
	}
}
