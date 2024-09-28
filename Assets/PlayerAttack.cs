using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mantega;

public class PlayerAttack : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerInput playerInput;

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1f;

    void Start()
    {
        if (playerInput == null)
            Generics.ReallyTryGetComponent(gameObject, out playerInput);
        playerInput.attack += Attack;
    }

    void Attack()
    {
        Debug.Log("Attack");
        var collisions = Physics.OverlapSphere(attackPoint.position, attackRange);
        foreach (var collision in collisions)
        {
            if (Generics.FamilyTryGetComponent(collision.gameObject, out HPController hpController) && hpController.gameObject.tag == "Enemy")
                hpController.LoseHP(1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void OnDestroy()
    {
        playerInput.attack -= Attack;
    }
}
