using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trepid.Flocking
{
	/// <summary>
	/// This provides evenly spaced landing positions along a bezier curve.
	/// </summary>
	public class LandingStripBezier : LandingStrip
	{
		#region Vars
		#region Inspector
		[SerializeField]
		private Transform m_midPoint;
		#endregion

		#region Private
		private Vector3[] m_bezierPoints;
		#endregion
		#endregion

		#region Methods
		#region Unity Callbacks
		protected override void Awake()
		{
			UpdateBezierPoints();
			base.Awake();
		}

		protected override void Update()
		{
			base.Update();
		}

		protected override void OnDrawGizmos()
		{
			UpdateBezierPoints();
			base.OnDrawGizmos();
		}
		#endregion

		#region Private
		private void UpdateBezierPoints()
		{
			m_bezierPoints = new Vector3[] {
			m_startPoint.position,

			m_midPoint == null ?
			(m_startPoint.position+m_endPoint.position)/2 : m_midPoint.position,

			m_endPoint.position
		};
		}

		protected override Vector3 FindPositionByNormal(float normal)
		{
			return Bezier.DoForNPoints(normal, m_bezierPoints);
		}

		protected override float GetLength()
		{
			float length = 0;
			Vector3 prev = FindPositionByNormal(0);
			Vector3 cur;
			for (float i = 0.1f; i <= 1; i += 0.1f)
			{
				cur = FindPositionByNormal(i);
				length += Vector3.Distance(cur, prev);
				prev = cur;
			}
			return length;
		}
		#endregion
		#endregion
	}
}