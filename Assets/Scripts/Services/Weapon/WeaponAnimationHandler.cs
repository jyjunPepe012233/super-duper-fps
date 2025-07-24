using UnityEngine;

namespace SDFPS.Services.Weapon
{

	public class WeaponAnimationHandler : MonoBehaviour
	{
		[SerializeField] private Animator m_animator;
		
		public void PlayAttackAnimation()
		{
			m_animator.CrossFadeInFixedTime("Attack", 0.02f);
		}

		public void PlayReloadAnimation(bool isAmmoEmpty)
		{
			m_animator.CrossFadeInFixedTime(isAmmoEmpty ? "Reload Empty" : "Reload", 0.1f);
		}
	}

}