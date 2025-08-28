using UnityEditor;
using UnityEngine;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using random = Unity.Mathematics.Random;

namespace ProcGen
{
	public class ProcGenHelper : MonoBehaviour
	{
		[SerializeField] private AssetsCollection _assets;
		[SerializeField] private Transform _parent;
		[SerializeField] private MinMaxAABB _boundingVolume = new(-1f, 1f);
		[SerializeField] private float2 minRoomSize, maxRoomSize;
		[SerializeField, Tooltip("Set to 0 to randomly generate a seed.")] private uint seed;

		[ContextMenu("Generate")]
		public void Generate()
		{
			Generator.Generate(new(_assets, _boundingVolume, minRoomSize, maxRoomSize), GetRandom());
		}

		private void OnDrawGizmosSelected()
		{
			Handles.color = Color.blue;
			Handles.DrawWireCube(_boundingVolume.Center, _boundingVolume.Extents);
		}

		private random GetRandom()
		{
			if (seed == 0)
				return new((uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue));
			return new(seed);
		}
	}
}