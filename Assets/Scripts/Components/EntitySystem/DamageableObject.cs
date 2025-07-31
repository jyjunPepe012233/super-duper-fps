using System;
using UnityEngine;
using UnityEngine.Events;

namespace Components.EntitySystem
{

	public class DamageableObject : MonoBehaviour
	{
		public UnityEvent onDamaged;

		public event Action<int> onDamagedAction;
		
		public void GiveDamage(int damage)
		{
			onDamaged?.Invoke();
			
			onDamagedAction?.Invoke(damage);
		}
	}

}