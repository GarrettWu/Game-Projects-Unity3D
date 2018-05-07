using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	private AudioSource audioDie;
	private AudioSource audioHit;
	private AudioSource audioPoint;
	private AudioSource audioSwooshing;
	private AudioSource audioWing;
	private AudioSource audioButton;
	private AudioSource audioCrack;

	void Awake(){
		audioDie = GameObject.Find("AudioController/AudioDie").GetComponent<AudioSource>();
		audioHit = GameObject.Find("AudioController/AudioHit").GetComponent<AudioSource>();
		audioPoint = GameObject.Find("AudioController/AudioPoint").GetComponent<AudioSource>();
		audioSwooshing = GameObject.Find("AudioController/AudioSwooshing").GetComponent<AudioSource>();
		audioWing = GameObject.Find("AudioController/AudioWing").GetComponent<AudioSource>();
		audioButton = GameObject.Find("AudioController/AudioButton").GetComponent<AudioSource>();
		audioCrack = GameObject.Find("AudioController/AudioCrack").GetComponent<AudioSource>();

	}

	public void PlayAudio(string audioName){
		if (audioName == "die")
			audioDie.Play();
		if (audioName == "hit")
			audioHit.Play();
		if (audioName == "point")
			audioPoint.Play();
		if (audioName == "swooshing")
			audioSwooshing.Play();
		if (audioName == "wing")
			audioWing.Play();
		if (audioName == "button")
			audioButton.Play();
		if (audioName == "crack")
			audioCrack.Play();

	}


}
