using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class PlayerAbility_ProjectileAbility : MonoBehaviour, PlayerAbility
{
    [SerializeField] protected Vector3 m_projectileSpawn;
    [SerializeField] protected GameObject m_projectile;
    protected PlayerAbilityHandler m_handler;
    public PlayerAbilityHandler Handler { get { return m_handler; }}

    public virtual void Initialize(PlayerAbilityHandler handler) { 
        m_handler = handler;
        print(handler);
        print(m_handler);
    }

    public virtual void Enter() {
    
    }

    public virtual void Exit() { 
        
    }

    public virtual void Activate() {
        GameObject camera = m_handler.Camera;
        Transform transform = camera.transform;
        Vector3 spawnPosition = transform.position + transform.right * m_projectileSpawn.x +
                                                        transform.up * m_projectileSpawn.y +
                                                        transform.forward * m_projectileSpawn.z;
        GameObject projectile = Instantiate(m_projectile, spawnPosition, m_handler.Camera.transform.rotation);
        projectile.GetComponent<Projectile>().Initialize();
        Vector3 hostSpeed = m_handler.GetCurrentSpeed();
        projectile.GetComponent<Projectile>().AddSpeed(hostSpeed);
    }

    public virtual void ActiveUpdate() {
        //Give me some activation button and call Activate()
    }

    public virtual void PassiveUpdate() { 
    
    }

    public virtual void TriggerStart()
    {
        
    }

    public virtual void TriggerStop()
    {
        
    }

   
}
