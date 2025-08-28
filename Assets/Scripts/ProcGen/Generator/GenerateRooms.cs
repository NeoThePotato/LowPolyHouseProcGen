using System;
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
			throw new NotImplementedException(); // TODO Implement
		}

		public readonly struct RoomData
		{
			public readonly MinMaxAABB boundingVolume;
			public readonly Transform parent;

			public RoomData(in MinMaxAABB boundingVolume, Transform parent)
			{
				this.boundingVolume = boundingVolume;
				this.parent = parent;
			}

			public static implicit operator MinMaxAABB(in RoomData roomData) => roomData.boundingVolume;

			public static implicit operator Transform(in RoomData roomData) => roomData.parent;
		}
	}
}
