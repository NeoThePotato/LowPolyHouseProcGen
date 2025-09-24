using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using random = Unity.Mathematics.Random;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProcGen
{
	public static partial class Generator
	{
		const int FLOOR = 0, CEILING = 1, WALLS = 2, SUBMESH_COUNT = 3;

		static readonly int[]
			LEFT = { 0, 4, 5, 1 },
			UP = { 1, 5, 6, 2 },
			RIGHT = { 2, 6, 7, 3 },
			DOWN = { 3, 7, 4, 0 };

		public static void CreateRoomMeshes(in Input input, ref random random, IEnumerable<RoomData> rooms)
		{
			foreach (var room in rooms)
				CreateAndAssignMesh(room);
		}

		private static MeshFilter CreateAndAssignMesh(RoomData room)
		{
			var meshFilter = room.parent.gameObject.AddComponent<MeshFilter>();
			var mesh = meshFilter.mesh = CreateMesh(room);
			var renderer = room.parent.gameObject.AddComponent<MeshRenderer>(); // TODO Move this to another generator step (I.E Assign Materials)
			renderer.materials = new Material[mesh.subMeshCount];
			return meshFilter;
		}

		private static Mesh CreateMesh(RoomData room)
		{
			var volume = room.boundingVolume;
			CenterBounds(ref volume);
			Mesh mesh = new();
			List<Vector3> vertices = new();
			List<int> quads = new();

			var floor = CreateFloor(in volume, vertices, quads);
			var ceiling = CreateCeiling(in volume, vertices, quads);
			var walls = CreateWalls(vertices, quads);
			mesh.subMeshCount = SUBMESH_COUNT;
			mesh.SetVertices(vertices);
			mesh.SetIndices(quads, floor.indexStart, floor.indexCount, MeshTopology.Quads, FLOOR, true);
			mesh.SetIndices(quads, ceiling.indexStart, ceiling.indexCount, MeshTopology.Quads, CEILING, true);
			mesh.SetIndices(quads, walls.indexStart, walls.indexCount, MeshTopology.Quads, WALLS, true);
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			return mesh;
		}

		private static SubMeshDescriptor CreateFloor(in MinMaxAABB bounds, List<Vector3> vertices, List<int> quads)
		{
			SubMeshDescriptor floor = new(indexStart: vertices.Count, indexCount: 4, topology: MeshTopology.Quads);
			float
				y = bounds.Min.y,
				left = bounds.Min.x,
				right = bounds.Max.x,
				bottom = bounds.Min.z,
				top = bounds.Max.z;
			vertices.Add(new(left, y, bottom));
			vertices.Add(new(left, y, top));
			vertices.Add(new(right, y, top));
			vertices.Add(new(right, y, bottom));
			quads.Add(floor.indexStart);
			quads.Add(floor.indexStart + 1);
			quads.Add(floor.indexStart + 2);
			quads.Add(floor.indexStart + 3);
			return floor;
		}

		private static SubMeshDescriptor CreateCeiling(in MinMaxAABB bounds, List<Vector3> vertices, List<int> quads)
		{
			SubMeshDescriptor ceiling = new(indexStart: vertices.Count, indexCount: 4, topology: MeshTopology.Quads);
			float
				y = bounds.Max.y,
				left = bounds.Min.x,
				right = bounds.Max.x,
				bottom = bounds.Min.z,
				top = bounds.Max.z;
			vertices.Add(new(left, y, bottom));
			vertices.Add(new(left, y, top));
			vertices.Add(new(right, y, top));
			vertices.Add(new(right, y, bottom));
			quads.Add(ceiling.indexStart);
			quads.Add(ceiling.indexStart + 3);
			quads.Add(ceiling.indexStart + 2);
			quads.Add(ceiling.indexStart + 1);
			return ceiling;
		}

		private static SubMeshDescriptor CreateWalls(List<Vector3> vertices, List<int> quads)
		{
			SubMeshDescriptor walls = new(indexStart: vertices.Count, indexCount: 4*4, topology: MeshTopology.Quads);
			quads.AddRange(LEFT);
			quads.AddRange(UP);
			quads.AddRange(RIGHT);
			quads.AddRange(DOWN);
			return walls;
		}

		private static void CenterBounds(ref MinMaxAABB bounds)
		{
			float3 center = Extensions.Math.GetCenterRoot(in bounds);
			bounds.Min -= center;
			bounds.Max -= center;
		}
	}
}
