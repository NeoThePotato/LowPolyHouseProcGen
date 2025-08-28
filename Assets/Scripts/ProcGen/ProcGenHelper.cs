using UnityEditor;
using UnityEngine;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;

namespace ProcGen
{
	public class ProcGenHelper : MonoBehaviour
	{
		[SerializeField] private AssetsCollection _assets;
		[SerializeField] private Transform _parent;
		[SerializeField] private MinMaxAABB _boundingVolume = new(-1f, 1f);
		[SerializeField] private float2 minRoomSize, maxRoomSize;

		[ContextMenu("Generate")]
		public void Generate()
		{
			Generator.Generate(new(_assets, _boundingVolume, minRoomSize, maxRoomSize));
		}

		private void OnDrawGizmosSelected()
		{
			Handles.color = Color.blue;
			Handles.DrawWireCube(_boundingVolume.Center, _boundingVolume.Extents);
		}
	}
}