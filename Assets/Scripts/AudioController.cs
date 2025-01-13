using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpactSounds
{
    POSITIVE_HIT,
    NEGATIVE_HIT,
    DUCK_SOUND,
    RELOAD_SOUND,
}

public enum SpawnSounds
{
    DUCK_SPAWN,
    DUCK_DESSPAWN,
    SIGN_SPAWN,
    SIGN_DESPAWN,
}



public class AudioController : MonoBehaviour
{
    public AudioSource audioSource; // El AudioSource principal

    [Header("Impact Sounds")]
    public AudioClip[] impactSounds; // Clips de sonido de impacto

    [Header("Spawn Sounds")]
    public AudioClip[] spawnSounds; // Clips de sonidos de aparición

    

    
    
    // Reproducir un sonido de impacto
    public void PlayImpactSound(ImpactSounds sound)
    {
        int index = (int)sound;
        if (index >= 0 && index < impactSounds.Length)
        {
            audioSource.PlayOneShot(impactSounds[index]);
        }
        else
        {
            Debug.LogWarning($"Impact sound index {index} is out of range!");
        }
    }

    // Reproducir un sonido de aparición
    public void PlaySpawnSound(SpawnSounds sound)
    {
        int index = (int)sound;
        if (index >= 0 && index < spawnSounds.Length)
        {
            audioSource.PlayOneShot(spawnSounds[index]);
        }
        else
        {
            Debug.LogWarning($"Spawn sound index {index} is out of range!");
        }
    }
}