using SDFPS.Components.Character;
using SDFPS.Components.EntitySystem.Damage;
using SDFPS.Utils.ComponentLocator;
using SDFPS.Utils.ObjectPooling;
using UnityEngine;

namespace SDFPS.Components.Projectile
{

	public class Bullet : PooleeBehaviour
	{
		[SerializeField] private Rigidbody m_rigidbody;
		[SerializeField] private Collider m_collider;
		[SerializeField] private TrailRenderer m_trailRenderer;
		[SerializeField] private DamageCollider m_damageCollider;

		[Header("Flight")]
		[SerializeField] private float m_speed = 100;
		[SerializeField] private float m_lifeTime = 5f;

		[Header("Damage")]
		[SerializeField] private int m_minDamage = 1;
		[SerializeField] private int m_maxDamage = 1;

		private float m_elapsedTime;


		public override void OnFirstInstantiated()
		{
			Physics.IgnoreCollision(
				WorldComponentLocator.singleton.Get<PlayerCharacter>().GetComponent<Collider>(),
				m_collider
			);
		}

		public override void OnTakeFromPool()
		{
			gameObject.SetActive(true);
			
			m_elapsedTime = 0;
		}

		public override void OnReturnedToPool()
		{
			gameObject.SetActive(false);
		}

		private void SetRandomDamage()
		{
			m_damageCollider.damage = Random.Range(m_minDamage, m_maxDamage + 1);
		}

		public void ShootAt(Vector3 position, Quaternion rotation)
		{
			transform.SetPositionAndRotation(position, rotation);
			
			m_rigidbody.velocity = transform.forward * m_speed;
			m_rigidbody.angularVelocity = Vector3.zero;
			
			// Bullet이 Pool에서 Take될 때 마지막 위치에서 새로운 발사 위치까지 Trail이 그려지기 때문에 새로운 발사 위치로 이동 후 Trail을 Clear함.
			m_trailRenderer.Clear();
			
			SetRandomDamage();
		}
		
		public void Update()
		{
			m_rigidbody.velocity = transform.forward * m_speed;
			m_rigidbody.angularVelocity = Vector3.zero;

			m_elapsedTime += Time.deltaTime;
			if (m_elapsedTime >= m_lifeTime)
			{
				ReturnToPool();
			}
		}

		public void OnCollisionEnter()
		{
			ReturnToPool();
		}
	}

}