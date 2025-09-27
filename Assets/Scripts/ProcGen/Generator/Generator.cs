using System;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using random = Unity.Mathematics.Random;
using ProcGen.Collections;

namespace ProcGen
{
	public static partial class Generator
	{
		public static GameObject Generate(in Input input, random random, out INode<RoomData> tree, out RoomData[] rooms, out RoomData.Connection[] connections)
		{
			GenerateRooms(in input, ref random, out tree);
			rooms = tree.Leaves().Select(n => n.Value).ToArray();
			ConnectRooms(in input, ref random, rooms, out connections);
			ShrinkRooms(in input, rooms, connections);
			CollapseConnections(in input, ref random, connections);
			CreateRoomMeshes(rooms);
			GenerateDoors(in input, ref random, connections);
			GenerateQuests(ref random, tree);
			CreateRoomRenderers(in input, ref random, rooms);
			GenerateFurniture(in input, ref random, rooms);
			return tree.Value.transform.gameObject;
		}

		[Serializable]
		public struct Input
		{
			public AssetsCollection assets;
			public MinMaxAABB boundingVolume;
			public MinMaxAABB roomSize;
			public float3 connectionSize;

			public Input(AssetsCollection assets, MinMaxAABB boundingVolume, MinMaxAABB roomSize, float3 connectionSize)
			{
				this.assets = assets;
				this.boundingVolume = boundingVolume;
				this.roomSize = roomSize;
				this.connectionSize = connectionSize;
			}
		}
	}
}
