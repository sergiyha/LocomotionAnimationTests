using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
		_animatorController = GetComponent<Animator>();
	}

	private Animator _animatorController;

	// Update is called once per frame
	void Update()
	{

		var x = Input.GetAxis("Horizontal");
		var y = Input.GetAxis("Vertical");

		_animatorController.SetFloat("X", x);
		_animatorController.SetFloat("Y", y);
	}
}
