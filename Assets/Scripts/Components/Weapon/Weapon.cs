using System.Collections;
using SDFPS.Components.Effects.Casing;
using SDFPS.Components.Projectile;
using SDFPS.Services.Weapon;
using SDFPS.Utils.ObjectPooling;
using UnityEngine;
using UnityEngine.Serialization;

namespace SDFPS.Components.Weapon
{
	public enum FireMode {
		SemiAuto,
		FullAuto
	}

	[RequireComponent(typeof(WeaponAnimationHandler))]
	public class Weapon : MonoBehaviour
	{
		[SerializeField] private WeaponAnimationHandler m_animationHandler;

		[Header("Fire")]
		[SerializeField] private bool m_allowFullAuto;
		[SerializeField] private int m_rpm = 500;
		[SerializeField] private Transform m_muzzleTransform; 
		[SerializeField] private ObjectPoolSetting m_bulletPool;
		
		[Header("Effects")]
		[SerializeField] private ParticleSystem m_muzzleFlare;
		[SerializeField] private int m_muzzleFlareCount = 5;
		[SerializeField] private Transform m_casingEjectPoint;
		[SerializeField] private ObjectPoolSetting m_casingPool;
		
		[Header("Ammo Loading")]
		[SerializeField] private bool m_useAmmo = true; 
		[SerializeField] private int m_maxAmmo = 30;
		
		[Header("Reserved Ammo")]
		[SerializeField] private bool m_infiniteMagazine = false;
		[SerializeField] private int m_magazine = 0; 
		
		private int m_loadedAmmo;
		private bool m_isAttackCoolDown;
		private bool m_usingFullAuto;

		private WaitForSeconds m_attackCoolYield;

		public bool allowFullAuto => m_allowFullAuto;
		public int rpm => m_rpm;
		public bool useAmmo => m_useAmmo;
		public int maxAmmo => m_maxAmmo;
		public int loadedAmmo => m_loadedAmmo;
		public bool isAttackCoolDown => m_isAttackCoolDown;
		public bool usingFullAuto => m_usingFullAuto;

		public bool infiniteMagazine => m_infiniteMagazine;
		public int magazine => m_magazine;

		public void Attack()
		{
			StartAttackCool();
			m_animationHandler.PlayAttackAnimation();
			
			if (m_bulletPool != null)
			{
				Bullet bullet = PoolManager.singleton.TakeFromPool(m_bulletPool).GetComponent<Bullet>();
				bullet.ShootAt(m_muzzleTransform.position, m_muzzleTransform.rotation);
			}
			
			if (m_casingPool != null)
			{ 
				Casing casing = PoolManager.singleton.TakeFromPool(m_casingPool).GetComponent<Casing>();
				casing.EjectAt(m_casingEjectPoint.position, m_casingEjectPoint.rotation);
			}

			if (m_muzzleFlare != null)
			{
				m_muzzleFlare.Emit(m_muzzleFlareCount);
			}
		}

		public void ConsumeAmmo()
		{
			if (m_loadedAmmo > 0)
			{
				m_loadedAmmo -= 1;
			}
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

		public void ToggleFireMode()
		{
			if (m_allowFullAuto)
			{
				m_usingFullAuto = !m_usingFullAuto;
			}
		}
	}

}