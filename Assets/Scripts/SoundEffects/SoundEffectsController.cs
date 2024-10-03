using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsController : MonoBehaviour
{
    [SerializeField] public AudioSource src;
    [SerializeField] public AudioClip gameOverSound, victorySound;
    


    public void PlayGameOverSound()
    {
        src.clip = gameOverSound;
        src.Play();
    }

    public void PlayVictorySound()
    {
        src.clip = victorySound;
        src.Play();
    }

    
}
