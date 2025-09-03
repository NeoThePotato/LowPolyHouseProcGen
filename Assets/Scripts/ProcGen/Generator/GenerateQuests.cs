using System;
using System.Collections.Generic;
using random = Unity.Mathematics.Random;
using ProcGen.Collections;
using System.Linq;
using static ProcGen.Generator;

namespace ProcGen
{
	public static partial class Generator
	{
        

        public static void GenerateQuests(ref random random, IEnumerable<INode<RoomData>> rooms)
		{
            Dictionary<INode<RoomData>, RoomType> newDict = RoomTypeAssigner.AssignTypes(rooms);
        }
	}

    public static class RoomTypeAssigner
    {
        public static Dictionary<INode<RoomData>, RoomType> AssignTypes(this IEnumerable<INode<RoomData>> rooms)
        {
            Dictionary<INode<RoomData>, RoomType> roomTypes = new();
            foreach (var room in rooms)
                roomTypes.Add(room, RoomType.None);

            var firstRoom = roomTypes.First().Key;
            roomTypes[firstRoom] = RoomType.Entrance;
            var lastRoom = roomTypes.Last().Key;
            roomTypes[lastRoom] = RoomType.Exit;

            // TODO Assign other room types (KeyRoom, LockedRoom, QuestRoom) based on specific criteria

            return roomTypes;
        }
    }

    public static class Pathfinder
    {
        public static List<INode<RoomData>> FindPath(INode<RoomData> start, INode<RoomData> end)
        {
            // TODO Implement pathfinding algorithm (e.g., A* or Dijkstra)
            return null;
        }
    }

    public enum RoomType
	{
        None,
        Entrance,
        Exit,
        KeyRoom,
        LockedRoom,
		QuestRoom
    }
}
