using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private GameObject m_target;

    [SerializeField] private float m_targetDistance = 10f;
    [SerializeField] private float m_minDistance = 3f;
    [SerializeField] private float m_maxDistance = 10f;

    [SerializeField] private float m_followSpeed = 10f;
    [SerializeField] private float m_dampener = 2f;

    [SerializeField] private float m_minElevation;
    [SerializeField] private float m_maxElevation;

    private float m_yaw;
    private float m_pitch;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ClampValues();
        UpdateCamera();
    }

    private void ClampValues()
    {
        m_pitch = Mathf.Clamp(m_pitch, m_minElevation, m_maxElevation);
    }

    private void UpdateCamera()
    {
        if (m_target != null)
        {
            Vector3 targetPosition = m_target.transform.position - transform.forward*m_targetDistance;
            
            transform.position = targetPosition;

        }
    }
}
