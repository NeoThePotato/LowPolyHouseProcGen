using System.Collections.Generic;

namespace ProcGen.Collections
{
	public interface INode<T> : IReadOnlyCollection<INode<T>>
	{
		public T Value { get; set; }
		public INode<T> Left { get; set; }
		public INode<T> Right { get; set; }
	}
}
