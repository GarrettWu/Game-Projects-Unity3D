using UnityEngine;
using System.Collections;

public class SoundManager : Singleton<SoundManager> {

	private AudioSource audioExplosion;
	private AudioSource audioShoot;
	private AudioSource audioCharge;
	private AudioSource audioPower;
	private AudioSource audioButton;
	
	void Awake(){
		audioExplosion = GameObject.Find("SoundManager/AudioExplosion").GetComponent<AudioSource>();
		audioShoot = GameObject.Find("SoundManager/AudioShoot").GetComponent<AudioSource>();
		audioCharge = GameObject.Find("SoundManager/AudioCharge").GetComponent<AudioSource>();
		audioPower = GameObject.Find("SoundManager/AudioPower").GetComponent<AudioSource>();
		audioButton = GameObject.Find("SoundManager/AudioButton").GetComponent<AudioSource>();

	}
	
	public void PlayAudio(string audioName){
		if (audioName == "explosion")
			audioExplosion.Play();

		if (audioName == "shoot")
			audioShoot.Play();

		if (audioName == "charge")
			audioCharge.Play();

		if (audioName == "power")
			audioPower.Play();

		if (audioName == "button")
			audioButton.Play();

	}

	public void StopAudio(string audioName){
		if (audioName == "power")
			audioPower.Stop();
	}
}
