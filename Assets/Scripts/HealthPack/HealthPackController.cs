using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackController : MonoBehaviour
{
    [SerializeField] private int healing;  
    [SerializeField] private float cooldown;

    private Renderer healthPackRenderer;
    private Collider healthPackCollider;

    private void Start()
    {
        healthPackRenderer = GetComponent<Renderer>();
        healthPackCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger acionado");

        Transform rootTransform = other.transform.root;
        if (rootTransform.CompareTag("Player"))
        {
            Debug.Log("detectou jogador");
            HPController hp = rootTransform.GetComponent<HPController>();

            if (hp != null && hp.getCurrentHP() < hp.getMaxHP())
            {
                Debug.Log("curando");
                hp.GainHP(healing);

                if(cooldown < 0)
                {
                    //Deixar cooldown negativo faz o healthpack nao reespawnar
                    gameObject.SetActive(false);
                }

                //Intervalo de respawn
                StartCoroutine(RespawnHealthPack());
                
                    
            }
        }
    }

    IEnumerator RespawnHealthPack()
    {
        // desativa renderizador e collider
        healthPackRenderer.enabled = false;
        healthPackCollider.enabled = false;
        Debug.Log("Health Pack em cooldown");

        // espera o cooldown
        yield return new WaitForSeconds(cooldown);

        // reativa renderizador e collider
        healthPackRenderer.enabled = true;
        healthPackCollider.enabled = true;
        Debug.Log("Health Pack reapareceu");
    }
}
