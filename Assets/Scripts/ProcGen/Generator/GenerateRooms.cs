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
			const string ROOT = "Root";

			BinaryTree<RoomData> tree = new(new(input.boundingVolume, new GameObject(ROOT).transform));
			BSP.SplitRecursive(in input, ref random, tree);
			rooms = tree.Root;
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
