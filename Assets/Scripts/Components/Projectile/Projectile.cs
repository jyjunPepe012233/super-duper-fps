using SDFPS.Components.Character;
using SDFPS.Utils.ComponentLocator;
using UnityEngine;

namespace SDFPS.Components.Projectile
{

	public class Projectile : MonoBehaviour
	{
		[SerializeField] private Rigidbody m_rigidbody;
		[SerializeField] private Collider m_collider;

		[Header("Flight")]
		[SerializeField] private float m_speed = 100;
		[SerializeField] private float m_lifeTime = 5f;

		private float m_elapsedTime;
		
		public void Start()
		{
			Physics.IgnoreCollision(
				WorldComponentLocator.singleton.Get<PlayerCharacter>().GetComponent<Collider>(),
				m_collider
				);
		}
		
		public void Update()
		{
			m_rigidbody.velocity = transform.forward * m_speed;
			
			m_elapsedTime += Time.deltaTime;
			if (m_elapsedTime >= m_lifeTime)
			{
				Destroy(gameObject);
			}
		}

		public void OnCollisionEnter()
		{
			Destroy(gameObject);
		}
	}

}