using Components.EntitySystem;
using UnityEngine;

namespace SDFPS.Components.Props.DamageableProp
{

	public class Target : MonoBehaviour
	{
		[SerializeField] private Collider[] m_colliders;
		[SerializeField] private DamageableObject m_damageable;
		[SerializeField] private Animator m_animator;

		[Header("Knock Down")] 
		[SerializeField] private int m_minDamageToKnockDown = 1;
		[SerializeField] private float m_minKnockDownTime = 3;
		[SerializeField] private float m_maxKnockDownTime = 5;

		private bool m_isKnockedDown;

		private float m_remainTimeToStandUp;
		
		private void Awake()
		{
			if (m_colliders == null || m_colliders.Length == 0)
			{
				m_colliders = GetComponentsInChildren<Collider>();
			}
			
			m_damageable.onDamagedAction += GetDamage; 
		}

		private void GetDamage(int damage)
		{
			Debug.Log(1);
			if (damage >= m_minDamageToKnockDown)
			{
				KnockDown();
			}
		}

		private void KnockDown()
		{
			m_isKnockedDown = true;

			m_remainTimeToStandUp = Random.Range(m_minKnockDownTime, m_maxKnockDownTime);

			m_animator.Play("Knock Down");

			foreach (Collider collider in m_colliders)
			{
				collider.enabled = false;
			}
		}

		private void StandUp()
		{
			m_isKnockedDown = false;
			
			m_animator.Play("Stand Up");
			
			foreach (Collider collider in m_colliders)
			{
				collider.enabled = true;
			}
		}

		private void Update()
		{
			if (m_isKnockedDown)
			{
				m_remainTimeToStandUp -= Time.deltaTime;

				if (m_remainTimeToStandUp <= 0)
				{
					StandUp();
				}
			}
		}
	}

}