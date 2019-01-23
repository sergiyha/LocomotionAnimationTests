using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionSMB : StateMachineBehaviour
{
	public float Damping;

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		var x = Input.GetAxis("Horizontal");
		var y = Input.GetAxis("Vertical");

		animator.SetFloat("X", x, Damping, Time.deltaTime);
		animator.SetFloat("Y", y, Damping, Time.deltaTime);
	}
}
