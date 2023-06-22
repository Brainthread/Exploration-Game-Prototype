using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 10.0f;
    [SerializeField] private float clampAngle = 80.0f;
    [SerializeField] private Camera myCamera = null;
    //[SerializeField] private bool invertY = false;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis
    private float rotZ = 0.0f; // rotation around the forward/z axis
    private float targetRotZ = 0;
    private float deltaZ = 2;

    void Start()
    {
        rotY = transform.localRotation.eulerAngles.y;
        rotX = myCamera.transform.localRotation.eulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity;
        rotX += mouseY * mouseSensitivity;
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);


        Vector3 bodyRot = transform.localRotation.eulerAngles;
        Vector3 camRot = Vector3.zero;

        bodyRot.y = rotY;
        camRot.x = rotX;

        rotZ = Mathf.MoveTowards(rotZ, targetRotZ, deltaZ*Time.deltaTime);
        transform.localRotation = Quaternion.Euler(bodyRot);
        myCamera.transform.localRotation = Quaternion.identity * Quaternion.Euler(camRot) * Quaternion.Euler(new Vector3(0, 0, rotZ));
    }

    public Vector2 GetMouseMovement ()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        return new Vector2(mouseX, mouseY);
    }


    public void DoTilt(float degrees, float delta)
    {
        deltaZ = delta;
        targetRotZ = degrees;
    }
}
