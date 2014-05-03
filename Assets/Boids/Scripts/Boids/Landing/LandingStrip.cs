using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trepid.Flocking
{
	/// <summary>
	/// This provides evenly spaced landing positions along a line.
	/// </summary>
	public class LandingStrip : MonoBehaviour
	{
		#region Vars
		#region Inspector
		[SerializeField]
		protected Transform m_startPoint;

		[SerializeField]
		protected Transform m_endPoint;
		#endregion

		#region Private
		private List<LandingPosition> m_landingPositions = new List<LandingPosition>();
		#endregion
		#endregion

		#region Methods
		#region Unity Callbacks
		protected virtual void Awake()
		{
			ComputeLandingPositions();
		}

		protected virtual void Update()
		{
		}

		/// <summary>
		/// Draws spheres at all the nodes and a line inbetween.
		/// </summary>
		protected virtual void OnDrawGizmos()
		{
			if (m_startPoint != null)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(m_startPoint.position, 0.25f);
			}

			if (m_endPoint != null)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(m_endPoint.position, 0.25f);
			}

			for (int i = 0; i < m_landingPositions.Count; i++)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(m_landingPositions[i].GetPosition(), 0.125f);
			}

			if (m_startPoint != null && m_endPoint != null)
			{
				Gizmos.color = new Color(1.0f, 0.0f, 1.0f);

				float n = 0.0f;
				Vector3 prev = FindPositionByNormal(n);
				Vector3 current;
				for (float i = 0.1f; i <= 1.1; i += 0.1f)
				{
					current = FindPositionByNormal(i);
					Gizmos.DrawLine(prev, current);
					prev = current;
				}
			}

			Gizmos.color = Color.white;
		}
		#endregion

		#region Public
		/// <summary>
		/// Gives you a vacant landing position on this strip.
		/// </summary>
		/// <returns>The LandingPosition object to land at,
		/// or null if there are none available.</returns>
		public LandingPosition GetLandingPosition()
		{
			for (int i = 0; i < m_landingPositions.Count; i++)
			{
				if (!m_landingPositions[i].IsOccupied())
				{
					return m_landingPositions[i];
				}
			}
			return null;
		}
		#endregion

		#region Private
		protected virtual void ComputeLandingPositions()
		{
			float normal = 0.0f;
			float length = GetLength();
			float step = LandingPosition.MIN_DISTANCE_BETWEEN_POINTS / length;
			while (normal < 1)
			{
				m_landingPositions.Add(
					new LandingPosition(FindPositionByNormal(normal))
					);

				normal += step;
			}
		}

		protected virtual Vector3 FindPositionByNormal(float normal)
		{
			return Vector3.Lerp(m_startPoint.position, m_endPoint.position, normal);
		}

		protected virtual float GetLength()
		{
			return (m_endPoint.transform.position - m_startPoint.transform.position).magnitude;
		}
		#endregion
		#endregion
	}
}