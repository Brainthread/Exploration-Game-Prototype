using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talisman : Projectile, IHitDetector, IHitResponder
{
    public IHitResponder HitResponder { get { return this; } set { value = this; }}

    public bool CheckHit(HitData data)
    {
        return true;
    }

    public void Response(HitData hitData, DamageData damageData)
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

    public void HitHurtable (IHurtDetector hurtDetector)
    {
        HitData hitRegistration = new HitData();
        hitRegistration.Hurtdetector = hurtDetector;
        hitRegistration.HitDetector = this;
        if(hitRegistration.Validate())
        {
            hurtDetector.HurtResponder.Response(hitRegistration, m_damageData);
            this.Response(hitRegistration, m_damageData);
        }
    }
}
