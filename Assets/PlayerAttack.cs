using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mantega;
using System;

public class PlayerAttack : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerInput playerInput;

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private bool _canAttack = true;
    public Action<float> attack;

    [Header("Weapon")]
    public ScriptableItem weapon;
    [SerializeField] private GameObject _weaponGameObject;
    [SerializeField] private Item _weaponItem;

    void Start()
    {
        if (playerInput == null)
            Generics.ReallyTryGetComponent(gameObject, out playerInput);
        playerInput.attack += Attack;

        if (weapon != null)
            ActiveWeapon(weapon);
    }

    public void ResetAttack()
    {
        DesactiveWeapon();
        _canAttack = true;
    }

    public void DesactiveWeapon()
    {
        if(_weaponGameObject != null)
            Destroy(_weaponGameObject);
        weapon = null;
        _weaponItem = null;
    }

    public void ActiveWeapon(ScriptableItem newWeapon)
    {
        DesactiveWeapon();
        weapon = newWeapon;
        if(weapon.prefab != null)
        {
            _weaponGameObject = Instantiate(weapon.prefab, transform);
            Generics.FamilyTryGetComponent(_weaponGameObject, out _weaponItem);
        }
    }

    void Attack()
    {
        if(!_canAttack || _weaponItem == null)
            return;

        _canAttack = false;
        Invoke(nameof(CooldownAttack), _weaponItem.attackCooldown);
        attack?.Invoke(_weaponItem.attackCooldown);

        GetComponent<PlayerSoundEffects>().PlayAtackSound();

        var collisions = Physics.OverlapSphere(attackPoint.position, _weaponItem.range);
        foreach (var collision in collisions)
        {
            if (Generics.FamilyTryGetComponent(collision.gameObject, out EnemyHPController hpController) && hpController.gameObject.tag == "Enemy")
                hpController.TakeDamage();
        }
    }

    void CooldownAttack() => _canAttack = true;

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null || _weaponItem == null)
            return;

        Gizmos.DrawSphere(attackPoint.position, _weaponItem.range);
    }

    void OnDestroy()
    {
        playerInput.attack -= Attack;
    }
}
