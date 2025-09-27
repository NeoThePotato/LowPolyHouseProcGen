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
		private INode<Generator.RoomData> _tree;
		private Generator.RoomData[] _rooms;
		private Generator.RoomData.Connection[] _connections;

		[ContextMenu("Generate")]
		public void Generate()
		{
			RemoveOldGeneration();
			var house = Generator.Generate(in _input, GetRandom(out var seed), out _tree, out _rooms, out _connections);
			if (!house)
				return;
			house.transform.SetParent(transform);
			house.name = seed.ToString();
		}

		private void OnDrawGizmosSelected()
		{
			DrawBoundingVolume();
			if (_tree == null)
				return;
			if (!_tree.Value.transform)
			{
				_tree = null;
				return;
			}
			RemoveDestroyedNodes(_tree);
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
				foreach (var room in _tree)
					Handles.DrawWireCube(room.Value.boundingVolume.Center, room.Value.boundingVolume.Extents);
			}

			void DrawConnections()
			{
				foreach (var connection in _connections)
				{
					Handles.color = Color.blue;
					Handles.DrawLine(connection.room1.boundingVolume.Center, connection.room2.boundingVolume.Center);
					Handles.color = Color.green;
					Handles.DrawWireCube(connection.volume.Center, connection.volume.Extents);
				}
			}

			void RemoveDestroyedNodes(INode<Generator.RoomData> parent)
			{
				if (parent.Left != null)
				{
					if (!parent.Left.Value.transform)
						parent.Left = null;
					else
						RemoveDestroyedNodes(parent.Left);
				}
				if (parent.Right != null)
				{
					if (!parent.Right.Value.transform)
						parent.Right = null;
					else
						RemoveDestroyedNodes(parent.Right);
				}
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
			if (_tree == null || !_tree.Value.transform)
				return;
			DestroyImmediate(_tree.Value.transform.gameObject);
			_tree = null;
		}
	}
}