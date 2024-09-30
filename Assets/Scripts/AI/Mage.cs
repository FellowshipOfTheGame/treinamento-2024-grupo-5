using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab; // Prefab da flecha
    [SerializeField] private Transform firePoint; // Ponto de origem da flecha
    [SerializeField] private float shootInterval = 2f; // Intervalo entre disparos
    [SerializeField] private float arrowSpeed = 10f; // Velocidade da flecha
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float losePlayerRange = 30f;
    [SerializeField] private float attackCooldown = 4f;
    
    private Transform player; // Referência ao jogador
    private bool isPlayerDetected;
    private bool shoot;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        VerifyPlayerDistance(distanceToPlayer);

        if (!isPlayerDetected) 
            return;
        
        if (HasLineOfSightToPlayer())
        {
            // O arqueiro vira para o jogador
            Vector3 direction = player.position - transform.position;
            direction.y = 0; // Evita que o arqueiro olhe para cima/baixo
            transform.rotation = Quaternion.LookRotation(direction);

            if (shoot == false)
            {
                shoot = true;
                StartCoroutine(Shoot());
            }
        }
        else
        {
            // Se mover em direção ao Player

            shoot = false;
        }
    }

    void VerifyPlayerDistance(float distanceToPlayer)
    {
        if (distanceToPlayer <= detectionRange)
        {
            isPlayerDetected = true;
        }
        else if (distanceToPlayer > losePlayerRange)
        {
            isPlayerDetected = false;
        }
    }

    bool HasLineOfSightToPlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
        {
            return hit.transform == player;
        }

        return false;
    }

    IEnumerator Shoot()
    {
        while (shoot)
        {
            Debug.Log("Atirar");
            ShootArrow();
            yield return new WaitForSeconds(attackCooldown);
        }

        yield return null;
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
