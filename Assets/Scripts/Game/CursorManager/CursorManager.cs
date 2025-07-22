using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{

	public class CursorManager : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI m_textOutput;
		[SerializeField] private InputActionProperty m_chModInput;

		private bool m_isLocked;
		
		public void Awake()
		{
			m_chModInput.action.performed += _ => ChangeMode();
			m_chModInput.action.Enable();
			
			Setting(m_isLocked);
		}

		private void ChangeMode()
		{
			Debug.Log("Cursor LockState Changed");
			
			m_isLocked = !m_isLocked;
			Setting(m_isLocked);
		}

		private void Setting(bool isLocked)
		{
			Cursor.visible = !isLocked;
			Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;

			if (m_textOutput != null)
			{
				m_textOutput.text = m_isLocked ? "Cursor Locked" : "Cursor Unlocked";
			}
		}
	}

}