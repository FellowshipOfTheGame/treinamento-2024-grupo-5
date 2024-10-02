using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    [Header("General")] 
    [SerializeField] private float attackCooldown;
    [SerializeField] private float rotationSpeed;
    
    [Header("Melee attack")] 
    [SerializeField] private float meleeDamage;
    [SerializeField] private float meleeRange;
    
    [Header("Jump attack")] 
    [SerializeField] private float jumpDamage;
    [SerializeField] private float jumpRange;
    
    [Header("Fire Breath")] 
    [SerializeField] private float damage;
    [SerializeField] private float range;

    private Transform _playerTransform;
    private float _distanceToPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(AttackCorotine());
    }

    // Update is called once per frame
    void Update()
    {
        _distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
        
        RotateTowardsPlayer();
    }

    private void RotateTowardsPlayer()
    {
        // Calcular a direção para o jogador
        Vector3 directionToPlayer = _playerTransform.position - transform.position;
        directionToPlayer.y = 0; // Ignorar diferença de altura para rotação apenas no eixo Y

        // Calcular a rotação desejada
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Rotacionar suavemente em direção ao jogador
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    
    IEnumerator AttackCorotine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldown);
            Attack();
        }
    }

    void Attack()
    {
        if (_distanceToPlayer <= meleeRange)
        {
            MeleeAttack();
            return;
        }
        
        if (_distanceToPlayer <= jumpRange)
        {
            JumpAttack();
            return;
        }
        
        FireBreathAttack();
    }

    void MeleeAttack()
    {
        Debug.Log("Melee Attack");
    }
    
    void JumpAttack()
    {
        Debug.Log("Jump Attack");
    }

    void FireBreathAttack()
    {
        Debug.Log("Fire Breath");
    }
}
