using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mantega;

public class Player : MonoBehaviour
{
    [Header("Game")]
    public int keys = 0;

    [Header("Player")]
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private HPController playerHealth;

    [Header("Weapon")]
    [SerializeField] private ScriptableItem basicWeapon;

    private void Start()
    {
        if(playerAttack == null)
            Generics.FamilyTryGetComponent(gameObject, out playerAttack);
        playerAttack.ResetAttack();

        if (playerMovement == null)
            Generics.FamilyTryGetComponent(gameObject, out playerMovement);

        if (playerHealth == null)
            Generics.FamilyTryGetComponent(gameObject, out playerHealth);
        playerHealth.ResetHP();
    }

    public void ResetPlayer() => ResetPlayer(transform.position);
    public void ResetPlayer(Vector2 spawn)
    {
        keys = 0;

        playerHealth.ResetHP();
        playerAttack.ResetAttack();
        playerAttack.ActiveWeapon(basicWeapon);

        transform.position = spawn;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject gam = collision.gameObject;
        if (keys > 0 && gam.CompareTag("Door"))
        {
            Destroy(gam);
            keys--;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject gam = other.gameObject;
        Debug.Log(gam.name);
        Debug.Log(gam.tag);
        if (gam.CompareTag("Key"))
        {
            Destroy(gam);
            keys++;
        }
    }
}
