using UnityEditor;
using UnityEngine;
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
		[SerializeField, Tooltip("Set to 0 to randomly generate a seed.")] private uint _seed;
		private INode<Generator.RoomData> _rooms;

		[ContextMenu("Generate")]
		public void Generate()
		{
			RemoveOldGeneration();
			var house = Generator.Generate(new(_assets, _boundingVolume, _roomSize), GetRandom(out var seed), out _rooms);
			if (!house)
				return;
			house.transform.SetParent(_parent);
			house.name = seed.ToString();
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

		private random GetRandom(out uint seed)
		{
			if (_seed == 0)
				return new(seed = (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue));
			return new(seed = _seed);
		}

        [ContextMenu("RemoveOldGen")]
        private void RemoveOldGeneration()
		{
			if (_rooms == null)
				return;
			DestroyImmediate(_rooms.Value.parent.gameObject);
			_rooms = null;
		}
	}
}