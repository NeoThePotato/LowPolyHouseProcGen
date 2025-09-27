using System;
using UnityEngine;
using GameObject = UnityEngine.GameObject;
using Unity.Mathematics;
using random = Unity.Mathematics.Random;
using Extensions;
using EMath = Extensions.Math;

namespace ProcGen
{
	public static partial class Generator
	{
		public static void GenerateDoors(in Input input, ref random random, Span<RoomData.Connection> connections)
		{
			var doors = input.assets.Doors.Span;
			for (int i = 0; i < connections.Length; i++)
				GenerateDoor(doors, ref random, ref connections[i]);
		}

		private static void GenerateDoor(ReadOnlySpan<GameObject> doors, ref random random, ref RoomData.Connection connection)
		{
			var yAngle = math.select(90f, 0f, connection.XAligned);
			var door = GameObject.Instantiate(
				original: doors.GetRandom(ref random),
				position: EMath.GetCenterRoot(in connection.volume),
				rotation: Quaternion.Euler(0f, yAngle, 0f),
				parent: connection.room1.transform);
			connection.transform = door.transform;
		}
	}
}
