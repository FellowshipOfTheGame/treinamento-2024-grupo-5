using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundEffects : MonoBehaviour
{
    [SerializeField] public AudioSource src;
    [SerializeField] public AudioClip takeDamageSound, healingSound;
    public void PlayTakeDamageSound()
    {
        src.clip = takeDamageSound;
        src.Play();
    }
    public void PlayHealingSound()
    {
        src.clip = healingSound;
        src.Play();
    }
}
