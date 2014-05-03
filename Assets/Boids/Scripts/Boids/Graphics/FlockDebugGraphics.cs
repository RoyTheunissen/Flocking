using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trepid.Flocking
{
	public class FlockDebugGraphics : MonoBehaviour
	{
		#region Vars
		#region Inspector
		[SerializeField]
		private FlockMovement m_flockMovement;
		#endregion Inspector
		#endregion

		#region Methods
		#region Unity Callbacks
		void OnDrawGizmos()
		{
			Gizmos.color = m_flockMovement.IsLanded()
				? Color.yellow : new Color(1.0f, 0.0f, 1.0f, 0.1f);

			if (m_flockMovement.LandingPosition != null)
			{
				Gizmos.DrawLine(
					transform.position,
					m_flockMovement.LandingPosition.GetPosition()
					);
			}
			else if (m_flockMovement.Target)
			{
				Gizmos.DrawLine(transform.position, m_flockMovement.Target.position);
			}

			if (m_flockMovement.IsLanded())
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine(
					m_flockMovement.GoalPosition,
					m_flockMovement.GoalPosition + m_flockMovement.LandDirection * 3
					);
			}

			Gizmos.color = new Color(1.0f, 1.0f, 1.0f, .5f);

			Vector3 vel = m_flockMovement.Velocity;
			float speed = m_flockMovement.Velocity.magnitude;
			if (speed < 1)
			{
				vel *= 1 / speed;
			}
			Gizmos.DrawLine(transform.position, transform.position + vel * 0.1f);

			Gizmos.color = Color.white;
		}
		#endregion
		#endregion
	}
}
