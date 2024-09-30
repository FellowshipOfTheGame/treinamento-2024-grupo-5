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
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float losePlayerRange = 30f;
    [SerializeField] private float attackCooldown = 4f;
    
    private Transform player; // Referência ao jogador
    private bool isPlayerDetected;
    private bool shoot;
    private bool canShoot = true;
    private Coroutine shootCoroutine;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!player) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        VerifyPlayerDistance(distanceToPlayer);

        if (!isPlayerDetected) 
            return;
        
        Debug.Log("Has Line = " + HasLineOfSightToPlayer());
        if (HasLineOfSightToPlayer())
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);

            if (canShoot && shootCoroutine == null)
            {
                shootCoroutine = StartCoroutine(Shoot());
            }
        }
        else
        {
            StopShooting();
            // Implemente aqui a lógica para se mover em direção ao Player
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
            if (hit.transform.CompareTag(arrowPrefab.transform.tag))
            {
                return true;
            }
            
            return hit.transform.CompareTag(player.tag);
        }

        return false;
    }

    IEnumerator Shoot()
    {
        while (isPlayerDetected && HasLineOfSightToPlayer())
        {
            ShootArrow();
            canShoot = false;
            yield return new WaitForSeconds(attackCooldown);
            canShoot = true;
        }

        shootCoroutine = null;
    }
    
    void ShootArrow()
    {
        Vector3 direction = GetAimDirection();
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        arrow.GetComponent<Rigidbody>().velocity = direction * arrowSpeed;
    }
    
    void StopShooting()
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
        canShoot = true;
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
