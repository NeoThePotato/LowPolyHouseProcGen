using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;
using random = Unity.Mathematics.Random;
using EMath = Extensions.Math;
using ProcGen.Collections;

namespace ProcGen
{
	public static partial class Generator
	{
		public static GameObject Generate(in Input input, random random, out INode<RoomData> rooms)
		{
			GenerateRooms(in input, ref random, out rooms);
			GenerateFurniture(in input, ref random, rooms.Leaves());
			GenerateQuests(ref random, rooms);
			return rooms.Value.parent.gameObject;
		}

		public readonly struct Input
		{
			public readonly AssetsCollection assets;
			public readonly MinMaxAABB boundingVolume;
			public readonly MinMax2 roomSize;

			public Input(AssetsCollection assets, MinMaxAABB boundingVolume, float2 minRoomSize, float2 maxRoomSize)
			{
				this.assets = assets;
				this.boundingVolume = boundingVolume;
				EMath.EnsureSize(ref minRoomSize.x, ref maxRoomSize.x);
				EMath.EnsureSize(ref minRoomSize.y, ref maxRoomSize.y);
				roomSize = new(minRoomSize, maxRoomSize);
			}
		}
	}
}
