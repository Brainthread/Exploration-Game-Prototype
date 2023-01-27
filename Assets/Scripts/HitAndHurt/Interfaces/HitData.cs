using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPackage
{
    private int m_damage;
    public int Damage { get { return m_damage; } set { m_damage = value; } }

}

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
    public void Response(HitData data);
    public bool CheckHit(HitData data);
}

public interface IHurtDetector
{
    public IHitResponder HurtResponder { get; set; }
}

public interface IHurtResponder
{
    public void Response(HitData data);
    public bool CheckHit(HitData data);
}
