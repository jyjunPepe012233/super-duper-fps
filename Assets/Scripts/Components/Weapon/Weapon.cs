using System.Collections;
using SDFPS.Services.Weapon;
using UnityEngine;

namespace SDFPS.Components.Weapon
{

	[RequireComponent(typeof(WeaponAnimationHandler))]
	public class Weapon : MonoBehaviour
	{
		[SerializeField] private WeaponAnimationHandler m_animationHandler;
		
		[Header("Attack")]
		[SerializeField] private int m_rpm = 500;
		
		[Header("Ammo Loading")]
		[SerializeField] private bool m_useAmmo = true; 
		[SerializeField] private int m_maxAmmo = 30;
		
		[Header("Reserved Ammo")]
		[SerializeField] private bool m_infiniteMagazine = false;
		[SerializeField] private int m_magazine = 0; 
		
		private int m_loadedAmmo;
		private bool m_isAttackCoolDown;

		private WaitForSeconds m_attackCoolYield;

		public int rpm => m_rpm;
		public bool useAmmo => m_useAmmo;
		public int maxMagazine => maxMagazine;
		public int loadedAmmo => m_loadedAmmo;
		public bool isAttackCoolDown => m_isAttackCoolDown;

		public bool infiniteMagazine => m_infiniteMagazine;
		public int magazine => m_magazine;
		
		public bool AttemptAttack()
		{
			if (m_isAttackCoolDown)
			{
				return false;
			}
			
			if (m_useAmmo)
			{
				if (m_loadedAmmo > 0)
				{
					m_loadedAmmo -= 1;
					Attack();
				}
			}
			else
			{
				Attack();
			}

			StartAttackCool();
			m_animationHandler.PlayAttackAnimation();
			return true;
		}

		private void Attack()
		{
			Debug.Log("빵!");
		}

		private void StartAttackCool()
		{
			StartCoroutine(AttackCooldownRoutine());
		}

		private IEnumerator AttackCooldownRoutine()
		{
			m_isAttackCoolDown = true;

			if (m_attackCoolYield == null) m_attackCoolYield = new WaitForSeconds(60f / m_rpm);
			yield return m_attackCoolYield;

			m_isAttackCoolDown = false;
		}

		public void ChargeAmmo()
		{
			m_loadedAmmo = m_maxAmmo;
		}
		
		public void ConsumeMagazine()
		{
			m_magazine -= 1;
		}

		public void PlayReloadAnimation(bool isAmmoEmpty)
		{
			m_animationHandler.PlayReloadAnimation(isAmmoEmpty);
		}
	}

}