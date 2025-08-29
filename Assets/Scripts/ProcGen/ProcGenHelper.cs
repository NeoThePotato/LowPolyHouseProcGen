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
		[SerializeField] private float2 minRoomSize, maxRoomSize;
		[SerializeField, Tooltip("Set to 0 to randomly generate a seed.")] private uint seed;
		private INode<Generator.RoomData> _rooms;

		[ContextMenu("Generate")]
		public void Generate()
		{
			RemoveOldGeneration();
			Generator.Generate(new(_assets, _boundingVolume, minRoomSize, maxRoomSize), GetRandom(), out _rooms);
			_rooms.Value.parent.SetParent(_parent);
		}

		private void OnDrawGizmosSelected()
		{
			Handles.color = Color.blue;
			Handles.DrawWireCube(_boundingVolume.Center, _boundingVolume.Extents);
			if (_rooms == null)
				return;
			Handles.color = Color.red;
			foreach (var room in _rooms)
				Handles.DrawWireCube(room.Value.boundingVolume.Center, room.Value.boundingVolume.Extents);
		}

		private random GetRandom()
		{
			if (seed == 0)
				return new((uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue));
			return new(seed);
		}

		private void RemoveOldGeneration()
		{
			if (_parent.childCount == 0)
				return;
			while (_parent.childCount > 0)
				Destroy(_parent.GetChild(0).gameObject);
			_rooms = null;
		}
	}
}