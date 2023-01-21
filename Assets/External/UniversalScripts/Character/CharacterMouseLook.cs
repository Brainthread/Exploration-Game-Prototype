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
        Vector3 camRot = myCamera.transform.localRotation.eulerAngles;

        bodyRot.y = rotY;
        camRot.x = rotX;

        transform.localRotation = Quaternion.Euler(bodyRot);
        myCamera.transform.localRotation = Quaternion.Euler(camRot);
    }

    public Vector2 GetMouseMovement ()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        return new Vector2(mouseX, mouseY);
    }
}
