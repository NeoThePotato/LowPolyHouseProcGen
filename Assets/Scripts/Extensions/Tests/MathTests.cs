using Extensions;
using NUnit.Framework;
using UnityEngine;

public static class MathTests
{
    [Test]
    public static void ToPositiveAngle()
    {
        Assert.AreEqual(0f,		Math.ToPositiveAngle(0f));
		Assert.AreEqual(359f,	Math.ToPositiveAngle(-1f));
		Assert.AreEqual(90f,	Math.ToPositiveAngle(90f));
		Assert.AreEqual(270f,	Math.ToPositiveAngle(-90f));
		Assert.AreEqual(180f,	Math.ToPositiveAngle(-180f));
		Assert.AreEqual(181f,	Math.ToPositiveAngle(181f));
		Assert.AreEqual(0f,		Math.ToPositiveAngle(360f));
		Assert.AreEqual(0f,		Math.ToPositiveAngle(-360f));
		Assert.AreEqual(0f,		Math.ToPositiveAngle(720f));
		Assert.AreEqual(0f,		Math.ToPositiveAngle(-720f));
	}

	[Test]
	public static void ToSignedAngle()
	{
		Assert.AreEqual(0f,		Math.ToSignedAngle(0f));
		Assert.AreEqual(-1f,	Math.ToSignedAngle(-1f));
		Assert.AreEqual(90f,	Math.ToSignedAngle(90f));
		Assert.AreEqual(-90f,	Math.ToSignedAngle(-90f));
		Assert.AreEqual(-180f,	Math.ToSignedAngle(-180f));
		Assert.AreEqual(-179f,	Math.ToSignedAngle(181f));
		Assert.AreEqual(0f,		Math.ToSignedAngle(360f));
		Assert.AreEqual(-90f,	Math.ToSignedAngle(-450f));
		Assert.AreEqual(0f,		Math.ToSignedAngle(-360f));
		Assert.AreEqual(0f,		Math.ToSignedAngle(720f));
		Assert.AreEqual(0f,		Math.ToSignedAngle(-720f));
	}

	[Test]
	public static void EnsureSize()
	{
		EnsureAndAssert(10f, 1f);
		EnsureAndAssert(-5f, -1f);
		EnsureAndAssert(float.MinValue, float.MaxValue);
		EnsureAndAssert(0f, 0f);

		static void EnsureAndAssert(float low, float high)
		{
			Math.EnsureSize(ref low, ref high);
			Assert.GreaterOrEqual(high, low);
		}
	}

	[Test]
	public static void SubtractNoUnderflow()
	{
		SubtractAssert(5, 4, 1, 4);
		SubtractAssert(5, 5, 0, 5);
		SubtractAssert(1, 0, 1, 0);
		SubtractAssert(0, 1, 0, 0);
		SubtractAssert(0, 0, 0, 0);
		SubtractAssert(10, 5, 5, 5);
		SubtractAssert(0, uint.MaxValue, 0, 0);
		SubtractAssert(uint.MaxValue, uint.MaxValue, 0, uint.MaxValue);

		static void SubtractAssert(uint val, uint subtract, uint expectedVal, uint expectedSubtraction)
		{
			var subtracted = Math.SubtractNoUnderflow(ref val, subtract);
			Assert.AreEqual(expectedVal, val, "Subtraction result should have been {0} but was {1}.", expectedVal, val);
			Assert.AreEqual(expectedSubtraction, subtracted, "Subtracted value should have been {0} but was {1}.", expectedSubtraction, subtracted);
		}
	}

	[Test]
	public static void GetClosestZero()
	{
		Assert.AreEqual(0f, Math.GetClosestZero(0f));
		Assert.AreEqual(360f, Math.GetClosestZero(360f));
		Assert.AreEqual(0f, Math.GetClosestZero(80f));
		Assert.AreEqual(0f, Math.GetClosestZero(-100f));
		Assert.AreEqual(360f, Math.GetClosestZero(190f));
		Assert.AreEqual(-360f, Math.GetClosestZero(-190f));
		Assert.AreEqual(-360f, Math.GetClosestZero(-360f));
	}

	[Test]
	public static void GetClosestAngle()
	{
		Assert.AreEqual(0f, Math.GetClosestAngle(0f, 0f));
		Assert.AreEqual(90f, Math.GetClosestAngle(0f, 90f));
		Assert.AreEqual(90f, Math.GetClosestAngle(90f, 90f));
		Assert.AreEqual(90f, Math.GetClosestAngle(-90f, 90f));
		Assert.AreEqual(390f, Math.GetClosestAngle(270f, 30f));
		Assert.AreEqual(330f, Math.GetClosestAngle(170f, -30f));
		Assert.AreEqual(330f, Math.GetClosestAngle(180f, -30f));
	}
}
