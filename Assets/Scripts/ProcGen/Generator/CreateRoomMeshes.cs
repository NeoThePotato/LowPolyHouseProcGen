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
			const int FLOOR = 0, WALLS = 1, CEILING = 2;

			var volume = room.boundingVolume;
			CenterBounds(ref volume);
			Mesh mesh = new();
			List<Vector3> vertices = new();
			List<int> quads = new();

			var floor = CreateFloor(in volume, vertices, quads);
			mesh.SetVertices(vertices);
			mesh.SetIndices(topology: MeshTopology.Quads, indices: quads, submesh: FLOOR, indicesStart: floor.indexStart, indicesLength: floor.indexCount);
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			return mesh;
		}

		private static SubMeshDescriptor CreateFloor(in MinMaxAABB bounds, List<Vector3> vertices, List<int> quads)
		{
			SubMeshDescriptor floor = new(indexStart: vertices.Count, indexCount: 4, topology: MeshTopology.Quads);
			vertices.Add(bounds.Min);
			vertices.Add(new float3(xy: bounds.Min.xy, z: bounds.Max.z));
			vertices.Add(new float3(x: bounds.Max.x, y: bounds.Min.y, z: bounds.Max.z));
			vertices.Add(new float3(x: bounds.Max.x, yz: bounds.Min.yz));
			quads.Add(floor.indexStart);
			quads.Add(floor.indexStart+1);
			quads.Add(floor.indexStart+2);
			quads.Add(floor.indexStart+3);
			return floor;
		}

		private static void CenterBounds(ref MinMaxAABB bounds)
		{
			float3 center = Extensions.Math.GetCenterRoot(in bounds);
			bounds.Min -= center;
			bounds.Max -= center;
		}
	}
}
