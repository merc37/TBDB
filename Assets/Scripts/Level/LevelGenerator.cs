using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using NaughtyAttributes;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

namespace Level
{
	public class LevelGenerator : MonoBehaviour
	{
		public LevelGeneratorPreset Settings;

		private byte[,] chunk;
		private byte[,] chunkTemp;
		private Random random;

		[Button]
		public void Generate()
		{
			if (Settings == null) return;

			Settings.Generate();
		}

		#region Monobehaviour
		void Awake()
		{
			if (Settings != null)
				Settings.lGen = this;

			//InitializeChunk();
		}

		void Start()
		{
			RefreshSeed();
		}

		void Update()
		{

		}

		void OnDrawGizmos()
		{
			if (chunk != null && Settings != null)
			{
				for (int x = 0; x < Settings.ChunkSize.x; x++)
				{
					for (int y = 0; y < Settings.ChunkSize.y; y++)
					{
						Gizmos.color = chunk[x, y] == 0b1 ? Color.black : Color.white;
						Vector3 pos = new Vector3(-Settings.ChunkSize.x / 2 + x + 0.5f, -Settings.ChunkSize.y / 2 + y + 0.5f, 0);
						Gizmos.DrawCube(pos, Vector3.one);
					}
				}
			}
		}
		#endregion

		#region Helper methods
		public void InitializeChunk()
		{
			if (Settings == null) return;

			chunk = new byte[Settings.ChunkSize.x, Settings.ChunkSize.y];
			chunkTemp = new byte[Settings.ChunkSize.x, Settings.ChunkSize.y];
		}

		public void RefreshSeed()
		{
			if (Settings.UseRandomSeed)
				Settings.Seed = Time.time.ToString();

			random = new Random(Settings.Seed.GetHashCode());
		}

		public int GetNeighborCount(byte[,] chunk, int chunkX, int chunkY, int searchRadius, LevelFlags flag)
		{
			int count = 0;
			for (int x = chunkX - searchRadius; x <= chunkX + searchRadius; x++)
			{
				for (int y = chunkY - searchRadius; y <= chunkY + searchRadius; y++)
				{
					if ((x == chunkX && y == chunkY) ||
					    !IsInsideChunk(x, y))
					{
						continue;
					}

					if (HasFlag(chunk, x, y, flag))
					{
						count++;
					}
				}
			}

			return count;
		}
		#endregion

		#region Checks
		public bool IsInsideChunk(int x, int y)
		{
			return x >= 0 && x < Settings.ChunkSize.x && y >= 0 && y < Settings.ChunkSize.y;
		}

		public bool IsEdge(int x, int y)
		{
			return x == 0 || x == Settings.ChunkSize.x - 1 || y == 0 || y == Settings.ChunkSize.y - 1;
		}

		public bool HasFlag(byte[,] chunk, int x, int y, LevelFlags flag)
		{
			return (chunk[x, y] & flag.ToByte()) == flag.ToByte();
		}

		public bool IsWall(byte[,] chunk, int x, int y)
		{
			return HasFlag(chunk, x, y, LevelFlags.Wall);
		}
		#endregion

		#region Generic methods
		public delegate int CoordinateCountFunction(byte[,] chunk, int x, int y);
		public delegate bool CountCondition(int x);

		public void ApplyRules(CoordinateCountFunction coordinateCountFunction, bool ignoreEdges, params (CountCondition rule, LevelFlags value)[] rules)
		{
			Array.Copy(chunk, 0, chunkTemp, 0, chunk.Length);

			for (int x = 0; x < Settings.ChunkSize.x; x++)
			{
				for (int y = 0; y < Settings.ChunkSize.y; y++)
				{
					if (ignoreEdges && IsEdge(x, y))
					{
						continue;
					}

					int count = coordinateCountFunction(chunkTemp, x, y);

					foreach (var rulePair in rules)
					{
						if (rulePair.rule(count))
						{
							chunk[x, y] = rulePair.value.ToByte();
						}
					}
				}
			}
		}

		public delegate bool CoordinateCondition(int x, int y);

		public void ApplyRules(params (CoordinateCondition rule, LevelFlags value)[] rules)
		{
			for (int x = 0; x < Settings.ChunkSize.x; x++)
			{
				for (int y = 0; y < Settings.ChunkSize.y; y++)
				{
					foreach (var rulePair in rules)
					{
						if (rulePair.rule(x, y))
						{
							chunk[x, y] = rulePair.value.ToByte();
						}
					}
				}
			}
		}
		#endregion

		#region Fill functions
		private void GenericFill(CoordinateCondition rule, LevelFlags value)
		{
			for (int x = 0; x < Settings.ChunkSize.x; x++)
			{
				for (int y = 0; y < Settings.ChunkSize.y; y++)
				{
					if (rule(x, y))
					{
						chunk[x, y] = value.ToByte();
					}
				}
			}
		}

		public void Fill(LevelFlags fill)
		{
			GenericFill((x, y) => true, fill);
		}

		public void FillEdge(LevelFlags fill)
		{
			GenericFill(IsEdge, fill);
		}

		/// <param name="fillPercent">Value between 0 and 100.</param>
		public void FillRandom(LevelFlags fill, int fillPercent)
		{
			GenericFill((x, y) => (random.Next(0, 100) < fillPercent), fill);
		}
		#endregion

		public void Smooth(int neighborThreshold)
		{
			ApplyRules(
				(chunk, x, y) => GetNeighborCount(chunk, x, y, 1, LevelFlags.Wall),
				true,
				(neighborCount => neighborCount < neighborThreshold, LevelFlags.Empty),
				(neighborCount => neighborCount >= neighborThreshold, LevelFlags.Wall));
		}
	}
}