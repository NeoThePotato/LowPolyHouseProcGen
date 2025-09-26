using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using random = Unity.Mathematics.Random;

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
            var assets = input.assets;
            var roomType = room.roomType;

			const int objectCount = 5;
            const float minSafeDistance = 3f;
            const float cellDensityFactor = 0.2f; // Lower = more cells, higher = fewer cells
            Vector3 min = room.boundingVolume.Min + (room.boundingVolume.Extents * 0.2f);
            Vector3 max = room.boundingVolume.Max - (room.boundingVolume.Extents * 0.2f);

            // Increase grid resolution for more, smaller cells
            int gridX = Mathf.Max(2, Mathf.FloorToInt((max.x - min.x) / (minSafeDistance * cellDensityFactor)));
            int gridZ = Mathf.Max(2, Mathf.FloorToInt((max.z - min.z) / (minSafeDistance * cellDensityFactor)));

            bool[,] grid = new bool[gridX, gridZ];
            List<Vector3> placedPositions = new List<Vector3>(objectCount);

            float cellWidth = (max.x - min.x) / gridX;
            float cellDepth = (max.z - min.z) / gridZ;

            bool relaxAdjacency = (gridX * gridZ < objectCount * 2);

            for (int objIdx = 0; objIdx < objectCount; objIdx++)
            {
                List<(int x, int z)> validCells = new List<(int, int)>();
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

                if (validCells.Count == 0 && relaxAdjacency)
                {
                    for (int x = 0; x < gridX; x++)
                        for (int z = 0; z < gridZ; z++)
                            if (!grid[x, z])
                                validCells.Add((x, z));
                }

                if (validCells.Count == 0)
                    break;

                var cell = validCells[random.NextInt(0, validCells.Count)];
                grid[cell.x, cell.z] = true;

                Vector3 position = new Vector3(
                    min.x + (cell.x + 0.5f) * cellWidth,
                    0f,
                    min.z + (cell.z + 0.5f) * cellDepth
                );

                GameObject go = assets.Furniture[random.NextInt(0, assets.Furniture.Count)];
                go.tag = "Furniture";

                switch (roomType)
                {
                    case RoomType.Entrance:
                        go = assets.EntranceRoomObjects[random.NextInt(0, assets.EntranceRoomObjects.Count)];
                        break;
                    case RoomType.Exit:
                        go = assets.ExitRoomObjects[random.NextInt(0, assets.ExitRoomObjects.Count)];
                        break;
                    case RoomType.KeyRoom:
                        if (objIdx == 0)
                        {
                            go = assets.KeyRoomObjects[0];
                        }
                        else
                        {
                            go = assets.KeyRoomObjects[random.NextInt(1, assets.KeyRoomObjects.Count)];
                        }
                        break;
                    case RoomType.LockedRoom:
                        if (objIdx == 0)
                        {
                            go = assets.LockedRoomObjects[0];
                        }
                        else
                        {
                            go = assets.LockedRoomObjects[random.NextInt(1, assets.LockedRoomObjects.Count)];
                        }
                        break;
                    case RoomType.TreasureRoom:
                        go = assets.TreasureRoomObjects[random.NextInt(0, assets.TreasureRoomObjects.Count)];
                        break;
                    case RoomType.EnemyRoom:
                        go = assets.EnemyRoomObjects[random.NextInt(0, assets.EnemyRoomObjects.Count)];
                        break;
                    case RoomType.PuzzleRoom:
                        go = assets.PuzzleRoomObjects[random.NextInt(0, assets.PuzzleRoomObjects.Count)];
                        break;
                    case RoomType.None:
                        go = assets.Furniture[random.NextInt(0, assets.Furniture.Count)];
                        break;
                }

                Quaternion rotation = Quaternion.Euler(random.NextFloat3(0, 360f) * math.up());
                GameObject.Instantiate(go, position, rotation, room.parent);
                placedPositions.Add(position);
            }
        }
    }
}
