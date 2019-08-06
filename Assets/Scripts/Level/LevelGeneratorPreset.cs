using System.Collections;
using System.Collections.Generic;
using Level;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Level
{
	public abstract class LevelGeneratorPreset : ScriptableObject
	{
		public LevelGenerator lGen { get; set; }

		[Header("Base map parameters")]
		public Vector2Int ChunkSize;
		public bool UseRandomSeed;
		public bool SeedEnabled() { return !UseRandomSeed; }
		[EnableIf("SeedEnabled")]
		public string Seed;
		
		[Button]
		public abstract void Generate();
	}
}