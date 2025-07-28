using UnityEngine;

namespace SDFPS.Utils.ObjectPooling
{
	[CreateAssetMenu(menuName = "SDFPS/Object Pool Setting", fileName = "New Pool Setting", order = 1 << 5)]
	public class ObjectPoolSetting : ScriptableObject
	{ 
		public string poolName = "New Pool";
		public GameObject pooleePrefab;
		
		[Header("Poolee Handling")]
		public int initialCount = 0;
		public int summonCount = 1;
	}

}