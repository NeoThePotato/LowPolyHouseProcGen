using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using random = Unity.Mathematics.Random;

namespace ProcGen
{
	public static partial class Generator
	{
		public static void CreateRoomMeshes(in Input input, ref random random, IEnumerable<RoomData> rooms)
		{
			foreach (var room in rooms)
				CreateAndAssignMesh(room);
		}

		private static MeshFilter CreateAndAssignMesh(RoomData room)
		{
			var mesh = CreateMesh(room);
			var meshFilter = room.parent.gameObject.AddComponent<MeshFilter>();
			room.parent.gameObject.AddComponent<MeshRenderer>();
			meshFilter.mesh = mesh;
			return meshFilter;
		}

		private static Mesh CreateMesh(RoomData room)
		{
			const int FLOOR = 0, CEILING = 1, WALLS = 2;

			var volume = room.boundingVolume;
			CenterBounds(ref volume);
			Mesh mesh = new();
			List<Vector3> vertices = new();
			List<int> quads = new();

			var floor = CreateFloor(in volume, vertices, quads);
			var ceiling = CreateCeiling(in volume, vertices, quads);
			mesh.subMeshCount = 2;
			mesh.SetVertices(vertices);
			mesh.SetIndices(quads, floor.indexStart, floor.indexCount, MeshTopology.Quads, FLOOR, true);
			mesh.SetIndices(quads, ceiling.indexStart, ceiling.indexCount, MeshTopology.Quads, CEILING, true);
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

		private static void CenterBounds(ref MinMaxAABB bounds)
		{
			float3 center = Extensions.Math.GetCenterRoot(in bounds);
			bounds.Min -= center;
			bounds.Max -= center;
		}
	}
}
