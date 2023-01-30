using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalismanWeapon : MonoBehaviour, PlayerAbility
{
    [SerializeField] private Vector3 m_projectileSpawnPos;
    [SerializeField] private GameObject m_projectile;

    public PlayerAbilityHandler Handler => throw new System.NotImplementedException();

    public void Initialize(PlayerAbilityHandler handler)
    {
        throw new System.NotImplementedException();
    }

    public void Activate()
    {
        throw new System.NotImplementedException();
    }

    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void ActiveUpdate()
    {
        throw new System.NotImplementedException();
    }


    public void PassiveUpdate()
    {
        throw new System.NotImplementedException();
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
