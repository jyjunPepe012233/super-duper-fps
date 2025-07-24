using UnityEngine;

namespace SDFPS.Services.Character
{

    public class PlayerCharacterLocomotionHandler : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private Transform m_playerTransform;
        
        [Header("Input")]
        [SerializeField] private Transform m_directionReference;

        [Header("Move")]
        [SerializeField] private float m_moveSpeed = 3f;
        [SerializeField] private float m_sprintSpeed = 6f;

        [Header("Physics")]
        [SerializeField] private float m_groundRayLength = 0.2f;
        [SerializeField] private float m_groundRayOffset = 0.1f;
        [SerializeField] private LayerMask m_groundMask = 1;

        public void MoveToward(Vector2 playerInput, bool isSprinting)
        {
            Vector3 inputToLocalDirection = new Vector3(playerInput.x, 0, playerInput.y);
            Vector3 localToWorldDirection = m_directionReference.TransformDirection(inputToLocalDirection);
            
            localToWorldDirection.y = 0;
            localToWorldDirection.Normalize();
            
            m_rigidbody.velocity = 
                GetParalleledDirection(localToWorldDirection) * GetCurrentMoveSpeed(isSprinting);
        }

        public bool IsGrounded()
        {
            Vector3 origin = m_playerTransform.position + Vector3.up * m_groundRayOffset;
            return Physics.Raycast(origin, Vector3.down, m_groundRayLength, m_groundMask);
        }
        
        private Vector3 GetParalleledDirection(Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z);
        }

        private float GetCurrentMoveSpeed(bool isSprinting)
        {
            return isSprinting ? m_sprintSpeed : m_moveSpeed;
        }
    }

}