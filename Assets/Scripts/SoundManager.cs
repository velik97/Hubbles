using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoSingleton <SoundManager> {

	public AudioSource audioSource;

	public void Play (AudioClip clip) {
		audioSource.clip = clip;
		audioSource.Play ();
	}

}
