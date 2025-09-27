using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using random = Unity.Mathematics.Random;
using Extensions;
using static ProcGen.Generator.RoomData;

namespace ProcGen
{
	public static partial class Generator
	{
		public static void ConnectRooms(in Input input, ref random random, RoomData[] rooms, out Connection[] connections)
		{
			HashSet<RoomData> connectedRooms = new();
			List<Connection> connectionsList = new();
			for (int i = 0; i < rooms.Length; i++)
				ConnectRoom(rooms[i], ref random, input.connectionSize.xy);
			connections = connectionsList.ToArray();

			void ConnectRoom(RoomData room, ref random random, float2 connectionSize)
			{
				connectedRooms.Add(room);
				var availableConnections = rooms.Except(connectedRooms).Where(r => CanConnect(in room.boundingVolume, in r.boundingVolume, in connectionSize, out _)).ToArray();
				foreach (var toConnect in availableConnections)
				{
					Connect(room, toConnect, connectionsList);
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

		public static void Connect(RoomData room1, RoomData room2, List<Connection> connections)
		{
			RoomData.Connection connection = new() { room1 = room1, room2 = room2 };
			HasSharedBorder(room1.boundingVolume, room2.boundingVolume, out connection.volume);
			connections.Add(connection);
			var connectionIndex = connections.Count - 1;
			room1.connections.Add(connectionIndex);
			room2.connections.Add(connectionIndex);
		}
	}
}
