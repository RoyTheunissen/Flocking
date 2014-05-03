using UnityEngine;
using System.Collections;

namespace Trepid.Flocking
{
	public class PointAtBoid : MonoBehaviour
	{
		#region Vars
		#region Inspector
		[SerializeField]
		private Flock m_flock;

		[SerializeField]
		private Vector3 m_offset;
		#endregion

		#region Private
		private Transform m_boid;
		#endregion
		#endregion

		#region Unity Callbacks
		void Start()
		{
			m_boid = m_flock.Boids[
				Random.Range(0, m_flock.Boids.Count)
				].CachedTransform;

			transform.parent = m_boid;
			transform.localPosition = m_offset;
			transform.LookAt(m_boid.position);
		}

		void Update()
		{

		}
		#endregion
	}
}
