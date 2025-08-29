using Unity.Mathematics;
using Unity.Mathematics.Geometry;

namespace ProcGen.Collections
{
	public struct MinMax
	{
		public float min, max;

		public readonly float Range => max - min;

		public MinMax(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public static implicit operator float2(MinMax minMax) => new(minMax.min, minMax.max);

		public static implicit operator MinMax(float2 value) => new(value.x, value.y);
	}

	public struct MinMax2
	{
		public float2 min, max;

		public readonly float2 Range => max - min;

		public MinMax2(float2 min, float2 max)
		{
			this.min = min;
			this.max = max;
		}

		public static implicit operator float4(in MinMax2 minMax) => new(minMax.min, minMax.max);

		public static implicit operator MinMax2(in float4 value) => new(value.xy, value.zw);

		public static implicit operator MinMax2(in MinMaxAABB aabb) => new(aabb.Min.xy, aabb.Max.xy);
	}

	public static class MinMaxExtensions
	{
		public static float Random(this ref Random random, MinMax range) => random.NextFloat(range.min, range.max);
		
		public static float2 Random(this ref Random random, in MinMax2 range) => random.NextFloat2(range.min, range.max);

		public static MinMax2 MinMaxXY(this in MinMaxAABB aabb) => aabb;

		public static MinMax2 MinMaxXZ(this in MinMaxAABB aabb) => new(aabb.Min.xz, aabb.Max.xz);

		public static MinMax2 MinMaxYZ(this in MinMaxAABB aabb) => new(aabb.Min.yz, aabb.Max.yz);
	}
}
