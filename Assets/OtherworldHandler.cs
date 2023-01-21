using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherworldHandler : MonoBehaviour
{
    [SerializeField] private int m_insideObjects = 0;
    private void Start()
    {
        m_insideObjects = 0;
    }

    public bool IsInsideObjects()
    {
        return m_insideObjects > 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null&&other.gameObject.layer == 7) { m_insideObjects += 1; }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null && other.gameObject.layer == 7) { m_insideObjects -= 1; }
    }
}
