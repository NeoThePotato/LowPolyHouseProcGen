using System;
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
			public readonly Transform transform;
			public List<int> connections;
			public RoomType roomType = RoomType.None;

			public RoomData(in MinMaxAABB boundingVolume, Transform transform)
			{
				this.boundingVolume = boundingVolume;
				this.transform = transform;
				connections = new();
			}

			public static implicit operator MinMaxAABB(in RoomData roomData) => roomData.boundingVolume;

			public static implicit operator Transform(in RoomData roomData) => roomData.transform;

			public struct Connection : IEquatable<Connection>
			{
				public RoomData room1, room2;
				public MinMaxAABB volume;

				public readonly bool XAligned => volume.Min.z == volume.Max.z;

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
