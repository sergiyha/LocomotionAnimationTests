using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
	[SerializeField] private float Speed;


	void Update ()
	{
		var x = Input.GetAxis("Horizontal");
		var z = Input.GetAxis("Vertical");

		//transform.Translate();
	}
}
