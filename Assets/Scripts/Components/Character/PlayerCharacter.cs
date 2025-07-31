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

		private Coroutine m_reloadingCoroutine;

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

		public void OnToogleFireMode(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				ToggleFireMode();
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
				
				// 무기 전환 시 현재 진행 중인 장전을 취소
				if (m_isReloading)
				{
					m_isReloading = false;

					StopCoroutine(m_reloadingCoroutine);
					
					m_animationHandler.StopReloadAnimationImmediately();
				}
			}
		}

		private IEnumerator ScrollWeaponRoutine(int direction)
		{
			m_isChangingWeapon = true;
			
			if (m_weaponManager.GetCurrentWeapon() != null)
			{
				// 애니메이션을 실행시키고, 애니메이션의 이벤트를 구독하여 holster가 끝나기를 기다림.
				m_animationHandler.PlayHolsterAnimation();
				
				bool waitForHolsterFinish = false;
				
				m_animationEventReceiver.holsterEnded += () =>
				{
					waitForHolsterFinish = true;
				};
				
				yield return new WaitUntil(() => waitForHolsterFinish);
			}
			
			
			// WeaponManager의 ScrollWeapon은 실제로 활성화된 무기를 변경하는 작업임.
			m_weaponManager.ScrollWeapon(direction);
			
			
			// 애니메이션을 실행시키고, 애니메이션의 이벤트를 구독하여 unholster가 끝나기를 기다림.
			// unholster가 끝나면 isChangingWeapon 플래그가 비활성화되어 다른 행동이 가능해짐.
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
			if (m_weaponManager.AttemptAttack(out bool shouldCancelInput))
			{
				bool hasLoadedAmmo = m_weaponManager.HasLoadedAmmo();
				
				m_animationHandler.PlayAttackAnimation(!hasLoadedAmmo);

				if (!hasLoadedAmmo)
				{
					m_attackInput = false;
				}

				// shouldCancelInput이 활성화되면 공격 입력 신호를 비활성화한다.
				// shouldCancelInput은 총의 발사가 단발로 설정되어 있을 때, 플레이어가 물리적 입력을 끝내지 않아도
				// 한 번의 공격 후에 다시 공격이 발동하지 않도록 하기 위해 활성화된다.
				if (shouldCancelInput)
				{
					m_attackInput = false;
				}
			}
		}

		private void Reload()
		{
			if (m_weaponManager.HasMagazine() && !m_isReloading && !m_isChangingWeapon)
			{
				m_reloadingCoroutine = StartCoroutine(ReloadRoutine());
			}
		}

		private IEnumerator ReloadRoutine()
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

		private void ToggleFireMode()
		{
			m_weaponManager.TryToggleFireMode();
		}
 	}

}