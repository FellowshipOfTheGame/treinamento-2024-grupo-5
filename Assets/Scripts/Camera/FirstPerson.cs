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
    //[SerializeField] private int fov = 100;
    // bool invert_y_axis

    public float _zRotation = 0f;
    private float _xRotation = 0f;
    private float _yRotation = 0f;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    public float t = 0;
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Mouse X") * horizontalSense;
        float verticalInput = Input.GetAxisRaw("Mouse Y") * verticalSense;

        _xRotation += horizontalInput;
        _yRotation += verticalInput;

        _yRotation = Math.Clamp(_yRotation, -90f, 90f);
        
        transform.localEulerAngles = new Vector3(-_yRotation, _xRotation, _zRotation);

        playerBody.localEulerAngles = new Vector3(0, _xRotation, 0);

        SetZRotation(t);
    }

    private void LateUpdate()
    {
        transform.position = playerHead.position;
    }


    public void SetZRotation(float zRotation) => _zRotation = zRotation;
}
