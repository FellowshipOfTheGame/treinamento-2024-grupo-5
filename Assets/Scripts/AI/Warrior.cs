using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Warrior : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackAngle = 60f;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float firstAttackCooldown = 0.5f;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float losePlayerRange = 30f;
    
    private Transform _player; // Referência ao jogador
    private NavMeshAgent agent;
    private bool _isPlayerDetected;
    private bool _canAttack = true;
    private Coroutine _attackCoroutine;
    private float _distanceToPlayer;
    private HPController _playerHealth;
    private LayerMask _playerLayer;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        _playerHealth = _player.gameObject.GetComponent<HPController>();
        Debug.Log(_playerHealth);
        _playerLayer = _player.gameObject.layer;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_player) return;
        
        _distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        
        VerifyPlayerDistance(_distanceToPlayer);
        
        if (_isPlayerDetected)
        {
            Vector3 direction = _player.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
            agent.isStopped = false;
            agent.SetDestination(_player.position);

            if (_distanceToPlayer <= attackRange && _canAttack)
            {
                _attackCoroutine = StartCoroutine(Attack());
            }
        }
        else
        {
            agent.isStopped = true;
        }

        if (_distanceToPlayer <= attackRange)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
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
            return hit.transform.CompareTag(_player.tag);
        }
        
        return false;
    }

    IEnumerator Attack()
    {
        _canAttack = false;
        agent.isStopped = true;
        yield return new WaitForSeconds(firstAttackCooldown);
        
        while (_isPlayerDetected && HasLineOfSightToPlayer() && _distanceToPlayer <= attackRange)
        {
            _Attack();
            _canAttack = false;
            yield return new WaitForSeconds(attackCooldown);
            _canAttack = true;
        }

        _attackCoroutine = null;
    }

    
    void _Attack()
    {
        Debug.Log("Ataque");
        
        if (IsPlayerInRange())
        {
            Debug.Log("Morra player");
            _playerHealth.LoseHP(damage);
        }
    }

    bool IsPlayerInRange()
    {
        Vector3 directionToTarget = (_player.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, _player.position);
        float angle = Vector3.Angle(transform.forward, directionToTarget);

        if (distanceToTarget <= detectionRange && angle <= attackAngle / 2)
        {
            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _playerLayer))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }
}
