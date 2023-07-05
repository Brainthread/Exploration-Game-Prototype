using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpatiensCaster : MonoBehaviour, PlayerAbility
{
    [SerializeField] protected Vector3 m_projectileSpawn;
    [SerializeField] protected GameObject m_projectile;
    protected PlayerAbilityHandler m_handler;
    public PlayerAbilityHandler Handler { get { return m_handler; } }
    protected GameObject m_projectileBuf;
    public void Initialize(PlayerAbilityHandler handler)
    {
        m_handler = handler;
    }
    public void TriggerStart()
    {
        Activate();
    }

    public void TriggerStop()
    {

    }

    public void Activate()
    {
        GameObject camera = m_handler.Camera;
        Transform transform = camera.transform;
        Vector3 spawnPosition = transform.position + transform.right * m_projectileSpawn.x +
                                                        transform.up * m_projectileSpawn.y +
                                                        transform.forward * m_projectileSpawn.z;
        m_projectileBuf = Instantiate(m_projectile, spawnPosition, m_handler.Camera.transform.rotation);
        m_projectileBuf.GetComponent<Projectile>().Initialize();
        m_projectileBuf.GetComponent<Projectile>().EffectSpawnRotation = m_handler.transform.rotation;
        Vector3 hostSpeed = m_handler.GetCurrentSpeed();
        m_projectileBuf.GetComponent<Projectile>().AddSpeed(hostSpeed);
    }

    public void Enter()
    {

    }

    public void Exit()
    {

    }

    public void ActiveUpdate()
    {

    }

    public void PassiveUpdate()
    {

    }
}