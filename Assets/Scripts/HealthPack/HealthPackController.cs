using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackController : MonoBehaviour
{
    [SerializeField] private int healing;  
    [SerializeField] private float cooldown;

    private Renderer[] childRenderers;
    private Collider healthPackCollider;

    private void Start()
    {
        // Obtém o Collider do objeto pai
        healthPackCollider = GetComponent<Collider>();

        // Obtém todos os Renderers dos filhos
        childRenderers = GetComponentsInChildren<Renderer>();
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
        foreach (Renderer rend in childRenderers)
        {
            rend.enabled = false;
        }

        // Desativa o Collider do pai
        healthPackCollider.enabled = false;

        // espera o cooldown
        yield return new WaitForSeconds(cooldown);

        foreach (Renderer rend in childRenderers)
        {
            rend.enabled = true;
        }

        // Reativa o Collider do pai
        healthPackCollider.enabled = true;
        Debug.Log("Health Pack reapareceu");
    }
}
