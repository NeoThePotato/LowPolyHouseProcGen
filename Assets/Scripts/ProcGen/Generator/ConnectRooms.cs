using System.Linq;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using random = Unity.Mathematics.Random;
using ProcGen.Collections;

namespace ProcGen
{
	public static partial class Generator
	{
		public static void ConnectRooms(in Input input, ref random random, INode<RoomData> tree)
		{
			var rooms = tree.Leaves().Select(n => n.Value).ToArray();
			for (int i = 0; i < rooms.Length; i++)
			{
				for (int j = i+1; j < rooms.Length; j++)
				{
					if (CanConnect(in rooms[i].boundingVolume, in rooms[j].boundingVolume, in input.connectionSize, out _))
						Connect(rooms[i], rooms[j]);
				}
			}
		}

		public static bool CanConnect(in MinMaxAABB boundingVolume1, in MinMaxAABB boundingVolume2, in float2 connectionSize, out MinMaxAABB border)
		{
			if (HasSharedBorder(in  boundingVolume1, in boundingVolume2, out border) && border.Extents.y >= connectionSize.y && math.any(border.Extents.xz >= connectionSize.x))
				return true;
			return false;
		}

		public static bool HasSharedBorder(in MinMaxAABB boundingVolume1, in MinMaxAABB boundingVolume2, out MinMaxAABB border)
		{
			if (!boundingVolume1.Overlaps(boundingVolume2))
			{
				border = default;
				return false;
			}
			float3 min = math.max(boundingVolume1.Min, boundingVolume2.Min);
			float3 max = math.min(boundingVolume1.Max, boundingVolume2.Max);
			border = new(min, max);
			return true;
		}

		public static void Connect(RoomData room1, RoomData room2)
		{
			room1.connections.Add(room2);
			room2.connections.Add(room1);
		}
	}
}
