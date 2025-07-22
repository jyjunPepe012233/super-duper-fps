using UnityEngine;

namespace SDFPS.Services.Character
{

	public class PlayerCharacterAnimationHandler : MonoBehaviour
	{
		[SerializeField] private Animator m_animator;

		[Header("Transition Speed")]
		[SerializeField, Range(6, 12)] private float m_aimTransSpeed = 10;
		[SerializeField, Range(6, 12)] private float m_moveTransSpeed = 10;
		[SerializeField, Range(6, 12)] private float m_sprintTransSpeed = 10;

		private bool m_isAiming;
		private float m_aimBlend;

		private bool m_isMoving;
		private float m_moveBlend;
		private bool m_isSprinting;
		private float m_sprintBlend;
		
		public void StartAim()
		{
			if (m_isAiming) return;
			
			m_isAiming = true;
		}

		public void EndAim()
		{
			if (!m_isAiming) return;
			
			m_isAiming = false;
		}
		
		public void SetMoveAnimation(bool isMoving, bool isSprinting)
		{
			m_isMoving = isMoving;
			m_isSprinting = isSprinting;
		}

		private void Update()
		{
			m_aimBlend = Mathf.Lerp(m_aimBlend, m_isAiming ? 1 : 0, Time.deltaTime * m_aimTransSpeed);	
			m_animator.SetBool("IsAiming", m_isAiming);
			m_animator.SetFloat("AimBlend", m_aimBlend);

			m_moveBlend = Mathf.Lerp(m_moveBlend, m_isMoving ? 1 : 0, Time.deltaTime * m_moveTransSpeed);
			m_animator.SetFloat("MoveBlend", m_moveBlend);

			m_sprintBlend = Mathf.Lerp(m_sprintBlend, m_isSprinting ? 1 : 0, Time.deltaTime * m_sprintTransSpeed);
			m_animator.SetFloat("SprintBlend", m_sprintBlend);
		}
	}

}