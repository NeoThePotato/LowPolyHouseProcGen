using System;
using Unity.Mathematics.Geometry;
using UnityEngine;

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

			public Input(AssetsCollection assets, MinMaxAABB boundingVolume)
			{
				this.assets = assets;
				this.boundingVolume = boundingVolume;
			}
		}
	}
}
