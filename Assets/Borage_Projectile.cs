using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Borage_Projectile : Projectile, IHitDetector, IHitResponder, IHurtDetector, IHurtResponder
{
    public IHitResponder HitResponder { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public IHurtResponder HurtResponder { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public bool CheckHit(HitData hitdata)
    {
        throw new System.NotImplementedException();
    }

    public void HitResponse(HitData hitdata, DamageData damagedata)
    {
        throw new System.NotImplementedException();
    }

    public void HurtResponse(HitData data, DamageData damagedata)
    {
        throw new System.NotImplementedException();
    }

    public override void CheckCollision()
    {
        Vector3 direction = transform.position - m_formerPosition;
        if(GetComponent<Collider>().isTrigger == false)
        {
            direction = Vector3.down;
        }
        float rayLength = direction.magnitude;
        (bool hasCollided, RaycastHit hit) = HasCollided(direction, rayLength);
        if (hasCollided)
        {
            OnHit(hit);
        }
    }

    public override void UpdateRotation()
    {
        
    }

    public override void OnHit(RaycastHit hit)
    {
        Vector3 normal = hit.normal;
        float dot = Vector3.Dot(normal, Vector3.up);
        if(dot > 0.9f)
        {
            Instantiate(m_hitEffect, hit.point, Quaternion.identity);
            DestroyProjectile();
        }
        else
        {
            GetComponent<Collider>().isTrigger = false;
        }
    }

    public override void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
