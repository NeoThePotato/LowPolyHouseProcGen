using UnityEngine;
using Unity.Mathematics.Geometry;
using random = Unity.Mathematics.Random;
using ProcGen.Collections;

namespace ProcGen
{
	public static partial class Generator
	{
		public static GameObject Generate(in Input input, random random, out INode<RoomData> rooms)
		{
			GenerateRooms(in input, ref random, out rooms);
            GenerateQuests(ref random, rooms);
            GenerateFurniture(in input, ref random, rooms.Leaves());
			return rooms.Value.parent.gameObject;
		}

		public readonly struct Input
		{
			public readonly AssetsCollection assets;
			public readonly MinMaxAABB boundingVolume;
			public readonly MinMaxAABB roomSize;

			public Input(AssetsCollection assets, MinMaxAABB boundingVolume, MinMaxAABB roomSize)
			{
				this.assets = assets;
				this.boundingVolume = boundingVolume;
				this.roomSize = roomSize;
			}
		}
	}
}
