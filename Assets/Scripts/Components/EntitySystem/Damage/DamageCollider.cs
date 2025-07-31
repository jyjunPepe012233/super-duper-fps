using System;
using Components.EntitySystem;
using SDFPS.Components.EntitySystem.Damage;
using UnityEngine;

namespace SDFPS.Components.EntitySystem.Damage
{

	[RequireComponent(typeof(Collider))]
	public class DamageCollider : MonoBehaviour
	{
		public int damage;

		public event Action whenGaveDamage;
		
		public void OnCollisionEnter(Collision other)
		{
			DamageableObject damageable = other.collider.GetComponent<DamageableObject>();

			if (damageable == null)
			{
				damageable = other.collider.GetComponentInParent<DamageableObject>();
			}

			if (damageable != null)
			{
				damageable.GiveDamage(damage);
				
				whenGaveDamage?.Invoke();
			}
		}
	}

}