using System;
using UnityEngine;
using Unity.Mathematics.Geometry;

namespace ProcGen
{
	public static partial class Generator
	{
		public static void ShrinkRooms(in Input input, RoomData[] rooms, Span<RoomData.Connection> connections)
		{
			var shrinkBy = input.connectionSize.z * .5f;
			foreach (var room in rooms)
			{
				ShrinkBounds(ref room.boundingVolume, shrinkBy);
				RaiseTransformY(room.transform, shrinkBy);
			}
			for (var i = 0; i < connections.Length; i++)
				RaiseY(ref connections[i].volume, shrinkBy);
		}

		private static void ShrinkBounds(ref MinMaxAABB bounds, float by)
		{
			bounds.Min += by;
			bounds.Max -= by;
		}

		private static void RaiseY(ref MinMaxAABB connection, float by)
		{
			connection.Min.y += by;
			connection.Max.y += by;
		}

		private static void RaiseTransformY(Transform transform, float by)
		{
			var position = transform.localPosition;
			position.y += by;
			transform.localPosition = position;
		}
	}
}
