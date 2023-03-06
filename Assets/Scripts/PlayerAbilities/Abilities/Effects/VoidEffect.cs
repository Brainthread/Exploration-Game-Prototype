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
    [SerializeField] private int m_directions = 1;
    [SerializeField] private float m_verticalMaxSpeed = 20;

    private float m_timeSinceSpawn = 0;
    private List<ForceReciever> m_registeredForceRecievers = new List<ForceReciever>(10);
    private List<Vector3> m_startPosition = new List<Vector3>(10);
    [SerializeField] private bool m_instant = false;

    // Start is called before the first frame update
    void Start()
    {
        Current = this;
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
        for (int i = 0; i < forceRecievers.Count; i++)
        {
            current = forceRecievers[i];
            if (Vector3.Distance(current.transform.position, transform.position) < m_range)
            {
                if(m_instant)
                {
                    if(m_registeredForceRecievers.Contains(current) == false) {
                        m_registeredForceRecievers.Add(current);
                        ApplySpeedForce(current);
                    }
                }
                else
                    ApplySpeedForce(current);
            }
        }
    }


    void ApplySpeedForce(ForceReciever current)
    {
        Vector3 currentVelocity = current.Rigidbody.velocity;
        Vector3 offset = transform.position - current.Rigidbody.position;

        
        float yComponent = Vector3.Dot(transform.up, offset.normalized);
        float yZoneSize = (180 / m_directions);
        float yAngle = Mathf.Acos(yComponent) * Mathf.Rad2Deg - yZoneSize/2;
        float yRound = Mathf.Round((yAngle) / yZoneSize);

        yAngle = yRound * yZoneSize + yZoneSize*0.5f;
        yComponent = Mathf.Cos(yAngle * Mathf.Deg2Rad);
        offset.y = 0;
        offset = offset.normalized;
        offset.y = Mathf.Sign(yComponent);

        Debug.DrawRay(current.transform.position, transform.up * offset.y*100, Color.yellow);

        Vector3 targetVelocity = offset * m_targetSpeed;

        Debug.DrawRay(current.transform.position, targetVelocity * 100, Color.red);

        targetVelocity.y = Mathf.Clamp(targetVelocity.y, -m_verticalMaxSpeed, m_verticalMaxSpeed);
        
        Vector3 velocityRelTarget = Vector3.Project(currentVelocity, targetVelocity);
        targetVelocity = targetVelocity - velocityRelTarget;
        Vector3 neededAcceleration = targetVelocity / Time.fixedDeltaTime;
        Vector3 force = Mathf.Clamp(neededAcceleration.magnitude, 0, m_maxForce)*neededAcceleration.normalized;

        current.Rigidbody.velocity = targetVelocity/current.Rigidbody.mass;

    }




    private void OnDestroy()
    {
        Current = null;
    }
}
