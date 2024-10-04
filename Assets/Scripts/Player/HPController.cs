using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        //Debug.Log($"{name} perdeu vida");
        CurrentHP -= Damage;

        if (CurrentHP <= 0) // Condicao de derrota
        {
            GameOver();
            Debug.Log("Game Over");
        }
        else
        {
            GetComponent<PlayerSoundEffects>().PlayTakeDamageSound();
        }
    }

    public void GainHP(int HPGained)
    {
        CurrentHP += HPGained;

        if (CurrentHP > MaxHP) // Se ficou com vida maior que a vida maxima
        {
            CurrentHP = MaxHP;  
        }

        GetComponent<PlayerSoundEffects>().PlayHealingSound();
    }

    public void GameOver()
    {
        // Encontra o objeto com a tag "GameController"
        GameObject gameController = GameObject.FindWithTag("GameController");

        if (gameController != null)
        {
            // Acessa o componente MenuController
            MenuController menuController = gameController.GetComponent<MenuController>();

            if (menuController != null)
            {
                // Chama o método GameOver no MenuController
                menuController.GameOver();
            }
            else
            {
                Debug.LogError("O GameObject com a tag 'GameController' não possui o componente MenuController.");
            }
        }
        else
        {
            Debug.LogError("Nenhum GameObject com a tag 'GameController' foi encontrado.");
        }
    }

    public void ResetHP()
    {
        CurrentHP = MaxHP;
    }

    //Funcao para testar perda de vida e cura quando toca em inimigo
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LoseHP(10); 

        }
        else if (collision.gameObject.CompareTag("Win"))
        {
            GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
            if (gameController != null)
            {
                gameController.GetComponent<MenuController>().Victory();
                
            }
        }
    }

    
}
