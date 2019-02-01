using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsHandler : MonoBehaviour
{

	public event Action RollingAnimationStarted;
	public event Action RollingAnimationFinished;


	public void OnRollingAnimationStarted()
	{
		RollingAnimationStarted?.Invoke();
	}

	public void OnRollingAnimationFinished()
	{
		RollingAnimationFinished?.Invoke();
	}
}
