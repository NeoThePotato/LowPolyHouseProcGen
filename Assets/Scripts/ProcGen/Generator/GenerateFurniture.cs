using System;
using System.Collections.Generic;
using UnityEngine;
using random = Unity.Mathematics.Random;
using ProcGen.Collections;

namespace ProcGen
{
	public static partial class Generator
	{
		/// <summary>
		/// Generate furniture for all <paramref name="rooms"/>.
		/// </summary>
		/// <param name="input">Generation input.</param>
		/// <param name="random">Random number generator.</param>
		/// <param name="rooms">Rooms to furnish.</param>
		public static void GenerateFurniture(in Input input, ref random random, IEnumerable<INode<RoomData>> rooms)
		{
			foreach (var room in rooms)
				GenerateFurniture(room.Value, input.assets.Furniture, ref random);
		}

		/// <summary>
		/// Generate furniture for a room.
		/// </summary>
		/// <param name="roomBounds">Bounds of the room.</param>
		/// <param name="furniture">Available furniture pool.</param>
		/// <param name="random">Random number generator.</param>
		public static void GenerateFurniture(in RoomData roomBounds, IReadOnlyList<GameObject> furniture, ref random random)
		{
			// TODO Implement
		}
	}
}
