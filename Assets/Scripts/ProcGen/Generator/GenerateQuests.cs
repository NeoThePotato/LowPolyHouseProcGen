using System;
using System.Collections.Generic;
using random = Unity.Mathematics.Random;
using ProcGen.Collections;
using System.Linq;
using static ProcGen.Generator;
using Extensions;
using System.Diagnostics;

namespace ProcGen
{
	public static partial class Generator
	{
        public static Dictionary<INode<RoomData>, RoomType> GenerateQuests(ref random random, IEnumerable<INode<RoomData>> rooms)
		{
            return RoomTypeAssigner.AssignTypes(rooms, ref random);
        }
	}

    public static class RoomTypeAssigner
    {
        public static Dictionary<INode<RoomData>, RoomType> AssignTypes(this IEnumerable<INode<RoomData>> rooms, ref random random)
        {
            Dictionary<INode<RoomData>, RoomType> roomTypes = new();
            foreach (var room in rooms)
            {
                if (!roomTypes.ContainsKey(room))
                    roomTypes.Add(room, RoomType.None);
                else
                    UnityEngine.Debug.Log("Duplicate found");
            }
                
            var firstRoom = roomTypes.First().Key;
            roomTypes[firstRoom] = RoomType.Entrance;
            var lastRoom = roomTypes.Last().Key;
            roomTypes[lastRoom] = RoomType.Exit;

            var lockedRoom = GetRandomRoomExcept(roomTypes, new List<INode<RoomData>> {lastRoom}, ref random);
            roomTypes[lockedRoom] = RoomType.LockedRoom;
            var parents = GetAllParents(lockedRoom, firstRoom);
            var keyRoom = parents[random.NextInt(0, parents.Count)];
            roomTypes[keyRoom] = RoomType.KeyRoom;

            RandomRoomAssignmentExcept(roomTypes);

            return roomTypes;
        }

        public static INode<RoomData> GetRandomRoomExcept(Dictionary<INode<RoomData>, RoomType> roomList, List<INode<RoomData>> excludedList, ref random random)
        {
            if (roomList.Count <= excludedList.Count + 2)
                throw new ArgumentException("Excluded list cannot be greater than or equal to the room list - 2.");

            INode<RoomData> room = null;
            while (room == null)
            {
                List<INode<RoomData>> availableRooms = roomList.Keys.Except(excludedList).ToList();
                room = availableRooms[random.NextInt(1, availableRooms.Count)];
                if (CheckParentAmountSafe(room, roomList.First().Key))
                    return room;
                else
                    room = null;
            }
            return null;
        }

        public static bool CheckParentAmountSafe(INode<RoomData> room, INode<RoomData> root)
        {
            var parent = room.Parent(root);
            var grandParent = parent.Parent(root);
            return grandParent != null;
        }

        public static List<INode<RoomData>> GetAllParents(INode<RoomData> target, INode<RoomData> root)
        {
            var listToReturn = new List<INode<RoomData>>();
            while (target.Parent(root) != null)
            {
                target = target.Parent(root);
                listToReturn.Add(target);
            }
            return listToReturn;
        }

        public static void RandomRoomAssignmentExcept(Dictionary<INode<RoomData>, RoomType> dict)
        {
            //foreach (var room in dict)
            //{
            //    if (room.Value == RoomType.None)
            //        dict[room.Key] = (RoomType)UnityEngine.Random.Range(5, Enum.GetValues(typeof(RoomType)).Length);
            //}
            for (int i = 0; i < dict.Count; i++)
            {
                var room = dict.ElementAt(i);
                if (room.Value == RoomType.None)
                    dict[room.Key] = (RoomType)UnityEngine.Random.Range(5, Enum.GetValues(typeof(RoomType)).Length);
            }
        }
    }

    public enum RoomType
	{
        None,
        Entrance,
        Exit,
        KeyRoom,
        LockedRoom,
		TreasureRoom,
        EnemyRoom,
        PuzzleRoom
    }
}
