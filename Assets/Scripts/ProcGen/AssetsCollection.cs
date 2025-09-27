using System;
using UnityEngine;

namespace ProcGen
{
	[CreateAssetMenu(menuName = "AssetsCollection", fileName = "NewAssetsCollection", order = 0)]
	public class AssetsCollection : ScriptableObject
	{
		[Header("Walls")]
		[SerializeField] private GameObject[] _doors;
		[SerializeField] private GameObject[] _windows;
		[SerializeField] private Material[] _wallMaterials;
		[SerializeField] private Material[] _floorMaterials;
		[SerializeField] private Material[] _ceilingMaterials;
		[Header("Furniture")]
		[SerializeField] private GameObject[] _furniture;

		[Header("KeyRoomObjects")]
		[SerializeField] private GameObject[] _keyRoomObjects;
        [Header("LockedRoomObjects")]
        [SerializeField] private GameObject[] _lockedRoomObjects;
        [Header("EntranceRoomObjects")]
        [SerializeField] private GameObject[] _entranceRoomObjects;
        [Header("ExitRoomObjects")]
        [SerializeField] private GameObject[] _exitRoomObjects;
        [Header("EnemyRoomObjects")]
        [SerializeField] private GameObject[] _enemyRoomObjects;
        [Header("PuzzleRoomObjects")]
        [SerializeField] private GameObject[] _puzzleRoomObjects;
        [Header("TreasureRoomObjects")]
        [SerializeField] private GameObject[] _treasureRoomObjects;

		public ReadOnlyMemory<GameObject> Doors => _doors;
		public ReadOnlyMemory<GameObject> Windows => _windows;
		public ReadOnlyMemory<Material> WallMaterials => _wallMaterials;
		public ReadOnlyMemory<Material> FloorMaterials => _floorMaterials;
		public ReadOnlyMemory<Material> CeilingMaterials => _ceilingMaterials;
		public ReadOnlyMemory<GameObject> Furniture => _furniture;
		public ReadOnlyMemory<GameObject> KeyRoomObjects => _keyRoomObjects;
		public ReadOnlyMemory<GameObject> LockedRoomObjects => _lockedRoomObjects;
        public ReadOnlyMemory<GameObject> EntranceRoomObjects => _entranceRoomObjects;
        public ReadOnlyMemory<GameObject> ExitRoomObjects => _exitRoomObjects;
        public ReadOnlyMemory<GameObject> EnemyRoomObjects => _enemyRoomObjects;
        public ReadOnlyMemory<GameObject> PuzzleRoomObjects => _puzzleRoomObjects;
        public ReadOnlyMemory<GameObject> TreasureRoomObjects => _treasureRoomObjects;
    }
}
