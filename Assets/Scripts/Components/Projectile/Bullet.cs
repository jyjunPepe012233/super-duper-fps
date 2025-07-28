using System;
using SDFPS.Components.Character;
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

		[Header("Flight")]
		[SerializeField] private float m_speed = 100;
		[SerializeField] private float m_lifeTime = 5f;

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

		public void ShootAt(Vector3 position, Quaternion rotation)
		{
			transform.SetPositionAndRotation(position, rotation);
			
			m_rigidbody.velocity = transform.forward * m_speed;
			m_rigidbody.angularVelocity = Vector3.zero;
			
			// Bullet이 Pool에서 Take될 때 마지막 위치에서 새로운 발사 위치까지 Trail이 그려지기 때문에 새로운 발사 위치로 이동 후 Trail을 Clear함.
			m_trailRenderer.Clear();
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