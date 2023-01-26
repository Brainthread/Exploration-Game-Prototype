using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected float m_speed = 5f;
    [SerializeField] protected GameObject m_hitEffect;
    [SerializeField] protected LayerMask m_hittableLayers;

    protected Vector3 m_formerPosition;

    public virtual void Initialize()
    {
        m_formerPosition = transform.position;
    }

    public virtual bool HasCollided()
    {
        Vector3 direction = transform.position - m_formerPosition;
        float rayLength = direction.magnitude;
        if (Physics.Raycast(m_formerPosition, direction.normalized, rayLength, m_hittableLayers))
            return true;
        return false;
    }

    public abstract void OnHit();

}
