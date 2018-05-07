using UnityEngine;
using System.Collections;

public class KillerAnim : MonoBehaviour {
	
	private GameObject player;
	private CharacterController controller;
	private PlayerController playerController;
	private Animator animator;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
		controller = player.GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		playerController = player.GetComponent<PlayerController>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
		
		if (controller.isGrounded)
		{
			animator.SetBool("Jump", false);
			
		}
		else
		{
			animator.SetBool("Jump", true);
//			Debug.Log(playerController.isGrounded);
		}
	
	}
}
