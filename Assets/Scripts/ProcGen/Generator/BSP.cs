using UnityEngine;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using random = Unity.Mathematics.Random;
using ProcGen.Collections;

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
				const string LEFT = "left", RIGHT = "Right", DOWN = "Down", UP = "UP";

				var roomToSplit = toSplit.Value;
				ref readonly var boundingVolume = ref roomToSplit.boundingVolume;
				if (!CanSplit(input.roomSize.Min.xz, boundingVolume.MinMaxXZ(), out var result))
					return false;
				SplitAlongRandomAxis(boundingVolume.MinMaxXZ(), in result, ref random, out var boundsLeft, out var boundsRight, out var horizontal);
				toSplit.Left = CreateNode(roomToSplit, Zip(in boundingVolume, in boundsLeft), horizontal ? LEFT : DOWN);
				toSplit.Right = CreateNode(roomToSplit, Zip(in boundingVolume, in boundsRight), horizontal ? RIGHT : UP);
				return true;
			}

			private static void SplitAlongRandomAxis(in MinMax2 boundingVolume, in SplitRanges split, ref random random, out MinMax2 left, out MinMax2 right, out bool horizontal)
			{
				right = left = boundingVolume;
				if (horizontal = Horizontal(split.PossibleDirections, ref random))
					left.max.x = right.min.x = random.Random(split.X);
				else
					left.max.y = right.min.y = random.Random(split.Y);

				static bool Horizontal(bool2 possibleDirections, ref random random)
				{
					if (math.all(possibleDirections))
						return random.NextBool();
					else
						return possibleDirections.x;
				}
			}

			private static bool CanSplit(in float2 minRoomSize, in MinMax2 bounds, out SplitRanges result)
			{
				result.ranges = new(bounds.min + minRoomSize, bounds.max - minRoomSize);
				return result;
			}

			private static MinMaxAABB Zip(in MinMaxAABB source, in MinMax2 xy) => new(new(xy.min.x, source.Min.y, xy.min.y), new(xy.max.x, source.Max.y, xy.max.y));

			private static BinaryTree<RoomData>.Node CreateNode(Transform parent, MinMaxAABB bounds, string name)
			{
				var child = new GameObject(parent.name + '-' + name).transform;
				child.SetParent(parent);
				return new()
				{
					Value = new(bounds, child)
				};
			}

			private struct SplitRanges
			{
				public MinMax2 ranges;

				public readonly MinMax X => new(ranges.min.x, ranges.max.x);
				public readonly MinMax Y => new(ranges.min.y, ranges.max.y);
				public readonly bool2 PossibleDirections => ranges.Range >= 0f;

				public static implicit operator SplitRanges(in MinMax2 ranges) => new() { ranges = ranges };

				public static implicit operator MinMax2(in SplitRanges split) => split.ranges;

				public static implicit operator bool(in SplitRanges split) => math.any(split.PossibleDirections);
			}
		}
	}
}
