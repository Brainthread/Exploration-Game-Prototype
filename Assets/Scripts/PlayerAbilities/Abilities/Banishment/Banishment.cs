using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banishment : MonoBehaviour, PlayerAbility
{
    public GameObject m_projectileSpawn;
    public GameObject m_projectile;

    public PlayerAbilityHandler Handler => throw new System.NotImplementedException();

    public void Activate()
    {
        GameObject tlsm = Instantiate(m_projectile, m_projectileSpawn.transform.position, m_projectileSpawn.transform.rotation);
        tlsm.GetComponent<Projectile>();
    }

    public void ActiveUpdate()
    {
       
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
      
    }

    public void Initialize(PlayerAbilityHandler handler)
    {
        
    }

    public void PassiveUpdate()
    {
        
    }

    public void TriggerStart()
    {
        throw new System.NotImplementedException();
    }

    public void TriggerStop()
    {
        throw new System.NotImplementedException();
    }

}
