using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace SDFPS.Utils.ObjectPooling
{

	public class ObjectPool
	{
		private readonly ObjectPoolSetting m_poolSetting;

		// Key: Poolee
		// Value: IsValidToTake(IsReturnedToPool)
		private readonly Stack<PooleeBehaviour> m_validPoolees;
		private readonly HashSet<PooleeBehaviour> m_takedPoolees;

		public ObjectPool([NotNull] ObjectPoolSetting poolSetting)
		{
			m_poolSetting = poolSetting;
			m_validPoolees = new Stack<PooleeBehaviour>(poolSetting.initialCount);
			m_takedPoolees = new HashSet<PooleeBehaviour>();

			InitializePooleesOnStart(m_poolSetting.initialCount);
		}
		
		private void InitializePooleesOnStart(int count)
		{
			for (int i = 0; i < count; i++)
			{
				PooleeBehaviour newPoolee = UnityEngine.Object.Instantiate(m_poolSetting.pooleePrefab).GetComponent<PooleeBehaviour>();
				m_validPoolees.Push(newPoolee);
				newPoolee.OnFirstInstantiated();
				newPoolee.OnReturnedToPool();
			}
		}

		private void AddNewPooleesIntoQueue(int count)
		{
			for (int i = 0; i < count; i++)
			{
				PooleeBehaviour newPoolee = UnityEngine.Object.Instantiate(m_poolSetting.pooleePrefab).GetComponent<PooleeBehaviour>();
				m_validPoolees.Push(newPoolee);
				newPoolee.OnReturnedToPool();
			}
		}

		public GameObject TakeFromPool()
		{
			if (!m_validPoolees.TryPop(out PooleeBehaviour poolee)) 
			{
				// Valid Poolee Not Exists
				
				AddNewPooleesIntoQueue(m_poolSetting.summonCount);
				if (!m_validPoolees.TryPop(out poolee))
				{
					throw new Exception("Poolee를 생성할 수 없습니다.");
				}
			}
			
			m_takedPoolees.Add(poolee);
			poolee.OnTakeFromPool();
			
			return poolee.gameObject;
		}

		public bool ReturnToPool(PooleeBehaviour poolee)
		{
			if (!m_takedPoolees.Remove(poolee))
			{
				return false;
			}
			
			m_validPoolees.Push(poolee);
			poolee.OnReturnedToPool();
			return true;
		}
	}

}