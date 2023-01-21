using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RigidbodyCharacterEngine))]
public class ThirdPersonCharacterController : CharacterController
{
    RigidbodyCharacterEngine m_rigidbodyCharacterEngine;
    float m_horizontal;
    float m_vertical;
    bool m_jumpPressed;
    Camera m_camera;
    //Interface functions
    internal override void Initialize()
    {
        m_camera = Camera.main;
        m_rigidbodyCharacterEngine = GetComponent<RigidbodyCharacterEngine>();
    }

    internal override void ReadInput()
    {
        m_horizontal = Input.GetAxisRaw("Horizontal");
        m_vertical = Input.GetAxisRaw("Vertical");
        m_jumpPressed = Input.GetButtonDown("Jump");
    }

    internal override void SendInput()
    {
        m_rigidbodyCharacterEngine.GetMovementInput(Vector3.ProjectOnPlane(m_camera.transform.forward * m_vertical + m_camera.transform.right * m_horizontal, Vector3.up));
        if (m_jumpPressed)
            m_rigidbodyCharacterEngine.GetJumpInput(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
        SendInput();
    }
}