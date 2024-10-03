using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundEffects : MonoBehaviour
{
    [SerializeField] public AudioSource src;
    [SerializeField] public AudioClip takeDamageSound, healingSound, atackSound, keySound, doorSound;
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
    public void PlayAtackSound()
    {
        src.clip = atackSound;
        src.Play();
    }
    public void PlayKeySound()
    {
        src.clip = keySound;
        src.Play();
    }
    public void PlayDoorSound()
    {
        src.clip = doorSound;
        src.Play();
    }

}
