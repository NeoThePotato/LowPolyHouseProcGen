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
        public static void GenerateQuests(ref random random, INode<RoomData> rooms)
		{
             RoomTypeAssigner.AssignTypes(rooms, ref random);
        }
	}

    public static class RoomTypeAssigner
    {
        public static INode<RoomData> AssignTypes(this INode<RoomData> rooms, ref random random)
        {
            foreach (var room in rooms.Leaves())
            {
                room.Value.roomType = RoomType.None;
            }
                
            var root = rooms;
            root.Value.roomType = RoomType.Entrance;
            var lastRoom = GetLastInTree(root);
            lastRoom.Value.roomType = RoomType.Exit;

            var lockedRoom = GetRandomRoomExcept(root, new List<INode<RoomData>> {lastRoom}, ref random);
            lockedRoom.Value.roomType = RoomType.LockedRoom;
            var parents = GetAllParents(lockedRoom, root);
            var keyRoom = parents[random.NextInt(0, parents.Count-1)];
            keyRoom.Value.roomType = RoomType.KeyRoom;

            RandomRoomAssignmentExcept(root, ref random);

            return root;
        }

        public static INode<RoomData> GetRandomRoomExcept(INode<RoomData> root, List<INode<RoomData>> excludedList, ref random random)
        {
            if (root.Leaves().Count() <= excludedList.Count + 2)
                throw new ArgumentException("Excluded list cannot be greater than or equal to the room list - 2.");

            INode<RoomData> room = null;
            while (room == null)
            {
                List<INode<RoomData>> availableRooms = root.Leaves().Except(excludedList).ToList();
                room = availableRooms[random.NextInt(1, availableRooms.Count)];
                if (CheckParentAmountSafe(room, root))
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

        public static INode<RoomData> GetLastInTree(INode<RoomData> root)
        {
            List<INode<RoomData>> lastWithParents = new List<INode<RoomData>> {root };
            INode<RoomData> deepestRoom = root;

            foreach(var room in root.Leaves())
            {
                var currentPath = GetAllParents(room, root);
                if (lastWithParents.Count < currentPath.Count  )
                {
                    lastWithParents = currentPath;
                    deepestRoom = room;
                }
            }

            
            return deepestRoom;
        }

        public static void RandomRoomAssignmentExcept(INode<RoomData> root, ref random random)
        {
            //foreach (var room in dict)
            //{
            //    if (room.Value == RoomType.None)
            //        dict[room.Key] = (RoomType)UnityEngine.Random.Range(5, Enum.GetValues(typeof(RoomType)).Length);
            //}

            foreach(var room in root.Leaves())
            {
                if(room.Value.roomType == RoomType.None)
                {
                    room.Value.roomType = (RoomType)random.NextInt(5, Enum.GetValues(typeof(RoomType)).Length);
                }
            }

            //for (int i = 0; i < dict.Count; i++)
            //{
            //    var room = dict.ElementAt(i);
            //    if (room.Value == RoomType.None)
            //        dict[room.Key] = (RoomType)random.NextInt(5, Enum.GetValues(typeof(RoomType)).Length);
            //}
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
