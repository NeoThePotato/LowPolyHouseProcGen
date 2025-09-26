using System.Linq;
using UnityEditor;
using UnityEngine;
using random = Unity.Mathematics.Random;
using ProcGen.Collections;

namespace ProcGen
{
	public class ProcGenHelper : MonoBehaviour
	{
		[SerializeField] private Generator.Input _input;
		[SerializeField, Tooltip("Set to 0 to randomly generate a seed.")] private uint _seed;
		private INode<Generator.RoomData> _rooms;

		[ContextMenu("Generate")]
		public void Generate()
		{
			RemoveOldGeneration();
			var house = Generator.Generate(in _input, GetRandom(out var seed), out _rooms);
			if (!house)
				return;
			house.transform.SetParent(transform);
			house.name = seed.ToString();
		}

		private void OnDrawGizmosSelected()
		{
			DrawBoundingVolume();
			if (_rooms == null)
				return;
			DrawRooms();
			DrawConnections();

			void DrawBoundingVolume()
			{
				Handles.color = Color.blue;
				Handles.DrawWireCube(_input.boundingVolume.Center, _input.boundingVolume.Extents);
			}

			void DrawRooms()
			{
				Handles.color = Color.red;
				foreach (var room in _rooms)
					Handles.DrawWireCube(room.Value.boundingVolume.Center, room.Value.boundingVolume.Extents);
			}

			void DrawConnections()
			{
				Handles.color = Color.blue;
				var connections = _rooms.Leaves().SelectMany(n => n.Value.connections).Distinct();
				foreach (var connection in connections)
					Handles.DrawLine(connection.room1.boundingVolume.Center, connection.room2.boundingVolume.Center);
			}
		}

		private random GetRandom(out uint seed)
		{
			if (_seed == 0)
				return new(seed = (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue));
			return new(seed = _seed);
		}

        [ContextMenu("RemoveOldGen")]
        private void RemoveOldGeneration()
		{
			if (_rooms == null || !_rooms.Value.parent)
				return;
			DestroyImmediate(_rooms.Value.parent.gameObject);
			_rooms = null;
		}
	}
}