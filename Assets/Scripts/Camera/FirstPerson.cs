using System;
using Mantega;
using Unity.VisualScripting;
using UnityEngine;

public class FirstPerson : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerHead;
    
    [Header("Options")] 
    [SerializeField] private float horizontalSense = 1;
    [SerializeField] private float verticalSense = 1;
    public int baseFOV = 100;
    // bool invert_y_axis

    private bool canMove = true;

    public float _zRotation = 0f;
    private float _xRotation = 0f;
    private float _yRotation = 0f;
    
    void Start()
    {
        if(firstPersonCamera == null)
            Generics.ReallyTryGetComponent(gameObject, out firstPersonCamera);
        SetFOV(baseFOV);

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (canMove)
        {
            RotateWithMouse();
        }
    }

    private void LateUpdate()
    {
        transform.position = playerHead.position;
    }

    private void RotateWithMouse()
    {
        float horizontalInput = Input.GetAxisRaw("Mouse X") * horizontalSense;
        float verticalInput = Input.GetAxisRaw("Mouse Y") * verticalSense;

        _xRotation += horizontalInput;
        _yRotation += verticalInput;

        _yRotation = Math.Clamp(_yRotation, -90f, 90f);

        transform.localEulerAngles = new Vector3(-_yRotation, _xRotation, _zRotation);

        playerBody.localEulerAngles = new Vector3(0, _xRotation, 0);
    }

    public void SetFOV(float fov) => firstPersonCamera.fieldOfView = fov;

    public float GetFOV() => firstPersonCamera.fieldOfView;

    public void SetZRotation(float zRotation) => _zRotation = zRotation;

    public void StopCameraMovement()
    {
        canMove = false; // Impede a câmera de se mover
    }

    // Método para permitir a movimentação da câmera novamente
    public void ResumeCameraMovement()
    {
        canMove = true; // Permite a câmera se mover
    }
}
