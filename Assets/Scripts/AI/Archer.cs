using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To Do
// Se o arqueiro não conseguir ver o player ele deve se mover até conseguir ver ele
// Se o player chegar muito perto ele se afasta
// Melhorar comportamento das flechas

public class Archer : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab; // Prefab da flecha
    [SerializeField] private Transform firePoint; // Ponto de origem da flecha
    [SerializeField] private float shootInterval = 2f; // Intervalo entre disparos
    [SerializeField] private float arrowSpeed = 10f; // Velocidade da flecha
    
    private Transform player; // Referência ao jogador
    private float timeSinceLastShot = 0f;

    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            // O arqueiro vira para o jogador
            Vector3 direction = player.position - transform.position;
            direction.y = 0; // Evita que o arqueiro olhe para cima/baixo
            transform.rotation = Quaternion.LookRotation(direction);

            // Dispara a flecha a cada X segundos
            timeSinceLastShot += Time.deltaTime;
            if (timeSinceLastShot >= shootInterval)
            {
                ShootArrow();
                timeSinceLastShot = 0f;
            }
        }
    }
    
    void ShootArrow()
    {
        Vector3 direction = GetAimDirection();
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        arrow.GetComponent<Rigidbody>().velocity = direction * arrowSpeed;
    }
    
    private Vector3 GetAimDirection()
    {
        Vector3 direction = (player.position - firePoint.position);
        
        // Calcular a diferença de altura
        float heightDifference = player.position.y - firePoint.position.y;

        // Ajustar o ângulo de inclinação para compensar a gravidade
        // direction.y += heightDifference * 0.5f;  // Multiplicador para ajuste fino

        return direction.normalized;
    }
}
