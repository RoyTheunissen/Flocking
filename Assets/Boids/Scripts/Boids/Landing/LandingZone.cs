using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trepid.Flocking
{
	/// <summary>
	/// This manages a cluster of landing strips.
	/// </summary>
	public class LandingZone : MonoBehaviour
	{
		#region Vars
		#region Private
		private LandingStrip[] m_landingStrips;
		#endregion
		#endregion

		#region Methods
		#region Unity Callbacks
		void Awake()
		{
			m_landingStrips = GetComponentsInChildren<LandingStrip>();
		}
		#endregion

		#region Public
		/// <summary>
		/// Gives you a vacant landing position in this zone.
		/// </summary>
		/// <returns>The LandingPosition object to land at,
		/// or null if there are none available.</returns>
		public LandingPosition GetLandingPosition()
		{
			LandingPosition landingPosition;
			for (int i = 0; i < m_landingStrips.Length; i++)
			{
				landingPosition = m_landingStrips[i].GetLandingPosition();
				if (landingPosition != null)
				{
					return landingPosition;
				}
			}
			return null;
		}
		#endregion
		#endregion
	}
}