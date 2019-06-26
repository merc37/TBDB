﻿using System.Collections;
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
		[Slider(0, 10)]
		public int SmoothingIterations;

		public override void Generate()
		{
			if (lGen == null) return;

			lGen.InitializeChunk();
			lGen.RefreshSeed();
			lGen.FillRandom(FillPercent);

			for (int i = 0; i < SmoothingIterations; i++)
			{
				lGen.SmoothMap(WallCondition, EmptyCondition);
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