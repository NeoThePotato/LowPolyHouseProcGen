using System.Collections.Generic;
using UnityEngine;

namespace ProcGen
{
	[CreateAssetMenu(menuName = "AssetsCollection", fileName = "NewAssetsCollection", order = 0)]
	public class AssetsCollection : ScriptableObject
	{
		[Header("Walls")]
		[SerializeField] private GameObject[] _walls;
		[SerializeField] private GameObject[] _doors;
		[SerializeField] private GameObject[] _windows;
		[SerializeField] private Material[] _wallMaterials;
		[SerializeField] private Material[] _floorMaterials;
		[Header("Furniture")]
		[SerializeField] private GameObject[] _furniture;

		[Header("KeyRoomObjects")]
		[SerializeField] private GameObject[] _keyRoomObjects;
        [Header("LockedRoomObjects")]
        [SerializeField] private GameObject[] _lockedRoomObjects;

        public IReadOnlyList<GameObject> Walls => _walls;
		public IReadOnlyList<GameObject> Doors => _doors;
		public IReadOnlyList<GameObject> Windows => _windows;
		public IReadOnlyList<Material> WallMaterials => _wallMaterials;
		public IReadOnlyList<Material> FloorMaterials => _floorMaterials;
		public IReadOnlyList<GameObject> Furniture => _furniture;
		public IReadOnlyList<GameObject> KeyRoomObjects => _keyRoomObjects;
		public IReadOnlyList<GameObject> LockedRoomObjects => _lockedRoomObjects;


	}
}
