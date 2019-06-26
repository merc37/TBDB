using System;
using System.Collections;
using System.Collections.Generic;
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

		#region Monobehavior
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

		public bool IsInsideChunk(int x, int y)
		{
			return x >= 0 && x < Settings.ChunkSize.x && y >= 0 && y < Settings.ChunkSize.y;
		}

		public bool IsEdge(int x, int y)
		{
			return x == 0 || x == Settings.ChunkSize.x - 1 || y == 0 || y == Settings.ChunkSize.y - 1;
		}

		public bool IsWall(byte[,] chunk, int x, int y)
		{
			return (chunk[x, y] & LevelFlags.Wall.ToByte()) == LevelFlags.Wall.ToByte();
		}

		public int GetNeighboringWallCount(byte[,] chunk, int chunkX, int chunkY)
		{
			int wallCount = 0;
			if (chunk != null)
			{
				for (int x = chunkX - 1; x <= chunkX + 1; x++)
				{
					for (int y = chunkY - 1; y <= chunkY + 1; y++)
					{
						if ((x == chunkX && y == chunkY) ||
						    !IsInsideChunk(x, y))
						{
							continue;
						}

						if (/*IsEdge(x, y) ||*/IsWall(chunk, x, y))
						{
							wallCount++;
						}
					}
				}
			}

			return wallCount;
		}
		#endregion
		
		/// <param name="fillPercent">Value between 0 and 100.</param>
		public void FillRandom(int fillPercent)
		{
			for (int x = 0; x < Settings.ChunkSize.x; x++)
			{
				for (int y = 0; y < Settings.ChunkSize.y; y++)
				{
					if (IsEdge(x, y))
					{
						chunk[x, y] = LevelFlags.Wall.ToByte();
					}
					else
					{
						chunk[x, y] = (byte)(random.Next(0, 100) < fillPercent ? LevelFlags.Wall.ToByte() : LevelFlags.Empty.ToByte());
					}
				}
			}
		}

		public delegate bool IntThreshold(int x);

		public void SmoothMap(IntThreshold wallCondition, IntThreshold emptyCondition)
		{
			Array.Copy(chunk, 0, chunkTemp, 0, chunk.Length);

			for (int x = 0; x < Settings.ChunkSize.x; x++)
			{
				for (int y = 0; y < Settings.ChunkSize.y; y++)
				{
					int neighboringWallCount = GetNeighboringWallCount(chunkTemp, x, y);

					if (wallCondition(neighboringWallCount) || IsEdge(x, y))
					{
						chunk[x, y] = LevelFlags.Wall.ToByte();
					}
					else if (emptyCondition(neighboringWallCount))
					{
						chunk[x, y] = LevelFlags.Empty.ToByte();
					}
				}
			}
		}
	}
}