using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Vector3 _forward;
    private Vector3 _strafe;
    private CharacterController _characterController;
    
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxisRaw("Vertical");
        float strafeInput = Input.GetAxisRaw("Horizontal");

        _forward = forwardInput * speed * transform.forward;
        _strafe = strafeInput * speed * transform.right;

        Vector3 finalVelocity = _forward + _strafe;
        finalVelocity.y += Physics.gravity.y;
        
        _characterController.Move(finalVelocity * Time.deltaTime);
    }
}
