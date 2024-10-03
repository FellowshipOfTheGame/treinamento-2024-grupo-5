using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class EnemyHPController : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Hits taken to die")]
    private float hp;

    [SerializeField]
    [Tooltip("Boss HP Bar")]
    private Image HPBar;

    private float currenthp;


    void Start()
    {
        currenthp = hp;
    }

    void Update()
    {
        // util só pra barra de vida do boss
        if (HPBar != null)
            HPBar.fillAmount = (float)currenthp / hp;
    }

    public void TakeDamage()
    {
        currenthp -- ;

        if (currenthp <= 0) 
        {
            Die();
        }

        Debug.Log("Inimigo tomou dano");

    }

    public void Die()
    {
        // Fazer aqui comandos quando o inimigo morre
        Debug.Log("Inimigo morreu");
        Destroy(gameObject);
    }

    
}
