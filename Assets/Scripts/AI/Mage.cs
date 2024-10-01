using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mage : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab; // Prefab da flecha
    [SerializeField] private float movementSpeed;
    [SerializeField] private Transform firePoint; // Ponto de origem da flecha
    [SerializeField] private float arrowSpeed = 10f; // Velocidade da flecha
    [SerializeField] private float magicBallTimer = 10f;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float losePlayerRange = 30f;
    [SerializeField] private float attackCooldown = 4f;
    
    private Transform _player; // Referência ao jogador
    private bool _isPlayerDetected;
    private bool _shoot;
    private bool _canShoot = true;
    private Coroutine _shootCoroutine;
    private NavMeshAgent _agent;
    
    void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = movementSpeed;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!_player) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        
        VerifyPlayerDistance(distanceToPlayer);

        if (!_isPlayerDetected) 
            return;
        
        if (HasLineOfSightToPlayer())
        {
            _agent.isStopped = true;
            Vector3 direction = _player.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);

            if (_canShoot && _shootCoroutine == null)
            {
                _shootCoroutine = StartCoroutine(Shoot());
            }
        }
        else
        {
            StopShooting();
            _agent.isStopped = false;
            _agent.destination = _player.position;
        }
    }

    void VerifyPlayerDistance(float distanceToPlayer)
    {
        if (distanceToPlayer <= detectionRange && HasLineOfSightToPlayer())
        {
            _isPlayerDetected = true;
        }
        else if (distanceToPlayer > losePlayerRange)
        {
            _isPlayerDetected = false;
        }
    }

    bool HasLineOfSightToPlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (_player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
        {
            if (hit.transform.CompareTag(arrowPrefab.transform.tag))
            {
                return true;
            }
            
            return hit.transform.CompareTag(_player.tag);
        }

        return false;
    }

    IEnumerator Shoot()
    {
        while (_isPlayerDetected && HasLineOfSightToPlayer())
        {
            ShootArrow();
            _canShoot = false;
            yield return new WaitForSeconds(attackCooldown);
            _canShoot = true;
        }

        _shootCoroutine = null;
    }
    
    void ShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        arrow.GetComponent<MagicBall>().Initialize(_player, arrowSpeed, magicBallTimer);
    }
    
    void StopShooting()
    {
        if (_shootCoroutine != null)
        {
            StopCoroutine(_shootCoroutine);
            _shootCoroutine = null;
        }
        _canShoot = true;
    }
    
    private Vector3 GetAimDirection()
    {
        Vector3 direction = (_player.position - firePoint.position);

        return direction.normalized;
    }
}
