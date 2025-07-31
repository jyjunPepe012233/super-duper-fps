using System;

namespace SDFPS.UI.WeaponHUD
{

	public interface IWeaponHUDModel
	{
		event Action onLoadedAmmoUpdated;
		
		event Action onMagazineUpdated;

		event Action onFireModeUpdated;

		event Action onWeaponUpdated;

		public bool HasLoadedAmmo();

		public bool IsAmmoUnlimited();
		
		public int GetLoadedAmmoCount();

		public int GetMaxAmmoCount();
		
		public bool HasMagazine();

		public bool IsMagazineUnlimited();

		public int GetMagazineCount();

		public string GetWeaponName();

		public string GetFireMode();
	}

}