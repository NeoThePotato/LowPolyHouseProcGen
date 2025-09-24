using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using random = Unity.Mathematics.Random;
using Extensions;
using ProcGen.Collections;

namespace ProcGen
{
	public static partial class Generator
	{
		public static void ConnectRooms(in Input input, ref random random, INode<RoomData> tree)
		{
			var allRooms = tree.Leaves().Select(n => n.Value).ToArray();
			HashSet<RoomData> connectedRooms = new();
			for (int i = 0; i < allRooms.Length; i++)
				ConnectRoom(allRooms[i], ref random, input.connectionSize.xy);

			void ConnectRoom(RoomData room, ref random random, float2 connectionSize)
			{
				connectedRooms.Add(room);
				var availableConnections = allRooms.Except(connectedRooms).Where(r => CanConnect(in room.boundingVolume, in r.boundingVolume, in connectionSize, out _)).ToArray();
				foreach (var toConnect in availableConnections)
				{
					Connect(room, toConnect);
					connectedRooms.Add(toConnect);
				}
			}
		}

		public static bool CanConnect(in MinMaxAABB boundingVolume1, in MinMaxAABB boundingVolume2, in float2 connectionSize, out MinMaxAABB border)
		{
			if (HasSharedBorder(in boundingVolume1, in boundingVolume2, out border) && border.Extents.y >= connectionSize.y && math.any(border.Extents.xz >= connectionSize.x))
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
			RoomData.Connection connection = new() { room1 = room1, room2 = room2 };
			HasSharedBorder(room1.boundingVolume, room2.boundingVolume, out connection.volume);
			room1.connections.Add(connection);
			room2.connections.Add(connection);
		}
	}
}
