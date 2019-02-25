using UnityEngine;

public class InputProvider : MonoBehaviour
{
	public static Vector2 Axis { get; private set; }
	public static Vector2 NormalizedAxis { get; private set; }


	private void Update()
	{
		Axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		NormalizedAxis = Axis.normalized;
	}


}
