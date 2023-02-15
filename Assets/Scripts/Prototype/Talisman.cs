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

    public void HitResponse(HitData hitData, DamageData damageData)
    {

    }

    public override void OnHit(RaycastHit hit)
    {
        IHurtDetector hurtDetector = hit.transform.GetComponent<IHurtDetector>();
        if (hurtDetector != null)
        {
            HitHurtable(hurtDetector);
            if(hit.transform.GetComponent<Projectile>())
            {
                TimeEffectHandler.Current.SetTemporaryTimescale(0.2f, 0f);
                CameraEffectHandler.Current.CameraFOVEffect(0.2f, 59);
            }
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
            hurtDetector.HurtResponder.HurtResponse(hitRegistration, m_damageData);
            this.HitResponse(hitRegistration, m_damageData);
        }
    }
}
