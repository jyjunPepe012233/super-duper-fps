using System;
using System.Collections.Generic;
using SDFPS.Utils.Singleton;
using UnityEngine;

namespace SDFPS.Utils.ObjectPooling
{

	public class PoolManager : SingletonInitializeOnLoad<PoolManager>
	{
		private Dictionary<string, ObjectPool> m_livingPools;
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		protected static void InitializeOnLoad()
		{
			InitializeSingleton();
		}

		public void Awake()
		{
			m_livingPools = new Dictionary<string, ObjectPool>();
		}
		
		public ObjectPool AddNewPool(ObjectPoolSetting poolSetting)
		{
			if (m_livingPools.ContainsKey(poolSetting.poolName))
			{
				throw new Exception(poolSetting.poolName + "으로 명명된 Pool이 이미 존재합니다. Pool을 생성할 수 없습니다.");
			}
			
			ObjectPool newPool = new ObjectPool(poolSetting);
			m_livingPools.Add(poolSetting.poolName, newPool);
			return newPool;
		}
		
		public bool TryGetPool(string poolName, out ObjectPool pool) => m_livingPools.TryGetValue(poolName, out pool);
		
		public GameObject TakeFromPool(ObjectPoolSetting poolSetting)
		{
			ObjectPool pool = null;

			if (!TryGetPool(poolSetting.poolName, out pool))
			{
				pool = AddNewPool(poolSetting);
			}

			return pool.TakeFromPool();
		}

		public void ReturnToPool(PooleeBehaviour poolee)
		{
			bool successfulllyFoundPool = false;
			foreach (ObjectPool pool in m_livingPools.Values)
			{
				if (pool.ReturnToPool(poolee))
				{
					successfulllyFoundPool = true;
					break;
				}
			}

			if (!successfulllyFoundPool)
			{
				Debug.LogWarning(poolee.name + "은(는) Pool에 포함되지 않았거나 사용된 적이 없는 Poolee입니다.");
			}
		}
	}

}