using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DamageData
{
    private GameObject m_source;

    private GameObject m_inflictor;

    private int m_damage;
    public int Damage { get { return m_damage; } set { m_damage = value; } }
}


[System.Serializable]
public class HitData
{
    private IHurtDetector _hurtDetector;
    private IHitDetector _hitDetector;
    public IHurtDetector Hurtdetector { get { return _hurtDetector; } set { _hurtDetector = value; } }
    public IHitDetector HitDetector { get { return _hitDetector; } set { _hitDetector = value; } }
    public bool Validate ()
    {
        if(_hurtDetector != null && _hitDetector != null)
        {
            if(_hurtDetector.HurtResponder.CheckHit(this) && _hitDetector.HitResponder.CheckHit(this))
            { 
                return true; 
            }
        }

        return false;
    }
}


public interface IHitDetector
{
    public IHitResponder HitResponder { get; set; }
}

public interface IHitResponder
{
    public void Response(HitData hitdata, DamageData damagedata);
    public bool CheckHit(HitData hitdata);
}

public interface IHurtDetector
{
    public IHurtResponder HurtResponder { get; set; }
}

public interface IHurtResponder
{
    public void Response(HitData data, DamageData damagedata);
    public bool CheckHit(HitData data);
}
