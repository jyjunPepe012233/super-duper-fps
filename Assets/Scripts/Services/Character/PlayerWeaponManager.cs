using UnityEngine;

namespace SDFPS.Services.Character
{
	using Weapon = SDFPS.Components.Weapon.Weapon;
	
	public class PlayerWeaponManager : MonoBehaviour
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
		
		public bool AttemptAttack()
		{
			Weapon weapon = m_weapons[m_holdingWeapon];
			return weapon.AttemptAttack();
		}

		public void ChargeAmmoAndConsumeMagazineImmediately()
		{
			Weapon weapon = m_weapons[m_holdingWeapon];
			weapon.ChargeAmmo();
			weapon.ConsumeMagazine();
		}

		public void PlayReloadAnimation()
		{
			Weapon weapon = m_weapons[m_holdingWeapon];
			weapon.PlayReloadAnimation(!HasLoadedAmmo());
		}
		
		public bool HasLoadedAmmo()
		{
			Weapon weapon = m_weapons[m_holdingWeapon];
			return weapon.loadedAmmo > 0 || !weapon.useAmmo;
		}

		public int GetLoadedAmmoCount()
		{
			Weapon weapon = m_weapons[m_holdingWeapon];
			return weapon.loadedAmmo;
		}

		public bool HasMagazine()
		{
			Weapon weapon = m_weapons[m_holdingWeapon];
			return weapon.magazine > 0 || weapon.infiniteMagazine;
		}

		public int GetMagazineCount()
		{
			Weapon weapon = m_weapons[m_holdingWeapon];
			return weapon.magazine;
		}

		public void ScrollWeapon(int direction)
		{
			Debug.Log("direction: " + direction);
			Debug.Log(GetNextWeaponSlot(direction));
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
		}

		public Weapon GetCurrentWeapon()
		{
			if (m_weapons.Length == 0)
			{
				return null;
			}
			return m_weapons[m_holdingWeapon];
		}
	}

}