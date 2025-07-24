using System.Collections.Generic;
using SDFPS.Utils.Singleton;
using UnityEngine;

namespace SDFPS.Utils.ComponentLocator
{

	public class WorldComponentLocator : SingletonInitializeOnLoad<WorldComponentLocator>
	{
		private Dictionary<string, IWorldLocatable> m_worldLocatables;
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		protected static void InitializeOnLoad()
		{
			InitializeSingleton();
		}
		
		public T Get<T>() where T : MonoBehaviour, IWorldLocatable
		{
			if (m_worldLocatables == null) m_worldLocatables = new();
			
			string key = typeof(T).Name;
			if (!m_worldLocatables.ContainsKey(key) || m_worldLocatables[key] == null)
			{
				m_worldLocatables[key] = FindObjectOfType<T>();
			}
			
			IWorldLocatable result = m_worldLocatables[key];
			if (result != null)
			{
				return result.source.GetComponent<T>();
			}
			else
			{
				return null;
			}
		}
	}

}