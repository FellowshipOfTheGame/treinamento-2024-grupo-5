using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float gravity;
    private Vector3 velocity;

    void Start()
    {
        velocity = GetComponent<Rigidbody>().velocity;
    }

    void Update()
    {
        // Aplicar gravidade à flecha
        velocity.y += gravity * Time.deltaTime;
        GetComponent<Rigidbody>().velocity = velocity;

        // Alinhar a flecha na direção do movimento
        transform.rotation = Quaternion.LookRotation(velocity);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log(other.gameObject.tag);
        GetComponent<Rigidbody>().isKinematic = true;
        
        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Acertou o player");
        }
        
        Destroy(gameObject);
    }
}
