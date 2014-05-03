using UnityEngine;
using System.Collections;

namespace Trepid.Flocking
{
	public class LandingPosition
	{
		#region Vars
		#region Constants
		public const float MIN_DISTANCE_BETWEEN_POINTS = 1.0f;
		#endregion

		#region Private
		private Vector3 m_position;
		private bool m_isOccupied;
		#endregion
		#endregion

		#region Constructors
		public LandingPosition(Vector3 position)
		{
			m_position = position;
		}
		#endregion

		#region Methods
		public Vector3 GetPosition()
		{
			return m_position;
		}
		public void SetPosition(Vector3 pos)
		{
			m_position = pos;
		}

		public bool IsOccupied()
		{
			return m_isOccupied;
		}

		public void Land(FlockMovement boid)
		{
			m_isOccupied = true;
		}
		public void TakeOff()
		{
			m_isOccupied = false;
		}
		#endregion
	}
}
