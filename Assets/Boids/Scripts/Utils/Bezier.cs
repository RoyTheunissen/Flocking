using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Utility for drawing bezier curves.
/// </summary>
namespace Trepid.Flocking
{
	public static class Bezier
	{
		#region Standard Maths methods

		private static float BinomialCoefficient(int n, int k)
		{
			if ((k < 0) || (k > n)) return 0;
			k = (k > n / 2) ? n - k : k;
			return (float)FallingPower(n, k) / Factorial(k);
		}

		private static int Factorial(int n)
		{
			if (n == 0) return 1;
			int t = n;
			while (n-- > 2)
				t *= n;
			return t;
		}

		private static int FallingPower(int n, int p)
		{
			int t = 1;
			for (int i = 0; i < p; i++) t *= n--;
			return t;
		}

		#endregion

		public static Vector3 DoForNPoints(float t, Vector3[] currentArray)
		{
			Vector3 returnVector = Vector3.zero;

			float oneMinusT = (1f - t);

			int n = currentArray.Length - 1;

			for (int i = 0; i <= n; i++)
			{
				returnVector += BinomialCoefficient(n, i)
					* Mathf.Pow(oneMinusT, n - i)
					* Mathf.Pow(t, i)
					* currentArray[i];
			}

			return returnVector;
		}

	}
}
