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
			var newMin = connection.volume.Min;
			if (connection.XAligned)
			{
				newMin.x = random.NextFloat(connection.volume.Min.x, connection.volume.Max.x - connectionSize.x);
				connectionSize.z = 0f;
			}
			else
			{
				connectionSize.z = connectionSize.x;
				newMin.z = random.NextFloat(connection.volume.Min.z, connection.volume.Max.z - connectionSize.z);
				connectionSize.x = 0f;
			}
			connection.volume = new(newMin, newMin + connectionSize);
		}
	}
}
