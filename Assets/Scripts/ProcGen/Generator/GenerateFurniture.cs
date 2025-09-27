using System;
using UnityEngine;
using Unity.Mathematics;
using random = Unity.Mathematics.Random;
using static Extensions.Random;
using Unity.Collections;

namespace ProcGen
{
	public static partial class Generator
	{
        /// <summary>
        /// Generate furniture for all <paramref name="rooms"/>.
        /// </summary>d
        /// <param name="input">Generation input.</param>
        /// <param name="random">Random number generator.</param>
        /// <param name="rooms">Rooms to furnish.</param>
        public static void GenerateFurniture(in Input input, ref random random, RoomData[] rooms)
		{
			foreach (var room in rooms)
                PlaceGrammarFurniture(in input, room, ref random);
		}

        public static void PlaceGrammarFurniture(in Input input, in RoomData room, ref random random)
        {
            const string FURNITURE_TAG = "Furniture";

			var assets = input.assets;
            var roomType = room.roomType;

			const int objectCount = 5;
            const float minSafeDistance = 3f;
            const float cellDensityFactor = 0.2f; // Lower = more cells, higher = fewer cells
            Vector3 min = room.boundingVolume.Min + (room.boundingVolume.Extents * 0.2f);
            Vector3 max = room.boundingVolume.Max - (room.boundingVolume.Extents * 0.2f);

            // Increase grid resolution for more, smaller cells
            int gridX = math.max(2, (int)math.floor((max.x - min.x) / (minSafeDistance * cellDensityFactor)));
            int gridZ = math.max(2, (int)math.floor((max.z - min.z) / (minSafeDistance * cellDensityFactor)));

            bool[,] grid = new bool[gridX, gridZ];

            float cellWidth = (max.x - min.x) / gridX;
            float cellDepth = (max.z - min.z) / gridZ;

            bool relaxAdjacency = (gridX * gridZ < objectCount * 2);

			using NativeList<FloorPosition> validCells = new(Allocator.Temp);

            for (int objIdx = 0; objIdx < objectCount; objIdx++)
            {
                validCells.Clear();
				for (int x = 0; x < gridX; x++)
                {
                    for (int z = 0; z < gridZ; z++)
                    {
                        if (grid[x, z]) continue;
                        bool tooClose = false;
                        // Check for any occupied cell within 2 cells away (Chebyshev distance)
                        for (int dx = -2; dx <= 2; dx++)
                        {
                            for (int dz = -2; dz <= 2; dz++)
                            {
                                if (dx == 0 && dz == 0) continue;
                                int nx = x + dx;
                                int nz = z + dz;
                                if (nx >= 0 && nx < gridX && nz >= 0 && nz < gridZ && grid[nx, nz])
                                {
                                    tooClose = true;
                                    break;
                                }
                            }
                            if (tooClose) break;
                        }
                        if (!tooClose)
                            validCells.Add((x, z));
                    }
                }

                if (validCells.Length == 0 && relaxAdjacency)
                {
                    for (int x = 0; x < gridX; x++)
                        for (int z = 0; z < gridZ; z++)
                            if (!grid[x, z])
                                validCells.Add((x, z));
                }

                if (validCells.Length == 0)
                    break;

                var cell = validCells.GetRandom<NativeList<FloorPosition>, FloorPosition>(ref random);
                grid[cell.x, cell.z] = true;

                Vector3 position = new(
                    min.x + (cell.x + 0.5f) * cellWidth,
					room.boundingVolume.Min.y,
                    min.z + (cell.z + 0.5f) * cellDepth
                );

                var collection = GetCollection(assets, roomType);
                if (roomType == RoomType.KeyRoom || roomType == RoomType.LockedRoom)
                    collection = collection[1..];
                var go = collection.GetRandom(ref random);

                var rotation = quaternion.Euler(0f, random.NextFloat(360f), 0f);
                go = GameObject.Instantiate(go, position, rotation, room.parent);
                go.tag = FURNITURE_TAG;
            }
		}

		static ReadOnlyMemory<GameObject> GetCollection(AssetsCollection collection, RoomType type)
		{
			return type switch
			{
				RoomType.Entrance => collection.EntranceRoomObjects,
				RoomType.None => collection.Furniture,
				RoomType.Exit => collection.ExitRoomObjects,
				RoomType.KeyRoom => collection.KeyRoomObjects,
				RoomType.LockedRoom => collection.LockedRoomObjects,
				RoomType.TreasureRoom => collection.TreasureRoomObjects,
				RoomType.EnemyRoom => collection.EnemyRoomObjects,
				RoomType.PuzzleRoom => collection.PuzzleRoomObjects,
				_ => collection.Furniture
			};
		}

        private struct FloorPosition
        {
            public int x, z;

            public FloorPosition(int x, int z)
            {
                this.x = x;
                this.z = z;
            }

            public static implicit operator FloorPosition((int x, int z) tuple) => new(tuple.x, tuple.z);
        }
	}
}
