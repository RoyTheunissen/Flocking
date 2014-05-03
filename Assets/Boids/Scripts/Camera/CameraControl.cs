using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trepid.Flocking
{
	public class CameraControl : MonoBehaviour
	{
		#region Var
		#region Inspector
		[SerializeField]
		private Transform[] m_cameraPositions;

		[SerializeField]
		private KeyCode[] m_cameraButtons;

		[SerializeField]
		private GUIStyle m_textStyle;

		[SerializeField]
		private bool m_disableCameraPerspectives;
		#endregion

		#region 
		private Dictionary<string, string> m_controlTexts;
		#endregion
		#endregion

		#region Methods
		#region Private
		private void SwitchToCamera(int index)
		{
			transform.parent = m_cameraPositions[index];
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
		#endregion
		#endregion

		#region Unity Callbacks
		void Awake()
		{
			if (m_disableCameraPerspectives)
			{
				m_cameraButtons = new KeyCode[] { };
			}

			m_controlTexts = new Dictionary<string, string>();
			m_controlTexts.Add("Left", "Steer Flock Left");
			m_controlTexts.Add("Right", "Steer Flock Right");
			m_controlTexts.Add("Up", "Steer Flock Forward");
			m_controlTexts.Add("Down", "Steer Flock Backwards");
			for (int i = 0; i < m_cameraButtons.Length; i++)
			{
				m_controlTexts.Add((i + 1).ToString(), "Camera #" + (i + 1));
			}
			m_controlTexts.Add("Space", "Land Flock");
		}

		void Update()
		{
			// If the camera button is pressed, go to its corresponding camera position.
			for (int i = 0; i < m_cameraButtons.Length; i++)
			{
				if (Input.GetKeyDown(m_cameraButtons[i]))
				{
					Debug.Log("GOING TO CAMERA #" + i);
					SwitchToCamera(i);
				}
			}
		}

		void OnGUI()
		{
			GUI.Label(new Rect(16, 16, 256, 128), "Controls:", m_textStyle);
			int row = 1;
			foreach (var texts in m_controlTexts)
			{
				GUI.Label(new Rect(32, 16 + 16 * row, 256, 128), texts.Key, m_textStyle);
				GUI.Label(new Rect(72, 16 + 16 * row, 256, 128), texts.Value, m_textStyle);
				row++;
			}
		}
		#endregion
	}
}
