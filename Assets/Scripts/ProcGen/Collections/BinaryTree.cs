using System.Collections;
using System.Collections.Generic;

namespace ProcGen.Collections
{
	public class BinaryTree<T> : INode<T>
	{
		private readonly Node _root;

		public Node Root => _root;

		public BinaryTree(T rootValue) => _root = new() { Value = rootValue };

		public T Value { get => Root.Value; set => Root.Value = value; }
		public INode<T> Left { get => Root.Left; set => Root.Left = value; }
		public INode<T> Right { get => Root.Right; set => Root.Right = value; }

		public int Count => _root.Count;

		public IEnumerator<INode<T>> GetEnumerator() => _root.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _root.GetEnumerator();

		public class Node : INode<T>
		{
			public T Value { get; set; }
			public INode<T> Left { get; set; }
			public INode<T> Right { get; set; }

			public int Count => 1 + Left.Count + Right.Count;

			/// <summary>
			/// Depth-First enumeration.
			/// </summary>
			public IEnumerator<INode<T>> GetEnumerator()
			{
				yield return this;
				if (Left != null)
				{
					foreach (var item in Left)
						yield return item;
				}
				if (Right != null)
				{
					foreach (var item in Right)
						yield return item;
				}
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public static implicit operator T(Node node) => node.Value;
		}
	}
}
