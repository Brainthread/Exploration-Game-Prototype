using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapleEffect : MonoBehaviour
{
    [SerializeField] private float m_lifeTime = 2f;
    [SerializeField] private float m_force = 450f;
    [SerializeField] private float m_targetSpeed = 25f;
    [SerializeField] private float m_range = 10;
    [SerializeField] private int m_directions = 1;
    [SerializeField] private float m_verticalMaxSpeed = 20;

    private float m_timeSinceSpawn = 0;
    private List<ForceReciever> m_registeredForceRecievers = new List<ForceReciever>(10);
    private List<Vector3> m_startPosition = new List<Vector3>(10);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        m_timeSinceSpawn += Time.deltaTime;
        if (m_timeSinceSpawn > m_lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        ForceReciever current = null;
        List<ForceReciever> forceRecievers = ForceRecieverRegister.Current.Get();
        for (int i = 0; i < forceRecievers.Count; i++)
        {
            current = forceRecievers[i];
            if (Vector3.Distance(current.transform.position, transform.position) < m_range)
            {
                if (m_registeredForceRecievers.Contains(current) == false)
                {
                    m_registeredForceRecievers.Add(current);
                    ApplyForce(current);
                }
            }
        }
    }


    void ApplyForce(ForceReciever current)
    {
        Vector3 currentVelocity = current.Rigidbody.velocity;
        Vector3 offset = transform.position - current.Rigidbody.position;
        if (current.gameObject.layer == 3)
            currentVelocity.y = 0;
        current.Rigidbody.velocity = currentVelocity;
        current.ApplyForce(transform.up * m_force, ForceMode.Force);

    }

}

