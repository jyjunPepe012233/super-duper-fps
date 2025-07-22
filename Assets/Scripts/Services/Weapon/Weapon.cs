using UnityEngine;

namespace Services.Weapon
{

	public class Weapon : MonoBehaviour
	{
		[SerializeField] private Transform m_muzzleTransform;
		[Header("Prefabs")]
		[SerializeField] private GameObject m_bullets;
		[SerializeField] private GameObject m_casing;

		public void Shoot()
		{
			
		}
	}

}