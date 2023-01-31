using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidEffect : MonoBehaviour
{
    [SerializeField] private float m_lifeTime = 2f;
    [SerializeField] private float m_force = 45f;
    [SerializeField] private float m_range = 10;

    private float m_timeSinceSpawn = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_timeSinceSpawn += Time.deltaTime;
        List<ForceReciever> forceRecievers = ForceRecieverRegister.Current.Get();
        for(int i = 0; i < forceRecievers.Count; i++)
        {
            ForceReciever current = forceRecievers[i];
            if (Vector3.Distance(current.transform.position, transform.position) < m_range)
            {
                Vector3 forceDirection = (transform.position - current.transform.position).normalized;
                Vector3 force = m_force * forceDirection * Time.deltaTime;
                current.ApplyForce(force, ForceMode.Force);
            }
        }
        if(m_timeSinceSpawn> m_lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
