using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using random = Unity.Mathematics.Random;

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
			const int UV_CHANNEL = 0;

			var volume = room.boundingVolume;
			CenterBounds(ref volume);
			Mesh mesh = new();
			List<Vector3> vertices = new();
			List<int> quads = new();
			List<Vector2> uv = new();

			var floor = CreateFloor(in volume, vertices, quads);
			var ceiling = CreateCeiling(in volume, vertices, quads);
			var walls = CreateWalls(vertices, quads);
			uv.AddRange(FloorCeilingUVs(vertices, Combine(floor, ceiling)));
			uv.AddRange(WallUVs(vertices, walls));
			mesh.subMeshCount = SUBMESH_COUNT;
			mesh.SetVertices(vertices);
			mesh.SetUVs(UV_CHANNEL, uv);
			//mesh.SetSubMesh(); // TODO Try this instead of the 3 lines below
			mesh.SetIndices(quads, floor.indexStart, floor.indexCount, floor.topology, FLOOR, false);
			mesh.SetIndices(quads, ceiling.indexStart, ceiling.indexCount, ceiling.topology, CEILING, false);
			mesh.SetIndices(quads, walls.indexStart, walls.indexCount, walls.topology, WALLS, false);
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();
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
			var left = Wall(vertices, quads, LEFT);
			var up = Wall(vertices, quads, UP);
			var right = Wall(vertices, quads, RIGHT);
			var down = Wall(vertices, quads, DOWN);
			return Combine(left, up, right, down);

			static SubMeshDescriptor Wall(List<Vector3> vertices, List<int> quads, int[] copy)
			{
				SubMeshDescriptor wall = new(indexStart: vertices.Count, indexCount: copy.Length, topology: MeshTopology.Quads);
				for (int i = 0; i < copy.Length; i++)
					vertices.Add(vertices[copy[i]]);
				for (int i = 0; i < wall.indexCount; i++)
					quads.Add(wall.indexStart + i);
				return wall;
			}
		}

		private static IEnumerable<Vector2> FloorCeilingUVs(List<Vector3> vertices, SubMeshDescriptor submesh)
		{
			return vertices.GetRange(submesh.indexStart, submesh.indexCount).Select(v => new Vector2(v.x, v.z));
		}

		private static IEnumerable<Vector2> WallUVs(List<Vector3> vertices, SubMeshDescriptor submesh)
		{
			return vertices.GetRange(submesh.indexStart, submesh.indexCount).Select(v => new Vector2(v.x + v.z, v.y)); // TODO Refine
		}

		private static SubMeshDescriptor Combine(SubMeshDescriptor d1, SubMeshDescriptor d2)
		{
			return new(math.min(d1.indexStart, d2.indexStart), d1.indexCount + d2.indexCount, topology: d1.topology);
		}

		private static SubMeshDescriptor Combine(SubMeshDescriptor d1, SubMeshDescriptor d2, SubMeshDescriptor d3, SubMeshDescriptor d4)
		{
			return new(
				math.min(math.min(d1.indexStart, d2.indexStart), math.min(d3.indexStart, d4.indexStart)),
				d1.indexCount + d2.indexCount + d3.indexCount + d4.indexCount,
				topology: d1.topology);
		}

		private static void CenterBounds(ref MinMaxAABB bounds)
		{
			float3 center = Extensions.Math.GetCenterRoot(in bounds);
			bounds.Min -= center;
			bounds.Max -= center;
		}
	}
}
