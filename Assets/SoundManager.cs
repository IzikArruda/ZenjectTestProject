using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/*
 * This sound manager has a link to an audioSource and will play the audioClips
 * that it recieves from the EntitySoundSignal signals.
 */
public class SoundManager : MonoBehaviour {

    public AudioSource _audioSource;

    public void RecievedSignal(EntitySoundSignal signal) {
        _audioSource.PlayOneShot(signal.GetClip());
    }
}
