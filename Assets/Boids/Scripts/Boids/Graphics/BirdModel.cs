using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trepid.Flocking
{
	public class BirdModel : MonoBehaviour
	{
		#region Vars
		#region Inspector
		[SerializeField]
		private FlockMovement m_flockMovement;

		[SerializeField]
		private Animation m_animation;
		#endregion Inspector

		#region Private
		private AnimationState m_animationState;
		#endregion
		#endregion

		#region Methods
		#region Unity Callbacks
		void Awake()
		{
			m_animationState = m_animation["Fly"];
			m_flockMovement.TookOff += BirdTookOff;
		}

		void OnDestroy()
		{
			m_flockMovement.TookOff -= BirdTookOff;
		}

		void Update()
		{
			// If we have landed, stop the animation.
			if (m_flockMovement.IsLanded() && Time.time > m_flockMovement.LandTime + 2)
			{
				m_animation.Play("Idle");
			}
			else // Otherwise, speed up the animation based on speed.
			{
				m_animationState.speed = .1f
					+ (1 -
					(m_flockMovement.Velocity.sqrMagnitude
					/ m_flockMovement.Parameters.maxSpeed.GetValueSqr())
					) * 2;
			}

			// Face our movement.
			if (!m_flockMovement.IsLanded())
			{
				transform.LookAt(
					m_flockMovement.CachedTransform.position
					+ m_flockMovement.Velocity
					);
			}
			else
			{
				transform.rotation = Quaternion.Lerp(
					transform.rotation,
					Quaternion.LookRotation(m_flockMovement.LandDirection),
					Time.deltaTime
					);
			}
		}
		#endregion

		#region Private
		protected void BirdTookOff()
		{
			m_animation.Play("Fly");
		}
		#endregion
		#endregion
	}
}
