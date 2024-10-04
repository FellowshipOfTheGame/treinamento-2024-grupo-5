using System.Collections;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    [Header("General")] 
    [SerializeField] private float attackCooldown;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Rigidbody rb;

    [Header("Melee attack")] 
    [SerializeField] private Transform meleeAttackPoint;
    [SerializeField] private int meleeDamage;
    [SerializeField] private float meleeRange;
    [SerializeField] private float meleeForce;
    
    [Header("Jump attack")] 
    [SerializeField] private int jumpDamage;
    [SerializeField] private float jumpRange;
    [SerializeField] private float jumpDamageRange = 20f;
    [SerializeField] private float behindAngleThreshold = 200f;
    [SerializeField] private float jumpDuration = 10f;
    [SerializeField] private float jumpHeight = 15f;
    /*[SerializeField] private GameObject shockwave;
    [SerializeField] private Shockwave _shockwave;
    [SerializeField] private float shockwaveForce;
    [SerializeField] private float shockwaveMaxHeight;
    [SerializeField] private float shockwaveSpeed;*/
    
    [Header("Fire Breath")] 
    [SerializeField] private Transform breathingPoint;
    [SerializeField] private float fireBreathAngle = 30f;
    [SerializeField] private float fireBreathDuration = 3f;
    [SerializeField] private float range;
    [SerializeField] private int damage;
    [SerializeField] private ParticleSystem fireBreathEffect;

    private Transform _playerTransform;
    private float _distanceToPlayer;
    private HPController _playerHealth;
    private Vector3 _targetPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _playerHealth = _playerTransform.gameObject.GetComponent<HPController>();
        
        StartCoroutine(AttackCorotine());
    }

    // Update is called once per frame
    void Update()
    {
        _distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
        // shockwave.transform.position = new Vector3(gameObject.transform.position.x, shockwave.transform.position.y, gameObject.transform.position.z);
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
        Vector3 toPlayer = _playerTransform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, toPlayer);
        
        if (IsOnMeleeRange())
        {
            MeleeAttack();
            return;
        }
        
        if (_distanceToPlayer <= jumpRange && angle > behindAngleThreshold)
        {
            JumpAttack();
            return;
        }
        
        FireBreathAttack();
    }

    bool IsOnMeleeRange()
    {
        Vector3 toPlayer = _playerTransform.position - meleeAttackPoint.position;
        float angle = Vector3.Angle(meleeAttackPoint.forward, toPlayer);
        
        float distanceToPlayer = Vector3.Distance(meleeAttackPoint.position, _playerTransform.position);
        
        return distanceToPlayer <= meleeRange && angle < 160f;
    }
    
    void MeleeAttack()
    {
        Debug.Log("Melee Attack");

        _playerHealth.LoseHP(meleeDamage);
    }
    
    bool IsPlayerInRange()
    {
        Vector3 directionToTarget = (_playerTransform.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, _playerTransform.position);

        if (distanceToTarget <= meleeRange)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    
    void JumpAttack()
    {
        Debug.Log("Jump Attack");
        
        StartCoroutine(JumpAttackCoroutine());
    }
    
    IEnumerator JumpAttackCoroutine()
    {
        // Fase de salto
        yield return StartCoroutine(PerformJump());

        // Fase de queda e impacto
        yield return StartCoroutine(PerformLanding());
    }
    
    IEnumerator PerformJump()
    {
        Vector3 startPosition = transform.position;
        _targetPosition = _playerTransform.position;

        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            float t = elapsedTime / jumpDuration;
            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;

            transform.position = Vector3.Lerp(startPosition, _targetPosition, t) + Vector3.up * height;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator PerformLanding()
    {
        transform.position = _targetPosition;

        // Efeito de impacto
        rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);

        // Verifica se o jogador está na área de impacto
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, jumpDamageRange, playerLayer);
        foreach (var hitCollider in hitColliders)
        {
            _playerHealth.LoseHP(jumpDamage);
        }

        Debug.Log("Ataque de pulo concluído");
        yield return null;
    }

    void FireBreathAttack()
    {
        Debug.Log("Ataque de fogo");
        StartCoroutine(FireBreathCoroutine());
    }

    IEnumerator FireBreathCoroutine()
    {
        if (fireBreathEffect != null)
        {
            fireBreathEffect.Play();
        }

        float elapsedTime = 0f;
        while (elapsedTime < fireBreathDuration)
        {
            ApplyFireBreathDamage();
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Parar efeito de partículas
        if (fireBreathEffect != null)
        {
            fireBreathEffect.Stop();
        }
    }

    void ApplyFireBreathDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(breathingPoint.position, range);
        foreach (var hitCollider in hitColliders)
        {
            Vector3 directionToTarget = hitCollider.transform.position - breathingPoint.position;
            float angle = Vector3.Angle(breathingPoint.forward, directionToTarget);

            if (angle <= fireBreathAngle / 2 && directionToTarget.magnitude <= range)
            {
                // Verifique se o objeto atingido é o jogador
                if (hitCollider.CompareTag("PlayerBody"))
                {
                    Debug.Log("Jogador tomou dano");
                    _playerHealth.LoseHP(damage);
                }
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (breathingPoint == null) return;

        // Desenhar o cone de fogo
        Gizmos.color = Color.red;
        Vector3 forward = breathingPoint.forward;
        Vector3 up = breathingPoint.up;
        Vector3 right = breathingPoint.right;

        float halfAngle = fireBreathAngle * 0.5f * Mathf.Deg2Rad;
        float cosHalfAngle = Mathf.Cos(halfAngle);

        Vector3 baseCenter = breathingPoint.position + forward * range * cosHalfAngle;
        float radius = range * Mathf.Sin(halfAngle);

        // Desenhar o círculo da base do cone
        DrawCircle(baseCenter, forward, radius);

        // Desenhar as linhas do cone
        Gizmos.DrawLine(breathingPoint.position, baseCenter + up * radius);
        Gizmos.DrawLine(breathingPoint.position, baseCenter - up * radius);
        Gizmos.DrawLine(breathingPoint.position, baseCenter + right * radius);
        Gizmos.DrawLine(breathingPoint.position, baseCenter - right * radius);
    }

    void DrawCircle(Vector3 center, Vector3 normal, float radius)
    {
        int segments = 32;
        Vector3 forward = Vector3.Slerp(normal, -normal, 0.5f);
        Vector3 right = Vector3.Cross(normal, forward).normalized;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * 360f / segments * Mathf.Deg2Rad;
            float nextAngle = (i + 1) * 360f / segments * Mathf.Deg2Rad;
            Vector3 point1 = center + (Mathf.Sin(angle) * right + Mathf.Cos(angle) * forward) * radius;
            Vector3 point2 = center + (Mathf.Sin(nextAngle) * right + Mathf.Cos(nextAngle) * forward) * radius;
            Gizmos.DrawLine(point1, point2);
        }
    }
}
