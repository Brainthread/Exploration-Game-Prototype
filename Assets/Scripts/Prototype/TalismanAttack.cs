using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalismanAttack : MonoBehaviour
{
    [SerializeField] protected GameObject m_talismanSpawn;
    [SerializeField] protected GameObject m_talisman;

    public void Attack()
    {
        GameObject tlsm = Instantiate(m_talisman, m_talismanSpawn.transform.position, m_talismanSpawn.transform.rotation);
        tlsm.GetComponent<Projectile>().Initialize();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }
}
