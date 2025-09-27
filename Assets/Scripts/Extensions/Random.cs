using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using random = Unity.Mathematics.Random;
using Unity.Collections;

namespace Extensions
{
	public static class Random
	{
		public static T GetRandom<Indexable, T>(this Indexable source, ref random random) where Indexable : IIndexable<T> where T : unmanaged => source.ElementAt(random.NextInt(source.Length));

		public static ref readonly T GetRandom<T>(this ReadOnlyMemory<T> source, ref random random) => ref source.Span.GetRandom(ref random);

		public static ref readonly T GetRandom<T>(this ReadOnlySpan<T> source, ref random random) => ref source[random.NextInt(source.Length)];

		public static void Shuffle<T>(this IList<T> source, ref random random)
		{
			for (int i = 0; i < source.Count; i++)
			{
				var swap = random.NextInt(source.Count);
				(source[i], source[swap]) = (source[swap], source[i]);
			}
		}

		public static bool RandomInt(int numerator, int denominator)
		{
			var rand = UnityEngine.Random.Range(0, numerator);
			return rand >= denominator;
		}

		public static bool RandomInt(uint numerator, uint denominator)
		{
			var rand = UnityEngine.Random.Range(0, numerator);
			return rand >= denominator;
		}

		public static T GetWeightedRandom<T>(this IEnumerable<T> source, Func<T, int> getWeight)
		{
			var random = UnityEngine.Random.Range(0, source.Sum(getWeight));
			int iterated = 0;
			foreach (var t in source)
			{
				iterated += getWeight(t);
				if (iterated > random)
					return t;
			}
			return source.Last();
		}

		public static T GetWeightedRandom<T>(this IEnumerable<T> source, Func<T, float> getWeight)
		{
			var random = UnityEngine.Random.Range(0, source.Sum(getWeight));
			float iterated = 0f;
			foreach (var t in source)
			{
				iterated += getWeight(t);
				if (iterated > random)
					return t;
			}
			return source.Last();
		}

		/// <param name="chance">Chance to return <see langword="true"/>.</param>
		/// <returns>A random <see langword="bool"/> with <paramref name="chance"/>% chance of being <see langword="true"/>.</returns>
		public static bool TryRoll(float chance)
		{
			return chance >= 1f || UnityEngine.Random.value <= chance;
		}

		/// <summary>
		/// Like <see cref="TryRoll(float)"/> with an inherent 50%.
		/// </summary>
		/// <returns>A random <see langword="bool"/> with 50% chance of being <see langword="true"/>.</returns>
		public static bool TryRoll()
		{
			return UnityEngine.Random.Range(0, 2) == 1;
		}

		/// <param name="count">How many times to roll.</param>
		/// <param name="chance">Chance that roll will suceed.</param>
		/// <returns>How many rolls succeeded (Value between 0 and <paramref name="count"/>).</returns>
		public static uint RollMany(uint count, float chance) // TODO Optimize
		{
			if (chance >= 1f)
				return count;
			else if (chance <= 0f)
				return 0;
			uint ret = 0;
			for (int i = 0; i < count; i++)
				ret += (uint)math.select(0, 1, TryRoll(chance));
			return ret;
		}

		public static float GetRandomPolarity(float multiplier = 1f)
		{
			return TryRoll() ? multiplier : -multiplier;
		}

		public static Vector2 GetRandomDirection2DRad(float rangeRadians)
		{
			return new(Mathf.Cos(rangeRadians), Mathf.Sin(rangeRadians));
		}

		public static Vector2 GetRandomDirection2DDeg(float rangeDegrees)
		{
			return GetRandomDirection2DRad(rangeDegrees * Mathf.Deg2Rad);
		}

		/// <param name="minValues">Bottom-Left corner of the bounds.</param>
		/// <param name="maxValues">Top-Right corner of the bounds.</param>
		/// <returns>A random point within the bounds.</returns>
		public static Vector2 GetRandomPositionInBounds(Vector2 minValues, Vector2 maxValues)
		{
			return new(UnityEngine.Random.Range(minValues.x, maxValues.x), UnityEngine.Random.Range(minValues.y, maxValues.y));
		}
	}
}
