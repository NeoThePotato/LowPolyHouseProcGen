using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Mathematics;

namespace Extensions
{
	public static class Mask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Contains(in this LayerMask mask, int layer) => mask == (mask | layer.ToMask());

		/// <summary>
		/// Converts an index to a mask.
		/// Useful when trying to convert layer to LayerMask or NavMeshAreas to NavMesh masks.
		/// </summary>
		/// <param name="index">Index to convert</param>
		/// <returns>The created mask.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToMask(in this int index) => (1 << index);

		/// <param name="mask">The <see cref="LayerMask"/> to find the first layer of.</param>
		/// <returns>The first layer contained in <paramref name="mask"/>.</returns>
		/// <seealso cref="math.tzcnt"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LayerIndex(in this LayerMask mask) => math.tzcnt(mask.value);
	}
}