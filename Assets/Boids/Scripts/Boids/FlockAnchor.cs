using UnityEngine;
using System.Collections;

namespace Trepid.Flocking
{
	public class FlockAnchor : MonoBehaviour
	{
		#region Vars
		#region Inspector
		[SerializeField]
		private Transform m_target;

		[SerializeField]
		private float m_radius;

		[SerializeField]
		private float m_speed;
		#endregion

		#region Private
		private float m_angle;
		#endregion
		#endregion

		#region Methods
		#region Unity Callbacks
		void Update()
		{
			// Keyboard controls.
			Vector3 motion = Vector3.zero;
			float speed = 15;
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				motion.x -= speed;
			}
			if (Input.GetKey(KeyCode.RightArrow))
			{
				motion.x = speed;
			}
			if (Input.GetKey(KeyCode.UpArrow))
			{
				motion.z = speed;
			}
			if (Input.GetKey(KeyCode.DownArrow))
			{
				motion.z = -speed;
			}
			transform.position += motion * Time.deltaTime;

			// Rotate the target around ourselves.
			m_angle = Mathf.Repeat(m_angle + Time.deltaTime * m_speed, 360f);
			m_target.position = transform.position
				+ new Vector3(Mathf.Sin(m_angle), 0, Mathf.Cos(m_angle)) * m_radius;
		}
		#endregion
		#endregion
	}
}
