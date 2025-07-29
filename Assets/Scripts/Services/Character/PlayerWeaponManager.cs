using System;
using SDFPS.UI.WeaponHUD;
using UnityEngine;

namespace SDFPS.Services.Character
{
	using Weapon = SDFPS.Components.Weapon.Weapon;
	
	public class PlayerWeaponManager : MonoBehaviour, IWeaponHUDModel
	{ 
		[SerializeField] private Weapon[] m_weapons;

		private int m_holdingWeapon = 0;

		public void Awake()
		{
			foreach (Weapon weapon in m_weapons)
			{
				weapon.ChargeAmmo();
			}
			
			ChangeWeapon(0);
		}
		
		public Weapon GetCurrentWeapon()
		{
			if (m_weapons.Length == 0)
			{
				return null;
			}
			return m_weapons[m_holdingWeapon];
		}
		
		/// <summary>
		///	AttemptAttack은 PlayerCharacter가 공격을 실현하기로 결정했을 때 호출되는 메서드임.
		/// Weapon의 상태를 확인하고 공격을 실행하며, PlayerCharacter에게 실행 결과에 대한 응답을 줌.
		/// </summary>
		/// <param name="shouldCancelInput">ShouldCancelInput이 활성화되면 PlayerCharacter 클래스는 공격의 입력을 임의로 중단시키는데,
		/// 이는 FullAuto 모드가 아닐 때, 플레이어가 물리적 버튼을 해제하지 않아도 한 번 공격한 뒤 스스로 공격이 멈추게 하기 위함임.</param>
		/// <returns>공격을 시도했을 때 쿨타임이 해제되지 않았다면 false를 반환하여 총알의 발사가 실제로 실행되지 않았음을 알림.</returns>
		public bool AttemptAttack(out bool shouldCancelInput)
		{
			shouldCancelInput = false;
			
			Weapon weapon = GetCurrentWeapon();
			
			if (weapon.isAttackCoolDown)
			{
				return false;
			}
			
			if (weapon.useAmmo)
			{
				if (weapon.loadedAmmo == 0)
				{
					return false;
				}
				else
				{
					weapon.ConsumeAmmo();
					
					onLoadedAmmoUpdated?.Invoke();
				}
			}

			// usingFullAuto가 비활성화되어 있을 때(단발 발사일 때), shouldCancelInput을 활성화화여
			// PlayerCharacter가 공격 입력 신호를 비활성화하게 하여 공격을 멈춤. (버튼을 다시 눌러야 공격이 나가도록 구조함)
			if (!weapon.usingFullAuto) shouldCancelInput = true;
			
			weapon.Attack();
			
			return true;
		}

		public void ChargeAmmoAndConsumeMagazineImmediately()
		{
			Weapon weapon = GetCurrentWeapon();
			
			weapon.ChargeAmmo();
			
			onLoadedAmmoUpdated?.Invoke();
			
			weapon.ConsumeMagazine();
			
			onMagazineUpdated?.Invoke();
		}

		public void PlayReloadAnimation()
		{
			Weapon weapon = GetCurrentWeapon();
			
			weapon.PlayReloadAnimation(!HasLoadedAmmo());
		}
		
		public void ScrollWeapon(int direction)
		{
			ChangeWeapon(GetNextWeaponSlot(direction));
		}

		private int GetNextWeaponSlot(int direction)
		{
			return (m_holdingWeapon + direction + m_weapons.Length) % m_weapons.Length;
		}

		public void ChangeWeapon(int slot)
		{
			m_holdingWeapon = slot;

			for (int i = 0; i < m_weapons.Length; i++)
			{
				m_weapons[i].gameObject.SetActive(i == m_holdingWeapon);
			}
			
			onWeaponUpdated?.Invoke();
		}

		public bool TryToggleFireMode()
		{
			Weapon weapon = GetCurrentWeapon();
			
			if (weapon.allowFullAuto)
			{
				weapon.ToggleFireMode();
				
				onFireModeUpdated?.Invoke();
				
				return true;
			}
			else
			{
				return false;
			}
		}

		public event Action onLoadedAmmoUpdated;
		
		public event Action onMagazineUpdated;
		
		public event Action onFireModeUpdated;
		
		public event Action onWeaponUpdated;

		public bool HasLoadedAmmo()
		{
			Weapon weapon = m_weapons[m_holdingWeapon];
			return weapon.loadedAmmo > 0 || !weapon.useAmmo;
		}

		public bool IsAmmoUnlimited() => !GetCurrentWeapon().useAmmo;

		public int GetLoadedAmmoCount() => GetCurrentWeapon().loadedAmmo;

		public int GetMaxAmmoCount() => GetCurrentWeapon().maxAmmo;

		public bool HasMagazine()
		{
			Weapon weapon = m_weapons[m_holdingWeapon];
			return weapon.magazine > 0 || weapon.infiniteMagazine;
		}

		public bool IsMagazineUnlimited() => GetCurrentWeapon().infiniteMagazine;

		public int GetMagazineCount() => GetCurrentWeapon().magazine;

		public string GetWeaponName() => GetCurrentWeapon().name;

		public string GetFireMode()
		{
			return GetCurrentWeapon().usingFullAuto ? "Full-Auto" : "Semi-Auto";
		}
	}

}