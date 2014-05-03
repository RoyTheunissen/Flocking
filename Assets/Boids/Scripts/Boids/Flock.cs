using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trepid.Flocking
{
	public class Flock : MonoBehaviour
	{
		#region Vars
		#region Inspector
		[SerializeField]
		private GameObject m_boidPrefab;

		[SerializeField]
		private float m_flockRadius = 25.0f;

		[SerializeField]
		private int m_flockSize = 15;

		[SerializeField]
		private Transform m_target;

		[SerializeField]
		private LandingZone m_landingZone;
		#endregion

		#region Public
		private List<FlockMovement> m_boids;
		public List<FlockMovement> Boids
		{
			get { return m_boids; }
		}

		private Vector3 m_center;
		public Vector3 Center
		{
			get { return m_center; }
		}

		private Vector3 m_averageVelocity;
		public Vector3 AverageVelocity
		{
			get { return m_averageVelocity; }
		}
		#endregion

		#region Private
		private int m_tickCount = 0;

		private int m_flockProximityCheckIndex;
		private int m_flockProximityCheckSectionSize;

		private SortedDictionary<float, FlockMovement> m_boidsByProximity
			= new SortedDictionary<float, FlockMovement>();
		private int m_proximitiesConsidered;
		private float m_tempBoidProximity;
		#endregion
		#endregion

		#region Methods
		#region Unity Callbacks
		void Awake()
		{
			// Populate the flock.
			PopulateFlock();

			// Find the initial neighbours.
			for (int i = 0; i < m_boids.Count; i++)
			{
				FindNeighboursForBoid(m_boids[i], false);
			}

			// Set up the flock traversal sections.
			m_flockProximityCheckIndex = 0;
			m_flockProximityCheckSectionSize = m_flockSize / 10;
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				for (int i = 0; i < m_boids.Count; i++)
				{
					m_boids[i].ToggleFlyStatus(m_landingZone);
				}
			}

			// Declarations
			m_center = Vector3.zero;
			m_averageVelocity = Vector3.zero;

			// Go through all boids.
			for (int i = 0; i < m_boids.Count; i++)
			{
				// Add the boid position.
				m_center += m_boids[i].CachedTransform.position;

				// Add the boid velocity.
				m_averageVelocity += m_boids[i].Velocity;

				if (m_flockProximityCheckSectionSize > 0
					&& (i - m_flockProximityCheckIndex) % m_flockProximityCheckSectionSize == 0)
				{
					FindNeighboursForBoid(m_boids[i], true);
					//Debug.Log("T=" + Time.time + " proximity check #" + i);
				}
			}

			// Next frame, do proximity checks for the next boids.
			if (m_flockProximityCheckSectionSize == 0)
			{
				m_flockProximityCheckIndex = 0;
			}
			else
			{
				m_flockProximityCheckIndex = (m_flockProximityCheckIndex + 1)
					% m_flockProximityCheckSectionSize;
			}

			m_tickCount++;

			// Divide the sum of all boid positions by the boid count,
			// thus finding the average position of all boids.
			m_center /= m_boids.Count;

			// Divide the sum of all boid velocities by the boid count,
			// thus finding the average velocity of all boids.
			m_averageVelocity /= m_boids.Count;
		}
		#endregion

		#region Private
		private void PopulateFlock()
		{
			// Declare the flock.
			m_boids = new List<FlockMovement>(m_flockSize);
			GameObject obj;
			FlockMovement boid;

			// Populate it.
			Transform cachedTransform = transform;
			for (int i = 0; i < m_flockSize; i++)
			{
				obj = (GameObject)GameObject.Instantiate(m_boidPrefab,
					transform.position + Random.insideUnitSphere * m_flockRadius,
					Quaternion.identity);

				boid = obj.GetComponent<FlockMovement>();

				boid.Flock = this;
				boid.FlockIndex = m_boids.Count;
				boid.Target = m_target;

				boid.CachedTransform.name = "Boid #" + i;
				boid.CachedTransform.parent = cachedTransform;

				m_boids.Add(boid);
			}
		}

		private void AddBoidByProximity(ref SortedDictionary<float, FlockMovement> dictionary, FlockMovement candidate, FlockMovement proximityTo)
		{
			// Check if we should do a proximity check at all.
			if (candidate == null || proximityTo == null
				|| !candidate.ShouldAvoid(proximityTo))
			{
				return;
			}

			// Otherwise add the boid to the sorted collection.
			m_tempBoidProximity = (candidate.CachedTransform.position
				- proximityTo.CachedTransform.position).sqrMagnitude;

			if (!dictionary.ContainsKey(m_tempBoidProximity))
			{
				dictionary.Add(m_tempBoidProximity, candidate);
			}
		}

		private void FindNeighboursForBoid(FlockMovement boid, bool onlyTraverseAffiliates)
		{
			// Clear the boids by proximity.
			m_boidsByProximity.Clear();
			m_proximitiesConsidered = 0;

			// If flagged as such, traverse through our neighbour's neighbours only.
			if (onlyTraverseAffiliates)
			{
				for (int i = 0; i < boid.Neighbours.Length; i++)
				{
					// Only do proximity checks if we have a neighbour at all.
					if (boid.Neighbours[i] == null)
					{
						continue;
					}

					// Loop through the neighbour's neighbours.
					for (int j = 0; j < boid.Neighbours[i].Neighbours.Length; j++)
					{
						AddBoidByProximity(ref m_boidsByProximity,
							boid.Neighbours[i].Neighbours[j], boid);
					}
				}
			}
			else // Otherwise go through all boids (initial setup requires this).
			{
				for (int i = 0; i < m_boids.Count; i++)
				{
					AddBoidByProximity(ref m_boidsByProximity, m_boids[i], boid);
				}
			}

			// Find the first NEIGHBOUR_COUNT amount of boids.
			//Debug.Log("Boid #"+boid.FlockIndex+" has the following proximities:");
			foreach (float dist in m_boidsByProximity.Keys)
			{
				//Debug.Log("\t" + dist + " units to " + m_boidsByProximity[dist].name);
				boid.SetNeighbour(m_proximitiesConsidered, m_boidsByProximity[dist]);
				m_proximitiesConsidered++;
				if (m_proximitiesConsidered == FlockMovement.NEIGHBOUR_COUNT)
				{
					break;
				}
			}
			// Clear the other slots if we didn't find NEIGHBOUR_COUNT.
			if (m_proximitiesConsidered < FlockMovement.NEIGHBOUR_COUNT)
			{
				boid.ClearNeighboursStartingFrom(m_proximitiesConsidered);
			}
		}
		#endregion
		#endregion
	}
}
