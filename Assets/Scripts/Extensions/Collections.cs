using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace Extensions
{
	public static class Collections
	{
		public static void EnsureExactLength<T>(ref NativeArray<T> array, int length, Allocator allocator) where T : struct
		{
			if (!array.IsCreated)
			{
				array = new(length, allocator);
				return;
			}
			if (array.Length == length)
				return;
			array.Dispose();
			array = new(length, allocator);
		}

		public static void RemoveAtSwapBack<TCollection, T>(this TCollection source, Index index) where TCollection : IList<T>
		{
			source[index] = source[^1];
			source.RemoveAt(source.Count - 1);
		}

		public static void RemoveAtSwapBack<T>(this T[] source, Index index)
		{
			(source[index], source[^1]) = (source[^1], source[index]);
		}

		public static void RemoveAtSwapBack<T>(this T[] source, Index indexRemove, Index indexSwap)
		{
			(source[indexRemove], source[indexSwap]) = (source[indexSwap], source[indexRemove]);
		}

		public static void RemoveAtSwapBack<T>(this NativeArray<T> source, Index index) where T : struct
		{
			(source[index], source[^1]) = (source[^1], source[index]);
		}

		public static void RemoveAtSwapBack<T>(this NativeArray<T> source, Index indexRemove, Index indexSwap) where T : struct
		{
			(source[indexRemove], source[indexSwap]) = (source[indexSwap], source[indexRemove]);
		}

		public static bool Remove<TKey, TValue>(this NativeHashMap<TKey, TValue> source, TKey key, out TValue value)
		where TKey : unmanaged, IEquatable<TKey>
		where TValue : unmanaged
		{
			return source.TryGetValue(key, out value) && source.Remove(key);
		}

		/// <summary>
		/// Wrapper for duplicate-preventing <see cref="ICollection"/> with <see cref="ReadOnlySpan"/> support.
		/// </summary>
		public readonly struct SetSpan<T> : ICollection<T>, IReadOnlyList<T>
		{
			private readonly Dictionary<T, Index> _source;
			private readonly T[] _mirror;

			public int Capacity => _mirror.Length;

			public SetSpan(int mirrorCapacity)
			{
				_source = new(mirrorCapacity);
				_mirror = new T[mirrorCapacity];
			}

			public readonly ReadOnlySpan<T> AsSpan() => _mirror.AsSpan(0, Count);

			public readonly Memory<T> AsMemory() => _mirror.AsMemory(0, Count);

			public readonly bool TryAdd(T item)
			{
				var index = Count;
				if (index >= Capacity || !_source.TryAdd(item, index))
					return false;
				_mirror[index] = item;
				return true;
			}

			public readonly bool Remove(T item, out Index index)
			{
				if (!_source.Remove(item, out index))
					return false;
				var lastIndex = _source.Count;
				if (index.Value == lastIndex)
					return true;
				_mirror.RemoveAtSwapBack(index, lastIndex);
				_source[_mirror[index]] = index;
				return true;
			}

			#region OVERRIDES
			public readonly int Count => _source.Count;

			public readonly bool IsReadOnly => false;

			public T this[int index] => _mirror[index];

			public readonly void Add(T item) => TryAdd(item);

			public readonly bool Remove(T item) => Remove(item, out _);

			public readonly void Clear() => _source.Clear();

			public readonly bool Contains(T item) => _source.ContainsKey(item);

			public readonly void CopyTo(T[] array, int arrayIndex) => _mirror.CopyTo(array, arrayIndex);

			public readonly ReadOnlySpan<T>.Enumerator GetEnumerator() => AsSpan().GetEnumerator();

			readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => _source.Keys.GetEnumerator();

			readonly IEnumerator IEnumerable.GetEnumerator() => _source.GetEnumerator();
			#endregion

			public static implicit operator ReadOnlySpan<T>(in SetSpan<T> mirror) => mirror.AsSpan();
		}
	}
}
