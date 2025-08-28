using System.Collections.Generic;
using System.Linq;

namespace ProcGen.Collections
{
	public static class NodeExtensions
	{
		public static bool IsLeaf<T>(this INode<T> node) => node.Left == default && node.Right == default;

		/// <param name="node"><see cref="Node"/> to search.</param>
		/// <param name="ancestor">Ancestor of <paramref name="node"/>.</param>
		/// <returns>The parent of <paramref name="node"/> if exists.</returns>
		public static INode<T> Parent<T>(this INode<T> node, INode<T> ancestor) => ancestor.FirstOrDefault(n => n.Left == node || n.Right == node);

		/// <param name="node"><see cref="Node"/> to search.</param>
		/// <returns>All leaves under <paramref name="node"/>.</returns>
		public static IEnumerable<INode<T>> Leaves<T>(this INode<T> node) => node.Where(n => n.IsLeaf());
	}
}
