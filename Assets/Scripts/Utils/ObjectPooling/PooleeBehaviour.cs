using UnityEngine;
using Utils.IGameObjectSource;

namespace SDFPS.Utils.ObjectPooling
{

	public class PooleeBehaviour : MonoBehaviour
	{
		
		protected void ReturnToPool()
		{
			PoolManager.singleton.ReturnToPool(this);
		}

		public virtual void OnFirstInstantiated()
		{
			
		} 
		
		public virtual void OnTakeFromPool()
		{
			
		}

		public virtual void OnReturnedToPool()
		{
			
		}
	}

}