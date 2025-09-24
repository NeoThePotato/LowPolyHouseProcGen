using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using random = Unity.Mathematics.Random;
using ProcGen.Collections;

namespace ProcGen
{
	public class ProcGenHelper : MonoBehaviour
	{
		[SerializeField] private AssetsCollection _assets;
		[SerializeField] private Transform _parent;
		[SerializeField] private MinMaxAABB _boundingVolume = new(-1f, 1f);
		[SerializeField] private MinMaxAABB _roomSize;
		[SerializeField] private float3 _connectionSize;
		[SerializeField, Tooltip("Set to 0 to randomly generate a seed.")] private uint _seed;
		private INode<Generator.RoomData> _rooms;

		[ContextMenu("Generate")]
		public void Generate()
		{
			RemoveOldGeneration();
			var house = Generator.Generate(new(_assets, _boundingVolume, _roomSize, _connectionSize), GetRandom(out var seed), out _rooms);
			if (!house)
				return;
			house.transform.SetParent(_parent);
			house.name = seed.ToString();
		}

		private void OnDrawGizmosSelected()
		{
			if (_rooms == null)
				return;
			DrawRooms();
			DrawConnections();

			void DrawRooms()
			{
				Handles.color = Color.blue;
				Handles.DrawWireCube(_boundingVolume.Center, _boundingVolume.Extents);
				if (_rooms == null)
					return;
				Handles.color = Color.red;
				foreach (var room in _rooms)
					Handles.DrawWireCube(room.Value.boundingVolume.Center, room.Value.boundingVolume.Extents);
			}

			void DrawConnections()
			{
				Handles.color = Color.blue;
				foreach (var room in _rooms.Leaves().Select(n => n.Value))
				{
					foreach (var connection in room.connections)
					{
						Handles.DrawLine(room.boundingVolume.Center, connection.boundingVolume.Center);
					}
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
			if (_rooms == null || !_rooms.Value.parent)
				return;
			DestroyImmediate(_rooms.Value.parent.gameObject);
			_rooms = null;
		}
	}
}