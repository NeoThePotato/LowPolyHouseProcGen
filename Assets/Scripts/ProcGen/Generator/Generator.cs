using System;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;
using EMath = Extensions.Math;

namespace ProcGen
{
	public static partial class Generator
	{
		public static GameObject Generate(in Input input)
		{
			GenerateWalls(in input);
			GenerateFurniture(in input);
			GenerateQuests(in input);
			throw new NotImplementedException(); // TODO Implement
		}

		public readonly struct Input
		{
			public readonly AssetsCollection assets;
			public readonly MinMaxAABB boundingVolume;
			public readonly float2 minRoomSize, maxRoomSize;

			public Input(AssetsCollection assets, MinMaxAABB boundingVolume, float2 minRoomSize, float2 maxRoomSize)
			{
				this.assets = assets;
				this.boundingVolume = boundingVolume;
				EMath.EnsureSize(ref minRoomSize.x, ref maxRoomSize.x);
				EMath.EnsureSize(ref minRoomSize.y, ref maxRoomSize.y);
				this.minRoomSize = minRoomSize;
				this.maxRoomSize = maxRoomSize;
			}
		}
	}
}
