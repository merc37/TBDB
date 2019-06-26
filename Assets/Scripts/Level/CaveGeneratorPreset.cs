using System;
using System.Collections;
using System.Collections.Generic;
using Level;
using NaughtyAttributes;
using UnityEngine;

namespace Level
{
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CaveGeneratorPreset", order = 1)]
	public class CaveGeneratorPreset : LevelGeneratorPreset
	{
		[Header("Cave generator parameters")]
		[Slider(0, 100)]
		public int FillPercent;
		[ReorderableList]
		public List<Vector2Int> SmoothingRounds;

		public override void Generate()
		{
			if (lGen == null) return;

			lGen.InitializeChunk();
			lGen.RefreshSeed();
			lGen.FillRandom(FillPercent);

			foreach (var round in SmoothingRounds)
			{
				for (int i = 0; i < round.x; i++)
				{
					lGen.SmoothMap(w => w >= round.y, w => w < 4);
				} 
			}
		}

		private bool WallCondition(int wallCount)
		{
			return wallCount >= 4;
		}

		private bool EmptyCondition(int wallCount)
		{
			return wallCount < 4;
		}
	}
}
