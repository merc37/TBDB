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
	public abstract class LevelGenerator : MonoBehaviour
	{
		public Vector2Int ChunkSize;
		public bool UseRandomSeed;
		[EnableIf("SeedEnabled")]
		public string Seed;
		public bool SeedEnabled() { return !UseRandomSeed; }

		public byte[,] Chunk { get; private set; }

		protected Random random;
		protected byte[,] chunkTemp;
		
		#region Monobehavior
		void Awake()
		{
			InitializeChunk();
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
			if (Chunk != null)
			{
				for (int x = 0; x < ChunkSize.x; x++)
				{
					for (int y = 0; y < ChunkSize.y; y++)
					{
						Gizmos.color = Chunk[x, y] == 0b1 ? Color.black : Color.white;
						Vector3 pos = new Vector3(-ChunkSize.x / 2 + x + 0.5f, -ChunkSize.y / 2 + y + 0.5f, 0);
						Gizmos.DrawCube(pos, Vector3.one);
					}
				}
			}
		}
		#endregion

		#region Helper methods
		protected void InitializeChunk()
		{
			Chunk = new byte[ChunkSize.x, ChunkSize.y];
			chunkTemp = new byte[ChunkSize.x, ChunkSize.y];
		}

		protected void RefreshSeed()
		{
			if (UseRandomSeed)
				Seed = Time.time.ToString();

			random = new Random(Seed.GetHashCode());
		}

		protected bool IsInsideChunk(int x, int y)
		{
			return x >= 0 && x < ChunkSize.x && y >= 0 && y < ChunkSize.y;
		}

		protected bool IsEdge(int x, int y)
		{
			return x == 0 || x == ChunkSize.x - 1 || y == 0 || y == ChunkSize.y - 1;
		}

		protected bool IsWall(byte[,] chunk, int x, int y)
		{
			return (chunk[x, y] & LevelFlags.Wall.ToByte()) == LevelFlags.Wall.ToByte();
		}

		protected int GetNeighboringWallCount(byte[,] chunk, int chunkX, int chunkY)
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
			Debug.Log(wallCount);
			return wallCount;
		}
		#endregion

		/// <param name="fillPercent">Value between 0 and 100.</param>
		protected void FillRandom(int fillPercent)
		{
			for (int x = 0; x < ChunkSize.x; x++)
			{
				for (int y = 0; y < ChunkSize.y; y++)
				{
					if (IsEdge(x, y))
					{
						Chunk[x, y] = LevelFlags.Wall.ToByte();
					}
					else
					{
						Chunk[x, y] = (byte)(random.Next(0, 100) < fillPercent ? LevelFlags.Wall.ToByte() : LevelFlags.Empty.ToByte());
					}
				}
			}
		}

		public delegate bool IntThreshold(int x);

		protected void SmoothMap(IntThreshold wallCondition, IntThreshold emptyCondition)
		{
			Array.Copy(Chunk, 0, chunkTemp, 0, Chunk.Length);

			for (int x = 0; x < ChunkSize.x; x++)
			{
				for (int y = 0; y < ChunkSize.y; y++)
				{
					int neighboringWallCount = GetNeighboringWallCount(chunkTemp, x, y);

					if (wallCondition(neighboringWallCount) || IsEdge(x, y))
					{
						Chunk[x, y] = LevelFlags.Wall.ToByte();
					}
					else if (emptyCondition(neighboringWallCount))
					{
						Chunk[x, y] = LevelFlags.Empty.ToByte();
					}
				}
			}
		}
	}
}