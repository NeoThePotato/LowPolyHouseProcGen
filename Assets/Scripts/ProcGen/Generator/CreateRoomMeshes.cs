using System;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using Unity.Collections;

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

		public static void CreateRoomMeshes(RoomData[] rooms)
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
			const MeshUpdateFlags DONT_UPDATE = MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds;

			var volume = room.boundingVolume;
			CenterBounds(ref volume);
			Mesh mesh = new();
			using NativeList<Vector3> vertices = new(Allocator.Temp);
			using NativeList<ushort> quads = new(Allocator.Temp);
			using NativeList<Vector2> uv = new(Allocator.Temp);
			using NativeArray<SubMeshDescriptor> subMeshes = new(SUBMESH_COUNT, Allocator.Temp);

			// Create Vertices & Quads
			ref SubMeshDescriptor floor = ref subMeshes.AsSpan()[FLOOR];
			ref SubMeshDescriptor ceiling = ref subMeshes.AsSpan()[CEILING];
			ref SubMeshDescriptor walls = ref subMeshes.AsSpan()[WALLS];
			floor = CreateFloor(in volume, vertices, quads);
			ceiling = CreateCeiling(in volume, vertices, quads);
			walls = CreateWalls(vertices, quads);

			// Create UVs
			FloorCeilingUVs(SpanFromDescriptor(vertices, Combine(floor, ceiling)), uv);
			WallUVs(SpanFromDescriptor(vertices, walls), uv);

			// Set Mesh data
			mesh.SetVertices(vertices.AsArray(), 0, vertices.Length, DONT_UPDATE);
			mesh.SetIndexBufferParams(quads.Length, IndexFormat.UInt16);
			mesh.SetIndexBufferData(quads.AsArray(), 0, 0, quads.Length, DONT_UPDATE);
			mesh.SetUVs(UV_CHANNEL, uv.AsArray(), 0, uv.Length, DONT_UPDATE);
			mesh.SetSubMeshes(subMeshes);

			// Validate mesh
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();
			return mesh;
		}

		private static SubMeshDescriptor CreateFloor(in MinMaxAABB bounds, NativeList<Vector3> vertices, NativeList<ushort> quads)
		{
			SubMeshDescriptor floor = new(indexStart: vertices.Length, indexCount: 4, topology: MeshTopology.Quads);
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
			quads.Add((ushort)floor.indexStart);
			quads.Add((ushort)(floor.indexStart + 1));
			quads.Add((ushort)(floor.indexStart + 2));
			quads.Add((ushort)(floor.indexStart + 3));
			return floor;
		}

		private static SubMeshDescriptor CreateCeiling(in MinMaxAABB bounds, NativeList<Vector3> vertices, NativeList<ushort> quads)
		{
			SubMeshDescriptor ceiling = new(indexStart: vertices.Length, indexCount: 4, topology: MeshTopology.Quads);
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
			quads.Add((ushort)ceiling.indexStart);
			quads.Add((ushort)(ceiling.indexStart + 3));
			quads.Add((ushort)(ceiling.indexStart + 2));
			quads.Add((ushort)(ceiling.indexStart + 1));
			return ceiling;
		}

		private static SubMeshDescriptor CreateWalls(NativeList<Vector3> vertices, NativeList<ushort> quads)
		{
			var left = Wall(vertices, quads, LEFT);
			var up = Wall(vertices, quads, UP);
			var right = Wall(vertices, quads, RIGHT);
			var down = Wall(vertices, quads, DOWN);
			return Combine(left, up, right, down);

			static SubMeshDescriptor Wall(NativeList<Vector3> vertices, NativeList<ushort> quads, int[] copy)
			{
				SubMeshDescriptor wall = new(indexStart: vertices.Length, indexCount: copy.Length, topology: MeshTopology.Quads);
				for (int i = 0; i < copy.Length; i++)
					vertices.Add(vertices[copy[i]]);
				for (int i = 0; i < wall.indexCount; i++)
					quads.Add((ushort)(wall.indexStart + i));
				return wall;
			}
		}

		private static void FloorCeilingUVs(ReadOnlySpan<Vector3> vertices, NativeList<Vector2> uv)
		{
			for (int i = 0; i < vertices.Length; i++)
				uv.Add(ToUV(vertices[i]));

			static Vector2 ToUV(Vector3 vertex) => new(vertex.x, vertex.z);
		}

		private static void WallUVs(ReadOnlySpan<Vector3> vertices, NativeList<Vector2> uv)
		{
			for (int i = 0; i < vertices.Length; i++)
				uv.Add(ToUV(vertices[i]));

			static Vector2 ToUV(Vector3 vertex) => new(vertex.x + vertex.z, vertex.y);
		}

		private static Span<T> SpanFromDescriptor<T>(NativeList<T> source, SubMeshDescriptor descriptor) where T : unmanaged
		{
			return source.AsArray().AsSpan().Slice(descriptor.indexStart, descriptor.indexCount);
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
