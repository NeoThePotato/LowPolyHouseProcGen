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
	}
}
