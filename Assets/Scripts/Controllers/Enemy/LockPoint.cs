using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPoint
{
	public Vector3 Position { get; protected set; }

	public void UpdatePosition(Vector3 position)
	{
		Position = position;
	}
}
