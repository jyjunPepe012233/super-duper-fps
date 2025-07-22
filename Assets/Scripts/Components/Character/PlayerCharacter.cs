using SDFPS.Services.Character;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SDFPS.Components.Character
{

	public class PlayerCharacter : MonoBehaviour, PlayerAction.ICameraActions, PlayerAction.ICharacterBehaviourActions
	{
		[Header("Services")]
		[SerializeField] private FirstPersonRotationHandler m_rotationHandler;
		[SerializeField] private PlayerCharacterLocomotionHandler m_locomotionHandler;
		[SerializeField] private PlayerCharacterAnimationHandler m_animationHandler;

		// Input.
		private Vector2 m_mouseMoveInput;
		private Vector2 m_moveInput;
		private bool m_sprintInput;
		
		// Control Flags
		private bool m_isSprinting;
		private bool m_isAiming;

		private PlayerAction m_actionMap;

		public void Awake()
		{
			m_actionMap = new PlayerAction();
			
			m_actionMap.Camera.SetCallbacks(this);
			m_actionMap.CharacterBehaviour.SetCallbacks(this);
			m_actionMap.Enable();
		}

		public void OnMouseMove(InputAction.CallbackContext context)
		{
			m_mouseMoveInput = context.ReadValue<Vector2>();
		}
		
		public void OnMove(InputAction.CallbackContext context)
		{
			m_moveInput = context.ReadValue<Vector2>();
		}
		
		public void OnSprint(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				m_sprintInput = true;
				
				if (m_moveInput.y > 0)
				{
					m_isSprinting = true;
					m_isAiming = false;
				}
			}
			
			if (context.canceled)
			{
				m_sprintInput = false;
				m_isSprinting = false;
			}
		}
		
		public void OnAim(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				m_isAiming = true;
				m_isSprinting = false;
			}

			if (context.canceled)
			{
				m_isAiming = false;
				
				if (m_sprintInput)
				{
					m_isSprinting = true;
				}
			}
		}
		
		private void Update()
		{
			m_rotationHandler.RotateToward(m_mouseMoveInput);

			m_isSprinting = m_isSprinting && m_moveInput.y > 0;
			if (m_locomotionHandler.IsGrounded())
			{
				m_locomotionHandler.MoveToward(m_moveInput, m_isSprinting);
			}
			
			m_animationHandler.SetMoveAnimation(m_moveInput.magnitude != 0, m_isSprinting);	
			if (m_isAiming)
			{
				m_animationHandler.StartAim();
			}
			else
			{
				m_animationHandler.EndAim();
			}
		}
	}

}