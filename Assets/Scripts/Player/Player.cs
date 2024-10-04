using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mantega;

public class Player : MonoBehaviour
{
    [Header("Game")]
    public List<Key> keys = new();

    [Header("Player")]
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private HPController playerHealth;
    [SerializeField] private PlayerSoundEffects playerSoundEffects;

    [Header("Weapon")]
    [SerializeField] private ScriptableItem basicWeapon;

    private void Start()
    {
        if(playerAttack == null)
            Generics.FamilyTryGetComponent(gameObject, out playerAttack);

        if (playerMovement == null)
            Generics.FamilyTryGetComponent(gameObject, out playerMovement);

        if (playerHealth == null)
            Generics.FamilyTryGetComponent(gameObject, out playerHealth);

        if(playerSoundEffects == null)
            Generics.FamilyTryGetComponent(gameObject, out playerSoundEffects);
    }

    public void ResetPlayer() => ResetPlayer(transform.position);
    public void ResetPlayer(Vector2 spawn)
    {
        keys.Clear();

        playerHealth.ResetHP();
        playerAttack.ResetAttack();
        playerAttack.ActiveWeapon(basicWeapon);

        transform.position = spawn;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject gam = collision.gameObject;
        if (gam.TryGetComponent(out Door door))
        {
            foreach (Key key in keys)
            {
                if (door.IsDoorKey(key))
                { 
                    playerSoundEffects.PlayDoorSound();
                    keys.Remove(key);
                    key.UseKey();

                    
                    // Inicia a coroutine para esperar 1 segundo antes de ir para o pr�ximo n�vel
                    StartCoroutine(WaitAndGoToNextLevel(door));
                    break;
                }
            }
        }
    }

    private IEnumerator WaitAndGoToNextLevel(Door door)
    {
        yield return new WaitForSeconds(1.5f); 
        door.GoToNextLevel(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject gam = other.gameObject;
        if (gam.TryGetComponent(out Key key))
        {
            key.GetKey();
            key.ShowUIKey();
            keys.Add(key);
            playerSoundEffects.PlayKeySound();
        }
    }
}
