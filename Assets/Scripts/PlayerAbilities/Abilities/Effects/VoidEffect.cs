using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VoidEffect : MonoBehaviour
{
    public static VoidEffect Current;
    [SerializeField] private float m_lifeTime = 2f;
    [SerializeField] private float m_maxForce = 450f;
    [SerializeField] private float m_targetSpeed = 25f;
    [SerializeField] private float m_range = 10;

    private float m_timeSinceSpawn = 0;
    private List<ForceReciever> m_registeredForceRecievers = new List<ForceReciever>(10);
    private List<Vector3> m_startPosition = new List<Vector3>(10);

    // Start is called before the first frame update
    void Start()
    {
        Current = this;
        print(this);
    }

    // Update is called once per frame
    void Update()
    {
        m_timeSinceSpawn += Time.deltaTime;
        if(m_timeSinceSpawn > m_lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        ForceReciever current = null;
        List<ForceReciever> forceRecievers = ForceRecieverRegister.Current.Get();
        //ApplyForceToRegisteredRecievers();
        for (int i = 0; i < forceRecievers.Count; i++)
        {
            current = forceRecievers[i];
            if (Vector3.Distance(current.transform.position, transform.position) < m_range)
            {
                ApplySpeedForce(current);
                //CheckRecognition(current);
            }
        }
    }

    void CheckRecognition(ForceReciever current)
    {
        for (int j = 0; j < m_registeredForceRecievers.Count; j++)
        {
            if (m_registeredForceRecievers[j] == current)
            {
                return;
            }
        }
        m_registeredForceRecievers.Add(current);
        m_startPosition.Add(current.transform.position);
    }

    void ApplySpeedForce(ForceReciever current)
    {
        Vector3 currentVelocity = current.Rigidbody.velocity;
        Vector3 offset = transform.position - current.Rigidbody.position;
        Vector3 targetVelocity = offset.normalized * m_targetSpeed;
        Vector3 velocityRelTarget = Vector3.Project(currentVelocity, targetVelocity);
        targetVelocity = targetVelocity - velocityRelTarget;
        Vector3 neededAcceleration = targetVelocity / Time.fixedDeltaTime;
        Vector3 force = Mathf.Clamp(neededAcceleration.magnitude, 0, m_maxForce)*neededAcceleration.normalized;

        current.ApplyForce(force*Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    void ApplyForce(ForceReciever current)
    {
        Vector3 currentVelocity = current.Rigidbody.velocity;
        Vector3 offset = transform.position - current.Rigidbody.position;

    }

    void ApplyForceToRegisteredRecievers()
    {
        ForceReciever current = null;
        for (int j = 0; j < m_registeredForceRecievers.Count; j++)
        {
            current = m_registeredForceRecievers[j];
            if (current == null || Vector3.Distance(current.transform.position, this.transform.position) > m_range)
            {
                m_registeredForceRecievers.RemoveAt(j);
                m_startPosition.RemoveAt(j);
                continue;
            }
            Vector3 forceDirection = (transform.position - current.transform.position).normalized;
            Vector3 startDirection = (transform.position - m_startPosition[j]).normalized;
            Vector3 force = m_maxForce * forceDirection * Time.deltaTime * Mathf.Clamp(Vector3.Dot(forceDirection, startDirection), 0.1f, 1);
            current.ApplyForce(force, ForceMode.Force);
        }
    }

    private void OnDestroy()
    {
        Current = null;
    }
}