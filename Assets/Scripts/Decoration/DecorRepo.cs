using System.Collections.Generic;
using UnityEngine;

namespace ProcGen
{
    [CreateAssetMenu(menuName = "DecorRepo", fileName = "NewDecorRepo", order = 0)]
    public class DecorRepo : ScriptableObject
    {
        [Header("Generic Decor")]
        [SerializeField] private GameObject[] _floorObjects;
        [SerializeField] private GameObject[] _wallObjects;

        [Header("Key Room Decor")]
        [SerializeField] private GameObject[] _keyRoomObjects;

        public IReadOnlyList<GameObject> FloorObjects => _floorObjects;
        public IReadOnlyList<GameObject> WallObjects => _wallObjects;
        public IReadOnlyList<GameObject> KeyRoomObjects => _keyRoomObjects;
    }
}
