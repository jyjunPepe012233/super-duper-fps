using System.Collections;
using SDFPS.Services.Character;
using SDFPS.Utils.ComponentLocator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SDFPS.Components.Character
{
	[RequireComponent(typeof(FirstPersonRotationHandler))]
	[RequireComponent(typeof(PlayerCharacterLocomotionHandler))]
	[RequireComponent(typeof(PlayerCharacterAnimationHandler))]
	[RequireComponent(typeof(PlayerWeaponManager))]
	public class PlayerCharacter : MonoBehaviour, IWorldLocatable, PlayerAction.ICameraActions, PlayerAction.ICharacterBehaviourActions
	{
		public GameObject gameObjectSource => gameObject;
		
		[Header("Services")]
		[SerializeField] private FirstPersonRotationHandler m_rotationHandler;
		[SerializeField] private PlayerCharacterLocomotionHandler m_locomotionHandler;
		[SerializeField] private PlayerCharacterAnimationHandler m_animationHandler;
		[SerializeField] private PlayerWeaponManager m_weaponManager;
		[Header("Event Receiver")]
		[SerializeField] private PlayerCharacterAnimationEventReceiver m_animationEventReceiver;

		// Input.
		private Vector2 m_mouseMoveInput;
		private Vector2 m_moveInput;
		private bool m_sprintInput;
		private bool m_attackInput;
		
		// Control Flags
		private bool m_isSprinting;
		private bool m_isAiming;
		private bool m_isChangingWeapon;
		private bool m_isReloading;

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

		public void OnScrollWeapon(InputAction.CallbackContext context)
		{
			int direction = (int)Mathf.Sign(context.ReadValue<float>());
			ScrollWeapon(direction);
			
			m_isReloading = false;
			m_animationHandler.StopReloadAnimationImmediately();
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				m_attackInput = true;
				m_isSprinting = false;
			}

			if (context.canceled)
			{
				m_attackInput = false;
			}
		}

		public void OnReload(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				Reload();
			}
		}

		private void Update()
		{
			m_rotationHandler.RotateToward(m_mouseMoveInput);

			m_isSprinting = m_sprintInput && m_moveInput.y > 0 && !m_isAiming && !m_isChangingWeapon && !m_isReloading;
			if (m_locomotionHandler.IsGrounded())
			{
				m_locomotionHandler.MoveToward(m_moveInput, m_isSprinting);
			}
			
			m_animationHandler.SetMoveAnimation(m_moveInput.magnitude != 0, m_isSprinting);
			
			if (m_isAiming && !m_isReloading)
			{
				m_animationHandler.StartAim();
			}
			else
			{
				m_animationHandler.EndAim();
			}

			if (m_attackInput && !m_isSprinting && !m_isChangingWeapon && !m_isReloading)
			{
				Attack();
			}
		}

		private void ScrollWeapon(int direction)
		{
			if (!m_isChangingWeapon)
			{
				StartCoroutine(ScrollWeaponRoutine(direction));
			}
		}

		private IEnumerator ScrollWeaponRoutine(int direction)
		{
			m_isChangingWeapon = true;
			
			if (m_weaponManager.GetCurrentWeapon() != null)
			{
				m_animationHandler.PlayHolsterAnimation();
				bool waitForHolsterFinish = false;
				m_animationEventReceiver.holsterEnded += () =>
				{
					waitForHolsterFinish = true;
				};
				yield return new WaitUntil(() => waitForHolsterFinish);
			}
			
			m_weaponManager.ScrollWeapon(direction);
			
			m_animationHandler.PlayUnholsterAnimation();
			bool waitForUnholsterFinish = false;
			m_animationEventReceiver.unholsterEnded += () =>
			{
				waitForUnholsterFinish = true;
			};
			yield return new WaitUntil(() => waitForUnholsterFinish);

			m_isChangingWeapon = false;
		}

		private void Attack()
		{
			if (m_weaponManager.AttemptAttack())
			{
				bool hasLoadedAmmo = m_weaponManager.HasLoadedAmmo();
				
				m_animationHandler.PlayAttackAnimation(!hasLoadedAmmo);

				if (!hasLoadedAmmo)
				{
					m_attackInput = false;
				}
			}
		}

		private void Reload()
		{
			StartCoroutine(ReloadRoutine());
		}

		private IEnumerator ReloadRoutine()
		{
			if (m_weaponManager.HasMagazine())
			{
				m_isReloading = true;
				
				m_animationHandler.PlayReloadAnimation(!m_weaponManager.HasLoadedAmmo());
				m_weaponManager.PlayReloadAnimation();

				bool isMagazineEquipped = false;
				m_animationEventReceiver.magazineEquipped += () =>
				{
					isMagazineEquipped = true;
				};
				
				yield return new WaitUntil(() => isMagazineEquipped);
				m_weaponManager.ChargeAmmoAndConsumeMagazineImmediately();
				
				bool isReloadEnded = false;
				m_animationEventReceiver.reloadEnded += () =>
				{
					isReloadEnded = true;
				};
			
				yield return new WaitUntil(() => isReloadEnded);
				m_isReloading = false;
			}
		}
 	}

}