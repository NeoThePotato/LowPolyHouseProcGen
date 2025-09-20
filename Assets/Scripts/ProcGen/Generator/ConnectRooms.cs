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
				ConnectRoom(allRooms[i], ref random, input.connectionSize);

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

		public static void Disconnect(RoomData room1, RoomData room2)
		{
			room1.connections.Remove(room2);
			room2.connections.Remove(room1);
		}

		public static bool Connected(RoomData room1, RoomData room2)
		{
			return room1.connections.Contains(room2) && room2.connections.Contains(room1);
		}
	}
}
