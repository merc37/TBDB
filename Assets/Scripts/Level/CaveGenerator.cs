using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro.EditorUtilities;
using UnityEngine;

namespace Level
{
	public class CaveGenerator : LevelGenerator
	{
		[Slider(0, 100)]
		public int FillPercent;
		[Slider(0, 10)]
		public int SmoothingIterations;

		[Button]
		public void Generate()
		{
			InitializeChunk();
			RefreshSeed();
			FillRandom(FillPercent);

			for (int i = 0; i < SmoothingIterations; i++)
			{
				SmoothMap(WallCondition, EmptyCondition);
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
