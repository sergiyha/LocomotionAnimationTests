using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSmt : StateMachineBehaviour
{
	public string SMTName;

	public event Action StartSMT;
	public event Action ExitSMT;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		StartSMT?.Invoke();
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateExit(animator, stateInfo, layerIndex);
		ExitSMT?.Invoke();
	}

	public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
	{
		base.OnStateMachineExit(animator, stateMachinePathHash);
		Debug.LogError("Exit");
	}
}
