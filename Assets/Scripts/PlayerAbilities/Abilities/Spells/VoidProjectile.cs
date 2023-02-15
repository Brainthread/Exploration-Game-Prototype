using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidProjectile : Projectile, IHitDetector, IHitResponder, IHurtDetector, IHurtResponder
{
    [SerializeField] private GameObject m_hurtEffect;

    public IHitResponder HitResponder { get { return this; } set { value = this; } }

    public IHurtResponder HurtResponder { get { return this; } set { value = this; } }

  

    public bool CheckHit(HitData data)
    {
        return true;
    }

    public void HitResponse(HitData hitData, DamageData damageData)
    {
        print("You hit something!! Go you!");
    }

    public override void OnHit(RaycastHit hit)
    {
        IHurtDetector hurtDetector = hit.transform.GetComponent<IHurtDetector>();
        if (hurtDetector != null)
        {
            HitHurtable(hurtDetector);
        }
        base.OnHit(hit);
    }

    public void HitHurtable(IHurtDetector hurtDetector)
    {
        HitData hitRegistration = new HitData();
        hitRegistration.Hurtdetector = hurtDetector;
        hitRegistration.HitDetector = this;
        if (hitRegistration.Validate())
        {
            hurtDetector.HurtResponder.HurtResponse(hitRegistration, m_damageData);
            this.HitResponse(hitRegistration, m_damageData);
        }
    }

    public void HurtResponse(HitData data, DamageData damagedata)
    {
        SummonHurtObject();
        Destroy(gameObject);
    }
    public void SummonHurtObject()
    {
        Instantiate(m_hurtEffect, transform.position, Quaternion.identity);
    }
}
