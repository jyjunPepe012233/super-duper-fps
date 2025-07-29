using System;
using SDFPS.Services.Character;
using SDFPS.UI.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace SDFPS.UI.WeaponHUD
{

	public class WeaponHUD : MonoBehaviour
	{
		[SerializeField] private GameObject m_modelSource;

		private IWeaponHUDModel m_model;

		[Header("Target")]
		[SerializeField] private FormatableTextMeshProUGUI m_loadedAmmo = new("{0}");
		[SerializeField] private FormatableTextMeshProUGUI m_maxAmmo = new("{0}");
		[SerializeField] private FormatableTextMeshProUGUI m_magazine = new("{0}");
		[SerializeField] private FormatableTextMeshProUGUI m_fireMode = new("{0}");
		[SerializeField] private FormatableTextMeshProUGUI m_weaponName = new("{0}");

		public void Awake()
		{
			m_model = m_modelSource?.GetComponent<IWeaponHUDModel>();
			
			SubscribeEventsToModel();
		}

		private void SubscribeEventsToModel()
		{
			if (m_loadedAmmo.textMeshPro != null)
			{
				m_model.onLoadedAmmoUpdated += UpdateLoadedAmmo;
				
				m_model.onWeaponUpdated += UpdateLoadedAmmo;
			}

			if (m_maxAmmo.textMeshPro != null)
			{
				m_model.onWeaponUpdated += UpdateMaxAmmo;
			}

			if (m_magazine.textMeshPro != null)
			{
				m_model.onMagazineUpdated += UpdateMagazine;
				
				m_model.onWeaponUpdated += UpdateMagazine;
			}

			if (m_fireMode.textMeshPro != null)
			{
				m_model.onFireModeUpdated += UpdateFireMode;
				
				m_model.onWeaponUpdated += UpdateFireMode;
			}

			if (m_weaponName.textMeshPro != null)
			{
				m_model.onWeaponUpdated += UpdateWeaponName;
			}
		}

		public void UpdateLoadedAmmo()
		{
			if (m_model.IsAmmoUnlimited())
			{
				m_loadedAmmo.SetText("-");
				return;
			}
			
			m_loadedAmmo.SetText(m_model.GetLoadedAmmoCount());
		}

		public void UpdateMaxAmmo()
		{
			m_maxAmmo.SetText(m_model.GetMaxAmmoCount());
		}

		public void UpdateMagazine()
		{
			if (m_model.IsMagazineUnlimited())
			{
				m_magazine.SetText("-");
				return;
			}

			m_magazine.SetText(m_model.GetMagazineCount() * m_model.GetMaxAmmoCount());
		}

		public void UpdateFireMode()
		{
			m_fireMode.SetText(m_model.GetFireMode());
		}

		public void UpdateWeaponName()
		{
			m_weaponName.SetText(m_model.GetWeaponName());
		}
	}

}