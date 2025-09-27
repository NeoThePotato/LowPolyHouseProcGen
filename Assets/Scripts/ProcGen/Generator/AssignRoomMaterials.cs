using System;
using System.Buffers;
using UnityEngine;
using random = Unity.Mathematics.Random;
using Extensions;

namespace ProcGen
{
	public static partial class Generator
	{
		private static void AssignRoomsMaterials(in Input input, ref random random, RoomData[] rooms)
		{
			var array = ArrayPool<Material>.Shared.Rent(SubMeshIndices.SUBMESH_COUNT);
			var materials = array[..SubMeshIndices.SUBMESH_COUNT];
			foreach (var room in rooms)
			{
				if (room.parent.TryGetComponent<MeshRenderer>(out var renderer))
					AssignRendererMaterials(input, ref random, renderer, materials);
			}
			ArrayPool<Material>.Shared.Return(array);
		}

		private static void AssignRendererMaterials(in Input input, ref random random, MeshRenderer meshRenderer, Material[] materials)
		{
			for (var subMesh = 0; subMesh < SubMeshIndices.SUBMESH_COUNT; subMesh++)
				materials[subMesh] = GetMaterialCollection(input.assets, subMesh).GetRandom(ref random);
			meshRenderer.materials = materials;

			static ReadOnlyMemory<Material> GetMaterialCollection(AssetsCollection assets, int subMeshIndex)
			{
				return subMeshIndex switch
				{
					SubMeshIndices.FLOOR => assets.FloorMaterials,
					SubMeshIndices.CEILING => assets.CeilingMaterials,
					SubMeshIndices.WALLS => assets.WallMaterials,
					_ => default
				};
			}
		}
	}
}
