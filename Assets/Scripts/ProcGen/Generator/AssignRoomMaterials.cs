using System;
using UnityEngine;
using random = Unity.Mathematics.Random;
using Extensions;

namespace ProcGen
{
	public static partial class Generator
	{
		private static void AssignRoomsMaterials(in Input input, ref random random, RoomData[] rooms)
		{
			var reusedList = new Material[3];
			foreach (var room in rooms)
			{
				if (room.parent.TryGetComponent<MeshRenderer>(out var renderer))
					AssignRendererMaterials(input, ref random, renderer, reusedList);
			}
		}

		private static void AssignRendererMaterials(in Input input, ref random random, MeshRenderer meshRenderer, Material[] materials)
		{
			for (var subMesh = 0; subMesh < SUBMESH_COUNT; subMesh++)
				materials[subMesh] = GetMaterialCollection(input.assets, subMesh).GetRandom(ref random);
			meshRenderer.materials = materials;

			static ReadOnlyMemory<Material> GetMaterialCollection(AssetsCollection assets, int subMeshIndex)
			{
				return subMeshIndex switch
				{
					FLOOR => assets.FloorMaterials,
					CEILING => assets.CeilingMaterials,
					WALLS => assets.WallMaterials,
					_ => default
				};
			}
		}
	}
}
