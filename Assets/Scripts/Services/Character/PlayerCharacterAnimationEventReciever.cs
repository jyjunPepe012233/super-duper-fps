using System;
using UnityEngine;

namespace SDFPS.Services.Character
{

	public class PlayerCharacterAnimationEventReceiver : MonoBehaviour
	{
		public event Action holsterEnded;
		public event Action unholsterEnded;
		public event Action magazineEquipped;
		public event Action reloadEnded;
		
		public void HolsterEnded()
		{
			holsterEnded?.Invoke();
		}

		public void UnholsterEnded()
		{
			unholsterEnded?.Invoke();
		}

		public void MagazineEquipped()
		{
			magazineEquipped?.Invoke();
		}

		public void ReloadEnded()
		{
			reloadEnded?.Invoke();
		}
		
	}

}