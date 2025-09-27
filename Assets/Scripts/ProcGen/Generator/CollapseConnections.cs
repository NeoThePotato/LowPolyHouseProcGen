using System;
using Unity.Mathematics;
using random = Unity.Mathematics.Random;
using static ProcGen.Generator.RoomData;

namespace ProcGen
{
	public static partial class Generator
	{
		public static void CollapseConnections(in Input input, ref random random, Span<Connection> connections)
		{
			for (int i = 0; i < connections.Length; i++)
				ShrinkConnection(input.connectionSize, ref connections[i], ref random);
		}

		private static void ShrinkConnection(float3 connectionSize, ref Connection connection, ref random random)
		{
			if (!connection.XAligned)
				(connectionSize.x, connectionSize.z) = (connectionSize.z, connectionSize.x);
			var newMin = random.NextFloat3(connection.volume.Min, connection.volume.Max - connectionSize);
			newMin.y = connection.volume.Min.y;
			connection.volume = new(newMin, newMin + connectionSize);
		}
	}
}
