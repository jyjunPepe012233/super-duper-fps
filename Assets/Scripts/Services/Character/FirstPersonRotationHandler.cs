using UnityEngine;
using PlayerSettings = SDFPS.Game.PlayerSettings.PlayerSettings;

namespace SDFPS.Services.Character
{

	public class FirstPersonRotationHandler : MonoBehaviour
	{
		[SerializeField] private Transform m_cameraRig;

		[Header("Angle Limit")]
		[SerializeField, Range(0, 90)] private float m_aboveLimit = 80;
		[SerializeField, Range(0, 90)] private float m_belowLimit = 80;
		
		[Header("Body Rotation")]
		[SerializeField] private Transform m_bodyRig;
		[SerializeField] private bool m_synchronizeForwardWithBody = true;
		
		public void RotateToward(Vector2 playerInput)
		{
			Vector3 inputToEulerAngles = new Vector3(-playerInput.y, playerInput.x, 0);
			Vector3 rotatedAngle = m_cameraRig.eulerAngles + inputToEulerAngles * PlayerSettings.cameraRotationSpeed;
			
			// Body를 먼저 회전시키고 Camera를 회전시켜야
			// Camera가 자식 오브젝트라서 발생하는 회전 중첩 문제가 발생하지 않음
			if (m_synchronizeForwardWithBody)
			{
				m_bodyRig.eulerAngles = new Vector3(m_bodyRig.eulerAngles.x, rotatedAngle.y, m_bodyRig.eulerAngles.z);
			}
			
			m_cameraRig.eulerAngles = ClampTiltAngle(rotatedAngle);
		}

		private Vector3 ClampTiltAngle(Vector3 angle)
		{
			float tiltAngle = angle.x;
			
			if (tiltAngle > 180)
			{
				tiltAngle = Mathf.Clamp(tiltAngle, 360 - m_belowLimit, 1000);
			}
			else
			{
				tiltAngle = Mathf.Clamp(tiltAngle, -1000, m_aboveLimit);
			}

			return new Vector3(tiltAngle, angle.y, angle.z);
		}
	}

}