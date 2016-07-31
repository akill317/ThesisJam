using UnityEngine;
using System.Collections;

public class AttackCheck : MonoBehaviour {

	public bool hittingOpponent;
	private bool touchingOpponent;
	Animator animator;
	// Use this for initialization
	void Start () {
		hittingOpponent = false;
		touchingOpponent = false;
		animator = GetComponent<Animator>();
	}

	void Update() {
		if(touchingOpponent && animator.GetCurrentAnimatorStateInfo(0).IsName("attack")){
			hittingOpponent = true;
		}
		else {
			hittingOpponent = false;
		}
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other) {
		if(other.gameObject.layer == 9) {
			touchingOpponent = true;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if(other.gameObject.layer == 9) {
			touchingOpponent = false;
		}
	}
}
