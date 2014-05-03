using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trepid.Flocking
{
	public class FlockMovement : MonoBehaviour
	{
		#region Events
		public delegate void TookOffEventHandler();
		public event TookOffEventHandler TookOff;
		protected virtual void OnTookOff()
		{
			if (TookOff != null)
			{
				TookOff();
			}
		}
		#endregion

		#region Inspector
		#region References
		[SerializeField] private Transform m_cachedTransform;

		[SerializeField] private Transform m_target;
		#endregion

		#region Parameters
		[SerializeField] private FlockingParameters m_parameters;
		#endregion
		#endregion

		#region Vars
		#region Constants
		public const int NEIGHBOUR_COUNT = 8;
		#endregion

		#region Public
		#region References
		public Transform CachedTransform { get { return m_cachedTransform; } }

		public Transform Target { get { return m_target; } set { m_target = value; } }

		public FlockingParameters Parameters
		{
			get { return m_parameters; } set { m_parameters = value; }
		}

		private Flock m_flock;
		public Flock Flock { get { return m_flock; } set { m_flock = value; } }

		private FlockMovement[] m_neighbours = new FlockMovement[NEIGHBOUR_COUNT];
		public FlockMovement[] Neighbours { get { return m_neighbours; } }
		#endregion

		#region Cache
		private int m_flockIndex;
		public int FlockIndex { get { return m_flockIndex; } set { m_flockIndex = value; } }

		private Vector3 m_velocity;
		public Vector3 Velocity { get { return m_velocity; } }

		private float m_landTime = -1;
		public float LandTime { get { return m_landTime; } set { m_landTime = value; } }

		private Vector3 m_landDirection;
		public Vector3 LandDirection
		{
			get { return m_landDirection; } set { m_landDirection = value; }
		}

		private LandingPosition m_landingPosition;
		public LandingPosition LandingPosition
		{
			get { return m_landingPosition; } set { m_landingPosition = value; }
		}

		private Vector3 m_goalPosition;
		public Vector3 GoalPosition
		{
			get { return m_goalPosition; } set { m_goalPosition = value; }
		}
		#endregion
		#endregion

		#region Private
		// Landing.
		private bool m_landed;

		// Goal following parameters.
		private Vector3 m_toGoal;
		private float m_goalDistanceSquared;
		#endregion
		#endregion

		#region Methods
		#region Unity Callbacks
		void Awake()
		{
			// Start with a random velocity.
			m_velocity = m_parameters.maxSpeed.GetValue() * Random.insideUnitSphere;
		}

		void Update()
		{
			UpdateGoal();

			PerformMovementRules();

			// If landed, coast towards our landing position.
			if (IsLanded())
			{
				m_velocity = m_toGoal * Time.deltaTime * 100.0f;
			}

			// Make sure the speed does not exceed our maximum.
			float mag = m_velocity.magnitude;
			float max = m_parameters.maxSpeed.GetValue() * GetIntendedSpeed();
			if (mag > max)
			{
				m_velocity /= mag / max;
			}

			// While landing.
			if (IsLanding())
			{
				// If close enough to actually land.
				if (m_goalDistanceSquared < m_parameters.landThreshold.GetValueSqr())
				{
					FlagAsLanded();
				}
				else // Otherwise slowly aid our course to circumvent slowness.
				{
					m_velocity = Vector3.Lerp(m_velocity, m_toGoal,
						Time.deltaTime * m_parameters.landCoursePrecision.GetValue());
				}
			}

			// Actually perform the movement.
			m_cachedTransform.position += m_velocity * Time.deltaTime;
		}
		#endregion

		#region Landing
		public void TryToLand(LandingZone landingZone)
		{
			m_landingPosition = landingZone.GetLandingPosition();
			if (m_landingPosition != null)
			{
				m_landingPosition.Land(this);
			}
		}
	
		public void TakeOff()
		{
			if (IsLanded())
			{
				UnflagAsLanded();
				m_landingPosition.TakeOff();
				m_landingPosition = null;
				OnTookOff();
			}
		}

		public void ToggleFlyStatus(LandingZone landingZone)
		{
			if (IsLandingOrLanded())
			{
				TakeOff();
			}
			else
			{
				TryToLand(landingZone);
			}
		}

		private void FlagAsLanded()
		{
			m_landed = true;
			m_landDirection = -m_velocity.normalized;
			m_landDirection.y = 0;
			m_landTime = Time.time;
		}

		private void UnflagAsLanded()
		{
			m_landed = false;
			m_landTime = -1;
		}

		public bool IsFlying()
		{
			return m_landingPosition == null;
		}
		public bool IsLanding()
		{
			return m_landingPosition != null && !m_landed;
		}
		public bool IsLandingOrLanded()
		{
			return m_landingPosition != null;
		}
		public bool IsLanded()
		{
			return m_landingPosition != null && m_landed;
		}
		#endregion

		#region Neighbours
		public bool ShouldAvoid(FlockMovement boid)
		{
			if (boid == null)
			{
				return false;
			}
			return boid.FlockIndex != m_flockIndex;
		}

		public void SetNeighbour(int index, FlockMovement boid)
		{
			m_neighbours[index] = boid;
		}
		public void ClearNeighboursStartingFrom(int index)
		{
			for (int i = index; i < m_neighbours.Length; i++)
			{
				m_neighbours[index] = null;
			}
		}
		#endregion

		#region Movement Rules
		private void UpdateGoal()
		{
			// Update the position.
			if (IsLandingOrLanded())
			{
				m_goalPosition = m_landingPosition.GetPosition();
			}
			else if (m_target != null)
			{
				m_goalPosition = m_target.position;
			}
			else
			{
				m_goalPosition = Vector3.zero;
			}

			// Update the delta to that position.
			m_toGoal = m_goalPosition - transform.position;

			// Update the distance to that position.
			if (m_goalPosition != Vector3.zero)
			{
				m_goalDistanceSquared = m_toGoal.sqrMagnitude;
			}
			else
			{
				m_goalDistanceSquared = 0;
			}
		}

		private float GetIntendedSpeed()
		{
			if (IsLanding())
			{
				return Mathf.Clamp(
					m_goalDistanceSquared
					/ m_parameters.slowDownForLandThreshold.GetValueSqr()
					, .15f, 1f);
			}
			else
			{
				return 1;
			}
		}

		private void ApplyMovementIntention(Vector3 movementIntention)
		{
			m_velocity += movementIntention
					* m_parameters.acceleration.GetValue() * Time.deltaTime;
		}

		private void PerformMovementRules()
		{
			// This rule doesn't apply when we have landed.
			if (!IsLanded())
			{
				// Rule 1: Move towards goal.
				ApplyMovementIntention(GetMovementTowardsGoal());
			}

			// These rules don't apply while landing or landed.
			if (IsFlying())
			{
				// Rule 2: Try not to bump into each other.
				ApplyMovementIntention(GetMovementAwayFromNeighbours());

				// Rule 3: Try to generally stick together.
				ApplyMovementIntention(GetMovementToFlockCenter());

				// Rule 4: Try to match the velocity of the flock.
				ApplyMovementIntention(GetMovementToMatchVelocity());
			}
		}
	
		private Vector3 GetMovementTowardsGoal()
		{
			// If we have a goal, return the direction towards it.
			if (m_goalPosition != Vector3.zero)
			{
				return (m_goalPosition - m_cachedTransform.position).normalized;
			}
			else // Otherwise, return zero.
			{
				return Vector3.zero;
			}
		}

		private Vector3 GetMovementToMatchVelocity()
		{
			// Decide upon an intention to match the flock velocity.
			// Try to ignore our own contribution to the average flock velocity.
			// Try not to adjust to the rest of the flock too much if we're not moving yet.
			// Otherwise, no single boid will start flying because no-one else is.
			return (((m_flock.AverageVelocity - (m_velocity/m_flock.Boids.Count))
				- m_velocity) * m_parameters.flockVelocityMatchingStrength.GetValue())
				* (m_velocity.magnitude / m_parameters.maxSpeed.GetValue());
		}

		private Vector3 GetMovementToFlockCenter()
		{
			// Decide upon an intention to move towards the flock center.
			// Try to ignore our own contribution to the average flock position.
			return ((m_flock.Center - (m_cachedTransform.position / m_flock.Boids.Count))
				- m_cachedTransform.position).normalized *
				m_parameters.flockFollowingStrength.GetValue();
		}

		private Vector3 GetMovementAwayFromNeighbours()
		{
			Vector3 result = Vector3.zero;
			float dist;
			Vector3 dir;

			// Go through all boids.
			for (int i = 0; i < m_neighbours.Length; i++)
			{
				// Don't try to move away from a neighbour we don't have.
				if (m_neighbours[i] == null)
				{
					break;
				}

				// Calculate the distance and direction to this boid.
				dist = (m_neighbours[i].CachedTransform.position
					- m_cachedTransform.position).sqrMagnitude;
				dir = (m_neighbours[i].CachedTransform.position
					- m_cachedTransform.position) / dist;

				// If we're too close to another boid.
				if (dist < m_parameters.minDistanceToNeighbour.GetValueSqr())
				{
					// We intend to move away from the boid.
					// The closer, the faster we want to move away.
					result -= dir * (m_parameters.minDistanceToNeighbour.GetValueSqr() - dist)
						/ m_parameters.minDistanceToNeighbour.GetValueSqr();
				}
			}

			return result * m_parameters.neighbourAvoidanceStrength.GetValue();
		}
		#endregion
		#endregion
	}
}
