using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using NaughtyAttributes;
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

		public void SetFlag(byte[,] chunk, int x, int y, LevelFlags flag)
		{
			if (flag == LevelFlags.Empty)
			{
				chunk[x, y] = flag.ToByte();
			}
			else
			{
				chunk[x, y] |= flag.ToByte();
			}
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

		/// <summary>
		/// Checks around a coordinate in expanding concentric rings for a tile with the given flag.
		/// </summary>
		public Vector2Int GetNearestNeighbor(byte[,] chunk, int chunkX, int chunkY, int maxSearchRadius, LevelFlags search)
		{
			for (int i = 1; i <= maxSearchRadius; i++)
			{
				var neighbors = SearchConcentric(chunk, chunkX, chunkY, i, search);

				if (neighbors != null && neighbors.Count > 0)
				{
					// TODO fix to select without bias if two or more equidistant neighbors are found
					return neighbors[0];
				}
			}

			return Constants.NullVectorInt;
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
			if (flag == LevelFlags.Empty)
			{
				return (chunk[x, y] | flag.ToByte()) == flag.ToByte();
			}
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

		public List<Vector2Int> SearchConcentric(byte[,] chunk, int chunkX, int chunkY, int radius, LevelFlags search)
		{
			List<Vector2Int> results = new List<Vector2Int>();
			int x, y;
			for (x = chunkX - radius; x <= chunkX + radius; x++)
			{
				if (x == chunkX - radius || x == chunkX + radius)
				{
					for (y = chunkY - radius; y <= chunkY + radius; y++)
					{
						if (IsInsideChunk(x, y) && HasFlag(chunk, x, y, search))
						{
							results.Add(new Vector2Int(x, y));
						}
					}
				}
				else
				{
					y = chunkY - radius;
					if (IsInsideChunk(x, y) && HasFlag(chunk, x, y, search))
					{
						results.Add(new Vector2Int(x, y));
					}

					y = chunkY + radius;
					if (IsInsideChunk(x, y) && HasFlag(chunk, x, y, search))
					{
						results.Add(new Vector2Int(x, y));
					}
				}
			}

			return results;
		}

		private static readonly List<Vector2Int> _directions = new List<Vector2Int> { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
		public List<Vector2Int> SearchCardinal(byte[,] chunk, int chunkX, int chunkY, int radius, LevelFlags search)
		{
			List<Vector2Int> results = new List<Vector2Int>();
			
			var pos = new Vector2Int(chunkX, chunkY);
			foreach (var dir in _directions)
			{
				var current = pos + (dir * radius);
				if (IsInsideChunk(current.x, current.y) &&
				    HasFlag(chunk, current.x, current.y, search))
				{
					results.Add(current);
				}
			}

			return results;
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

		public void ClearBetween(int x1, int y1, int x2, int y2)
		{
			throw new NotImplementedException();

			for (int x = x1; x <= x2; x++)
			{
				int y;

				if (x == x2 && y == y2)
				{

				}
			}
		}

		public void Flood(int startX, int startY, LevelFlags search, LevelFlags flood)
		{
			Queue<Vector2Int> openSet = new Queue<Vector2Int>();
			HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

			openSet.Enqueue(new Vector2Int(startX, startY));
			while (openSet.Count > 0)
			{
				var current = openSet.Dequeue();

				if (HasFlag(chunk, current.x, current.y, search) &&
				    !closedSet.Contains(current))
				{
					SetFlag(chunk, current.x, current.y, flood);
					//var neighbors = SearchConcentric(chunk, current.x, current.y, 1, search);
					var neighbors = SearchCardinal(chunk, current.x, current.y, 1, search);
					neighbors.ForEach(n => openSet.Enqueue(n));
				}

				closedSet.Add(current);
			}
		}

		public void ConnectRooms(int startX, int startY)
		{
			// TODO automatically select an empty starting point
			Flood(startX, startY, LevelFlags.Empty, LevelFlags.Flood);
		}
	}
}