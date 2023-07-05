using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropperProjectile : Projectile, IHitDetector, IHitResponder, IHurtDetector, IHurtResponder
{
    [SerializeField] private GameObject m_hurtEffect;
    [SerializeField] private float m_speedDecay = 2;

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

    public override void Update()
    {
        base.Update();
        if(m_rigidbody.velocity.magnitude<3)
        {
            m_rigidbody.useGravity = true;
        }
    }

    public void FixedUpdate()
    {
         Vector3 m_horiSpeed = m_rigidbody.velocity;
         m_horiSpeed *= 1 - (Time.fixedDeltaTime * 1f);
         m_rigidbody.velocity = m_horiSpeed;
    }

    public override void OnHit(RaycastHit hit)
    {
        IHurtDetector hurtDetector = hit.transform.GetComponent<IHurtDetector>();
        if (hurtDetector != null)
        {
            HitHurtable(hurtDetector, hit);
        }
        SummonEffectAtHit(hit, m_hitEffect);
        DestroyProjectile();
    }

    public void HitHurtable(IHurtDetector hurtDetector, RaycastHit hit)
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
    public void SummonEffectAtHit(RaycastHit hit, GameObject effect)
    {
        if (m_hitEffect != null)
            Instantiate(effect, hit.point, Quaternion.identity);
    }
    public void SummonHurtObject()
    {
        Instantiate(m_hurtEffect, transform.position, Quaternion.identity);
    }

    public override void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
