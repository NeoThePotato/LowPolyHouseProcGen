using UnityEngine;
using Unity.Mathematics.Geometry;

namespace ProcGen
{
	public static partial class Generator
	{
		public static void ShrinkRooms(in Input input, RoomData[] rooms)
		{
			var shrinkBy = input.connectionSize.z * .5f;
			foreach (var room in rooms)
			{
				ShrinkBounds(ref room.boundingVolume, shrinkBy);
				RaiseTransformY(room.parent, shrinkBy);
			}
		}

		private static void ShrinkBounds(ref MinMaxAABB bounds, float by)
		{
			bounds.Min += by;
			bounds.Max -= by;
		}

		private static void RaiseTransformY(Transform transform, float by)
		{
			var position = transform.localPosition;
			position.y += by;
			transform.localPosition = position;
		}
	}
}
