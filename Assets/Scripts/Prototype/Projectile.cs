using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected float m_speed = 5f;
    [SerializeField] protected GameObject m_hitEffect;
    [SerializeField] protected LayerMask m_hittableLayers;
    [SerializeField] protected float m_lifetime;
    [SerializeField] protected DamageData m_damageData;

    protected Rigidbody m_rigidbody;
    protected Vector3 m_formerPosition;
    protected float m_timeSinceCreation;

    public virtual void Initialize()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        //print(m_rigidbody);
        m_formerPosition = transform.position;
        m_rigidbody.velocity = transform.forward * m_speed;
        m_timeSinceCreation = 0;
    }

    public virtual void Start()
    {
        //Initialize();
    }

    public virtual void Update()
    {
        Quaternion lookRotation = Quaternion.LookRotation(m_rigidbody.velocity.normalized);
        transform.rotation = lookRotation;
        m_timeSinceCreation += Time.deltaTime;
        CheckCollision();
        m_formerPosition = transform.position;
        if(m_timeSinceCreation > m_lifetime)
        {
            DestroyProjectile();
        }
    }

    public virtual void CheckCollision()
    {
        Vector3 direction = transform.position - m_formerPosition;
        float rayLength = direction.magnitude;
        (bool hasCollided, RaycastHit hit) = HasCollided(direction, rayLength);
        if (hasCollided)
        {
            OnHit(hit);
        }
    }

    public virtual void AddSpeed(Vector3 speed)
    {
        m_rigidbody.AddForce(speed, ForceMode.VelocityChange);
    }

    public virtual (bool, RaycastHit) HasCollided(Vector3 direction, float rayLength)
    {
        RaycastHit hit;
        if (Physics.Raycast(m_formerPosition, direction.normalized, out hit, rayLength, m_hittableLayers))
            return (true, hit);
        return (false, hit);
    }

    public virtual void OnHit(RaycastHit hit)
    {
        DestroyProjectile();
    }

    public virtual void DestroyProjectile()
    {
        if (m_hitEffect != null)
            Instantiate(m_hitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
