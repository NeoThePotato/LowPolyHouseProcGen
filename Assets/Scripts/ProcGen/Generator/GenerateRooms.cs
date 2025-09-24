using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics.Geometry;
using random = Unity.Mathematics.Random;
using ProcGen.Collections;

namespace ProcGen
{
	public static partial class Generator
	{
		public static void GenerateRooms(in Input input, ref random random, out INode<RoomData> rooms)
		{
			rooms = new BinaryTree<RoomData>(new(input.boundingVolume, new GameObject().transform));
			BSP.SplitRecursive(in input, ref random, rooms);
		}

		public class RoomData
		{
			public MinMaxAABB boundingVolume;
			public readonly Transform parent;
			public HashSet<RoomData> connections;
			public RoomType roomType = RoomType.None;

			public RoomData(in MinMaxAABB boundingVolume, Transform parent)
			{
				this.boundingVolume = boundingVolume;
				this.parent = parent;
				connections = new();
			}

			public static implicit operator MinMaxAABB(in RoomData roomData) => roomData.boundingVolume;

			public static implicit operator Transform(in RoomData roomData) => roomData.parent;
		}
	}
}
