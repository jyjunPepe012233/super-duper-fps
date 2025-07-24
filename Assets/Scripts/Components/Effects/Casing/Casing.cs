using SDFPS.Components.Character;
using SDFPS.Utils.ComponentLocator;
using UnityEngine;

namespace SDFPS.Components.Effects.Casing
{

    [RequireComponent(typeof(Rigidbody))]
    public class Casing : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private Collider m_collider;
        
        [Header("Ejection Forces")]
        [SerializeField] private float m_minXForce = 1f;
        [SerializeField] private float m_maxXForce = 2f;
        [Space]
        [SerializeField] private float m_minYForce = 2f;
        [SerializeField] private float m_maxYForce = 3f;
        [Space]
        [SerializeField] private float m_minZForce = -2;
        [SerializeField] private float m_maxZForce = 0f;
        [Space]
        [SerializeField] private float m_minAngularForce = 1f;
        [SerializeField] private float m_maxAngularForce = 3f;

        [Header("Lifetime")]
        [SerializeField] private float m_lifeTime = 1f;

        private float m_elapsedTime;
        
        private void Start()
        {
            Physics.IgnoreCollision(
                WorldComponentLocator.singleton.Get<PlayerCharacter>().GetComponent<Collider>(),
                m_collider
            );
            
            Vector3 ejectionForce = Vector3.zero;
            ejectionForce.x = Mathf.Lerp(m_minXForce, m_maxXForce, Random.value);
            ejectionForce.y = Mathf.Lerp(m_minYForce, m_maxYForce, Random.value);
            ejectionForce.z = Mathf.Lerp(m_minZForce, m_maxZForce, Random.value);
            m_rigidbody?.AddForce(transform.rotation * ejectionForce, ForceMode.Impulse);
            
            float angularForce = Mathf.Lerp(m_minAngularForce, m_maxAngularForce, Random.value);
            Vector3 randomAngularDir = Random.onUnitSphere;
            m_rigidbody?.AddTorque(randomAngularDir * angularForce, ForceMode.Impulse);
        }

        private void Update()
        {
            m_elapsedTime += Time.deltaTime;
            if (m_elapsedTime >= m_lifeTime)
            {
                Destroy(gameObject);
            }
        }
    }

}