using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Need change
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoSingleton <SoundManager> {

	public AudioSource audioSourcePrefab;

	private Queue<AudioSource> freeAudioSourcesQueue;

	private void Awake()
	{
		freeAudioSourcesQueue = new Queue<AudioSource>();
	}

	/// <summary>
	/// Play given sound
	/// </summary>
	/// <param name="clip">audio clip to be played</param>
	/// <param name="pitch">pitch of sound</param>
	/// <param name="delay">delay before playing sound</param>
	public void Play (AudioClip clip, float pitch = 1f, float delay = 0f)
	{	
		StartCoroutine(PlaySoundOnAudioSource(clip, pitch, delay));
	}

	private IEnumerator PlaySoundOnAudioSource(AudioClip clip, float pitch, float delay)
	{
		if (delay > 0f)
			yield return new WaitForSeconds(delay);
		
		AudioSource source;
		if (freeAudioSourcesQueue.Count == 0)
			source = Instantiate(audioSourcePrefab, transform);
		else
			source = freeAudioSourcesQueue.Dequeue();

		source.clip = clip;
		source.pitch = pitch;
		source.Play();
		
		yield return new WaitUntil(() => !source.isPlaying);
		freeAudioSourcesQueue.Enqueue(source);
	}

}
