using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mantega;

public class JumpPadController : MonoBehaviour
{
    [SerializeField] private float JumpingForce;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto ou seu pai tem a tag "Player"
        // Obtém o Rigidbody do objeto pai (que deve estar no GameObject vazio "Player")
        if (Generics.ReallyTryGetComponent(other.gameObject ,out PlayerMovement pm))
        {
            Rigidbody rb = pm.GetComponent<Rigidbody>();
            // Aplica a força de pulo no Rigidbody do objeto pai
            rb.AddForce(Vector3.up * JumpingForce, ForceMode.Impulse);
        }
        
    }
}
