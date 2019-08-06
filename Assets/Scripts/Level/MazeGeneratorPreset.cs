using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MazeGeneratorPreset", order = 1)]
	public class MazeGeneratorPreset : LevelGeneratorPreset
	{
		[Header("Maze generator parameters")]
		[Slider(0, 100)] public int FillPercent;
		[Slider(0, 100)] public int Generations;

		public override void Generate()
		{
			if (lGen == null) return;

			lGen.InitializeChunk();
			lGen.RefreshSeed();
			lGen.FillRandom(LevelFlags.Wall, FillPercent);
			lGen.FillEdge(LevelFlags.Wall);

			for (int i = 0; i < Generations; i++)
			{
				lGen.ApplyRules(
					(chunk, x, y) => lGen.GetNeighborCount(chunk, x, y, 1, LevelFlags.Wall),
					true,
					(neighborCount => neighborCount == 3, LevelFlags.Wall),
					(neighborCount => neighborCount < 1 || neighborCount > 5, LevelFlags.Empty));
			}


		}

		public Vector2Int test;
		[Button]
		public void Flood()
		{
			lGen.Flood(test.x, test.y, LevelFlags.Wall, LevelFlags.Empty);
		}
	}
}