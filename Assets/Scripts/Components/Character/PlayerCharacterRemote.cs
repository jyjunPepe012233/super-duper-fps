using Mirror;
using UnityEngine;

namespace SDFPS.Components.Character
{

	public class PlayerCharacterRemote : NetworkBehaviour
	{
		public void SetTransform(Vector3 position, Quaternion rotation)
		{
			transform.SetPositionAndRotation(position, rotation);
		}
	}

}