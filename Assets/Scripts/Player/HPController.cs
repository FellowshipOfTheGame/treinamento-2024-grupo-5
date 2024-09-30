using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HPController : MonoBehaviour
{
    [SerializeField] public Image HPBar;
    [SerializeField] private int MaxHP;
    [SerializeField] private int CurrentHP;

    void Start()
    {
        CurrentHP = MaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (HPBar != null)
            HPBar.fillAmount = (float)CurrentHP / MaxHP;
    }

    public int getMaxHP() { return MaxHP; }
    public int getCurrentHP() { return CurrentHP; }

    public void LoseHP(int Damage)
    {
        Debug.Log($"{name} perdeu vida");
        CurrentHP -= Damage;

        if (CurrentHP <= 0) // Condicao de derrota
        {
            // GameOver();
            Debug.Log("Game Over");
        }
    }

    public void GainHP(int HPGained)
    {
        CurrentHP += HPGained;

        if (CurrentHP > MaxHP) // Se ficou com vida maior que a vida maxima
        {
            CurrentHP = MaxHP;  
        }
    }

    public void GameOver()
    {
        
    }

    //Funcao para testar perda de vida e cura quando toca em inimigo
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LoseHP(10); 

        }
    }

    
}
