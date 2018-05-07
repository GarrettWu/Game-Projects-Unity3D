using UnityEngine;
using System.Collections;

public class SoundManager : Singleton<SoundManager> {

	private AudioSource audioExplosion;
	private AudioSource audioPop1;
	private AudioSource audioPop2;
	private AudioSource audioButton;
	
	void Awake(){
		audioExplosion = GameObject.Find("SoundManager/AudioExplosion").GetComponent<AudioSource>();
		audioPop1 = GameObject.Find("SoundManager/AudioPop1").GetComponent<AudioSource>();
		audioPop2 = GameObject.Find("SoundManager/AudioPop2").GetComponent<AudioSource>();
		audioButton = GameObject.Find("SoundManager/AudioButton").GetComponent<AudioSource>();

	}
	
	public void PlayAudio(string audioName){
		if (audioName == "explosion")
			audioExplosion.Play();

		if (audioName == "pop1")
			audioPop1.Play();

		if (audioName == "pop2")
			audioPop2.Play();

		if (audioName == "pop"){
			int i = Random.Range (0, 2);
			if (i == 0){
				audioPop1.Play();
			} else {
				audioPop2.Play();
			}
		}

		if (audioName == "button")
			audioButton.Play();

	}
	
}
