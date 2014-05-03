using UnityEngine;
using System.Collections;

namespace Trepid.Flocking
{
	public class PointToFlock : MonoBehaviour
	{
		#region Vars
		#region Inspector
		[SerializeField]
		private Flock m_flock;
		#endregion
		#endregion

		#region Methods
		#region Unity Callbacks
		void Update()
		{
			transform.LookAt(m_flock.Center);
		}
		#endregion
		#endregion
	}
}
