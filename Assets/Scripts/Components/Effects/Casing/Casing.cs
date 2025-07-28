using SDFPS.Components.Character;
using SDFPS.Utils.ComponentLocator;
using SDFPS.Utils.ObjectPooling;
using UnityEngine;

namespace SDFPS.Components.Effects.Casing
{

    [RequireComponent(typeof(Rigidbody))]
    public class Casing : PooleeBehaviour
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

        public override void OnFirstInstantiated()
        {
            Physics.IgnoreCollision(
                WorldComponentLocator.singleton.Get<PlayerCharacter>().GetComponent<Collider>(),
                m_collider
            );
        }

        public override void OnTakeFromPool()
        {
            gameObject.SetActive(true);

            m_elapsedTime = 0;
        }

        public override void OnReturnedToPool()
        {
            gameObject.SetActive(false);
        }

        private void SetRandomVelocity()
        {
            Vector3 ejectionForce = Vector3.zero;
            ejectionForce.x = Mathf.Lerp(m_minXForce, m_maxXForce, Random.value);
            ejectionForce.y = Mathf.Lerp(m_minYForce, m_maxYForce, Random.value);
            ejectionForce.z = Mathf.Lerp(m_minZForce, m_maxZForce, Random.value);
            m_rigidbody.velocity = transform.rotation * ejectionForce;
            
            float angularForce = Mathf.Lerp(m_minAngularForce, m_maxAngularForce, Random.value);
            Vector3 randomAngularDir = Random.onUnitSphere;
            m_rigidbody.angularVelocity = Vector3.zero;
            m_rigidbody.AddTorque(randomAngularDir * angularForce, ForceMode.Impulse);
        }

        public void EjectAt(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
            
            SetRandomVelocity();
        }

        private void Update()
        {
            m_elapsedTime += Time.deltaTime;
            if (m_elapsedTime >= m_lifeTime)
            {
                ReturnToPool();
            }
        }
    }

}