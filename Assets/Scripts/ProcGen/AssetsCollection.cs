using System.Collections.Generic;
using UnityEngine;

namespace ProcGen
{
	[CreateAssetMenu(menuName = "AssetsCollection", fileName = "NewAssetsCollection", order = 0)]
	public class AssetsCollection : ScriptableObject
	{
		[Header("Walls")]
		[SerializeField] private GameObject[] _walls;
		[SerializeField] private Material[] _wallMaterials;
		[SerializeField] private Material[] _floorMaterials;
		[Header("Furniture")]
		[SerializeField] private GameObject[] _furniture;

		public IReadOnlyList<GameObject> Walls => _walls;
		public IReadOnlyList<Material> WallMaterials => _wallMaterials;
		public IReadOnlyList<Material> FloorMaterials => _floorMaterials;
		public IReadOnlyList<GameObject> Furniture => _furniture;
	}
}
