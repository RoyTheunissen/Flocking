using UnityEngine;
using System.Collections;

namespace Trepid.Flocking
{
	/// <summary>
	/// This is a randomized parameter that is squared.
	/// Use this for distances so we don't need to normalize vectors.
	/// Normalizing vectors requires square root operations, which are slow.
	/// </summary>
	[System.Serializable]
	public class RandomizedParameterSquared : RandomizedParameter
	{
		#region Vars
		protected float generatedValueSquared;
		#endregion

		#region Methods
		public override void GenerateValue()
		{
			base.GenerateValue();
			generatedValueSquared = generatedValue * generatedValue;
		}

		public float GetValueSqr()
		{
			if (!generated)
			{
				GenerateValue();
				generated = true;
			}
			return generatedValueSquared;
		}
		#endregion
	}
}
