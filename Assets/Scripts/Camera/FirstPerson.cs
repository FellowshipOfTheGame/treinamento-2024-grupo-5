using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPerson : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerHead;
    
    [Header("Options")] 
    [SerializeField] private float horizontalSense = 1;
    [SerializeField] private float verticalSense = 1;
    [SerializeField] private int fov = 100;
    // bool invert_y_axis
    
    private float _xRotation = 0f;
    private float _yRotation = 0f;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
    }
    
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Mouse X") * horizontalSense;
        float verticalInput = Input.GetAxisRaw("Mouse Y") * verticalSense;

        _xRotation += horizontalInput;
        _yRotation += verticalInput;

        transform.localEulerAngles = new Vector3(-_yRotation, _xRotation, 0);

        playerBody.localEulerAngles = new Vector3(0, _xRotation, 0);
    }

    private void LateUpdate()
    {
        transform.position = playerHead.position;
    }
}
