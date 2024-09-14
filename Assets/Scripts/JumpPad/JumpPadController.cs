using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadController : MonoBehaviour
{
    [SerializeField] private float JumpingForce;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto ou seu pai tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            // Obtém o Rigidbody do objeto pai (que deve estar no GameObject vazio "Player")
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Aplica a força de pulo no Rigidbody do objeto pai
                rb.AddForce(Vector3.up * JumpingForce, ForceMode.Impulse);
            }
        }
    }
}
