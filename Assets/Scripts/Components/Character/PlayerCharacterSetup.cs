using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace SDFPS.Components.Character
{

	public class PlayerCharacterSetup : NetworkBehaviour
	{ 
		[SerializeField] private GameObject m_localCharacter; 
		[SerializeField] private GameObject m_remoteCharacter;

		private PlayerCharacter m_localPlayerController;
		
		/// <summary>
		/// Local용 플레이어 캐릭터의 PlayerCharacter 컴포넌트
		/// </summary>
		public PlayerCharacter localPlayerController
		{
			get
			{
				if (m_localPlayerController == null)
				{
					m_localPlayerController = m_localCharacter.GetComponent<PlayerCharacter>();
				}

				return m_localPlayerController;
			}
		}
		
		private PlayerCharacterRemote m_remotePlayerController;
		
		/// <summary>
		/// 외부 클라이언트용 플레이어 캐릭터의 PlayerCharacterRemote 컴포넌트
		/// </summary>
		public PlayerCharacterRemote remotePlayerController
		{
			get
			{
				if (m_remotePlayerController == null)
				{
					m_remotePlayerController = m_remoteCharacter.GetComponent<PlayerCharacterRemote>();
				}

				return m_remotePlayerController;
			}
		}
		
		public override void OnStartClient()
		{
			if (m_localCharacter == null)
			{
				Debug.LogError("Player Prefab이 설정되지 않음");
				return;
			}

			if (m_remoteCharacter == null)
			{
				Debug.LogError("Remote Character Prefab이 설정되지 않음");
				return;
			}

			Debug.Log($"Setup 완료: netId-{netId} isLocalPlayer-{isLocalPlayer}");

			GameObject character = isLocalPlayer ? m_localCharacter : m_remoteCharacter;
			
			m_localCharacter.SetActive(false);
			m_remoteCharacter.SetActive(false);
			
			character.SetActive(true);
		}
	}

}