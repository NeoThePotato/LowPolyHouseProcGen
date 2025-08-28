using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;

namespace Extensions
{
	[BurstCompile]
	public static class Transform
	{
		public static Vector2 Direction(Vector2 source, Vector2 target) => target - source;

		public static Vector2 Direction(this UnityEngine.Transform source, Vector2 target) => Direction(source.position, target);

		public static Vector2 Direction(this UnityEngine.Transform source, UnityEngine.Transform target) => Direction(source.position, target.position);

		public static void LookAt2D(this UnityEngine.Transform transform, Vector2 target) => transform.right = Direction(transform.position, target);

		public static void LookAt2D(this UnityEngine.Transform transform, UnityEngine.Transform target) => LookAt2D(transform, target.position);

		public static Vector2 Rotate2DDeg(this Vector2 vector, float degrees) => vector.Rotate2DRad(degrees * Mathf.Deg2Rad);

		public static Vector2 Rotate2DRad(this Vector2 vector, float radians)
		{
			float sin = Mathf.Sin(radians);
			float cos = Mathf.Cos(radians);
			return new((cos * vector.x) - (sin * vector.y), (sin * vector.x) + (cos * vector.y));
		}

		public static Vector2 DirectionFromDeg(float degrees) => DirectionFromRad(degrees * Mathf.Deg2Rad);

		public static Vector2 DirectionFromRad(float radians) => new(Mathf.Cos(radians), Mathf.Sin(radians));

		public static float DegFromDirection(this Vector2 direction) => Vector2.SignedAngle(Vector2.right, direction);

		public static float RadFromDirection(this Vector2 direction) => DegFromDirection(direction) * Mathf.Deg2Rad;

		[BurstCompile]
		public static float DegFromDirection(in this float2 direction) => Math.AngleDegrees(new(1f, 0f), direction) * math.sign(direction.y);

		[BurstCompile]
		public static float RadFromDirection(in this float2 direction) => Math.AngleRadians(new(1f, 0f), direction) * math.sign(direction.y);
	}
}
