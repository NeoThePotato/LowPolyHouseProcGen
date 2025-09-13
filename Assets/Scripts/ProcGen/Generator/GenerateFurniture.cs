using System;
using System.Collections.Generic;
using UnityEngine;
using random = Unity.Mathematics.Random;
using ProcGen.Collections;
using Unity.Mathematics;
using UnityEditor;
using System.Linq;

namespace ProcGen
{
	public static partial class Generator
	{
        public static AssetsCollection assets = AssetDatabase.LoadAssetAtPath<AssetsCollection>("Assets/Config/LowPolyHouse.asset");

        /// <summary>
        /// Generate furniture for all <paramref name="rooms"/>.
        /// </summary>
        /// <param name="input">Generation input.</param>
        /// <param name="random">Random number generator.</param>
        /// <param name="rooms">Rooms to furnish.</param>
        public static void GenerateFurniture(in Input input, ref random random, IEnumerable<INode<RoomData>> rooms, Dictionary<INode<RoomData>, RoomType> roomTypes)
		{

			//Debug.Log(rooms.Count());
			//Debug.Log(roomTypes.Count());
			// TODO talk with nehorai about the duplicated lists
			foreach (var room in rooms)
            {
				

					
				switch (roomTypes[room])
                {
					case RoomType.KeyRoom:
						PlaceKeyRoomObjects(room.Value, input.assets.Furniture, ref random, roomTypes);
							Debug.LogWarning("KEyRoom");
						break;

					case RoomType.LockedRoom:
						PlaceLockedRoomObjects(room.Value, input.assets.Furniture, ref random, roomTypes);
						Debug.LogWarning("DoorRoom");
						break;

					case RoomType.None:
                         GenerateFurniture(room.Value, input.assets.Furniture, ref random, roomTypes);
                         break;

					default:
						GenerateFurniture(room.Value, input.assets.Furniture, ref random, roomTypes);
							
							break;



					}
			}
				
		}

		/// <summary>
		/// Generate furniture for a room.
		/// </summary>
		/// <param name="roomBounds">Bounds of the room.</param>
		/// <param name="furniture">Available furniture pool.</param>
		/// <param name="random">Random number generator.</param>
		public static void GenerateFurniture(in RoomData roomBounds, IReadOnlyList<GameObject> furniture, ref random random, Dictionary<INode<RoomData>, RoomType> roomTypes)
		{
			GameObject go = assets.Furniture[random.NextInt(0, assets.Furniture.Count)];
			go.tag = "Furniture";

            Vector3 position = random.NextFloat3(roomBounds.boundingVolume.Min, roomBounds.boundingVolume.Max);
            quaternion rotation = quaternion.Euler(random.NextFloat3(0, 360f) * math.up());
            GameObject.Instantiate(go, position, rotation, roomBounds.parent);
        }

		public static void PlaceKeyRoomObjects(in RoomData roomBounds, IReadOnlyList<GameObject> furniture, ref random random, Dictionary<INode<RoomData>, RoomType> roomTypes)
		{
			GameObject go = assets.KeyRoomObjects[random.NextInt(0, assets.KeyRoomObjects.Count)];
			go.tag = "Furniture";

			Vector3 position = random.NextFloat3(roomBounds.boundingVolume.Min, roomBounds.boundingVolume.Max);
			quaternion rotation = quaternion.Euler(random.NextFloat3(0, 360f) * math.up());
			GameObject.Instantiate(go, position, rotation, roomBounds.parent);
		}

		public static void PlaceLockedRoomObjects(in RoomData roomBounds, IReadOnlyList<GameObject> furniture, ref random random, Dictionary<INode<RoomData>, RoomType> roomTypes)
		{
			GameObject go = assets.LockedRoomObjects[random.NextInt(0, assets.LockedRoomObjects.Count)];
			go.tag = "Furniture";

			Vector3 position = random.NextFloat3(roomBounds.boundingVolume.Min, roomBounds.boundingVolume.Max);
			quaternion rotation = quaternion.Euler(random.NextFloat3(0, 360f) * math.up());
			GameObject.Instantiate(go, position, rotation, roomBounds.parent);
		}
    }
}
