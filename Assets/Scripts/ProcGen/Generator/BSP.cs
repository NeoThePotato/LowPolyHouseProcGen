using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using random = Unity.Mathematics.Random;
using ProcGen.Collections;
using EMath = Extensions.Math;

namespace ProcGen
{
	public static partial class Generator
	{
		public static class BSP
		{
			public static void SplitRecursive(in Input input, ref random random, INode<RoomData> root)
			{
				if (!TrySplitSingle(input, ref random, root))
					return;
				SplitRecursive(input, ref random, root.Left);
				SplitRecursive(input, ref random, root.Right);	
			}

			public static bool TrySplitSingle(in Input input, ref random random, INode<RoomData> toSplit)
			{
				const string LEFT = "Left", RIGHT = "Right", BACK = "Back", FWD = "Forward", DOWN = "Down", UP = "Up";

				var roomToSplit = toSplit.Value;
				ref readonly var boundingVolume = ref roomToSplit.boundingVolume;
				var canSplit = CanSplit(input.roomSize.Min, boundingVolume);
				if (!canSplit)
					return false;
				SplitAlongRandomAxis(in boundingVolume, in canSplit, ref random, out var boundsLeft, out var boundsRight, out var horizontal, out var vertical);
				toSplit.Left = CreateNode(roomToSplit, in boundsLeft, vertical ? DOWN : (horizontal ? LEFT : BACK));
				toSplit.Right = CreateNode(roomToSplit, in boundsRight, vertical ? UP : (horizontal ? RIGHT : FWD));
				return true;
			}

			private static void SplitAlongRandomAxis(in MinMaxAABB boundingVolume, in SplitRanges split, ref random random, out MinMaxAABB left, out MinMaxAABB right, out bool horizontal, out bool vertical)
			{
				right = left = boundingVolume;
				// Always prioritize splitting vertically to create floors first
				if (vertical = split.PossibleDirections.y)
				{
					horizontal = false;
					left.Max.y = right.Min.y = random.Random(split.Y);
					return;
				}
				if (horizontal = Horizontal(split.PossibleDirections, ref random))
					left.Max.x = right.Min.x = random.Random(split.X);
				else
					left.Max.z = right.Min.z = random.Random(split.Z);

				static bool Horizontal(bool3 possibleDirections, ref random random)
				{
					if (math.all(possibleDirections.xz))
						return random.NextBool();
					else
						return possibleDirections.x;
				}
			}

			private static SplitRanges CanSplit(in float3 minRoomSize, in MinMaxAABB bounds)
			{
				return new(bounds.Min + minRoomSize, bounds.Max - minRoomSize);
			}

			private static BinaryTree<RoomData>.Node CreateNode(Transform parent, in MinMaxAABB bounds, string name)
			{
				var child = new GameObject(name).transform;
				child.SetParent(parent);
				child.position = EMath.GetCenterRoot(bounds);
				return new()
				{
					Value = new(bounds, child)
				};
			}

			[StructLayout(LayoutKind.Explicit)]
			private struct SplitRanges
			{
				[FieldOffset(0)]
				public MinMaxAABB ranges;
				[FieldOffset(0)] // Alias of ranges.Min
				public float3 Min;
				[FieldOffset(3 * sizeof(float))] // Alias of ranges.Max
				public float3 Max;

				public readonly MinMax X => new(Min.x, Max.x);
				public readonly MinMax Y => new(Min.y, Max.y);
				public readonly MinMax Z => new(Min.z, Max.z);
				public readonly bool3 PossibleDirections => ranges.Extents >= 0f;

				public SplitRanges(float3 min, float3 max)
				{
					ranges = default;
					Min = min;
					Max = max;
				}

				public static implicit operator SplitRanges(in MinMaxAABB ranges) => new() { ranges = ranges };

				public static implicit operator MinMaxAABB(in SplitRanges split) => split.ranges;

				public static implicit operator bool(in SplitRanges split) => math.any(split.PossibleDirections);
			}
		}
	}
}
