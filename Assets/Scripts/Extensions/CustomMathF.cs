using UnityEngine;

namespace Extensions
{
	public static class CustomMathF
	{

		public static float QuadraticSmoothStep(float from, float to, float t)
		{
			t = Mathf.Clamp01(t);
			t = t * t;
			return (float)((double)to * (double)t + (double)from * (1.0 - (double)t));
		}

		public static float DoubleSmoothStep(float from, float to, float t)
		{
			t = Mathf.Clamp01(t);
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			return (float)((double)to * (double)t + (double)from * (1.0 - (double)t));
		}

		public static float EaseOutCirc(float start, float end, float value)
		{
			value--;
			end -= start;
			return end * Mathf.Sqrt(1 - value * value) + start;
		}

	}
}
