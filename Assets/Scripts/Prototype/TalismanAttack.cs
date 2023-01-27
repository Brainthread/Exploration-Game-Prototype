using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalismanAttack : MonoBehaviour
{
    public GameObject m_talismanSpawn;
    public GameObject m_talisman;

    public void Attack()
    {
        GameObject tlsm = Instantiate(m_talisman, m_talismanSpawn.transform.position, m_talismanSpawn.transform.rotation);
        tlsm.GetComponent<Projectile>();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }
}
