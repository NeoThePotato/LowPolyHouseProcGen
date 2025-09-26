using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics.Geometry;
using random = Unity.Mathematics.Random;
using ProcGen.Collections;
using System;

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
			public HashSet<Connection> connections;
			public RoomType roomType = RoomType.None;

			public RoomData(in MinMaxAABB boundingVolume, Transform parent)
			{
				this.boundingVolume = boundingVolume;
				this.parent = parent;
				connections = new();
			}

			public static implicit operator MinMaxAABB(in RoomData roomData) => roomData.boundingVolume;

			public static implicit operator Transform(in RoomData roomData) => roomData.parent;

			public struct Connection : IEquatable<Connection>
			{
				public RoomData room1, room2;
				public MinMaxAABB volume;

				public readonly bool Equals(Connection other)
				{
					return
						(room1 == other.room1 && room2 == other.room2)
						||
						(room1 == other.room2 && room2 == other.room1);
				}
			}
		}
	}
}
