using System;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
	public float speed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;

	private Vector3 moveDirection = Vector3.zero;
	private CharacterController controller;
	[SerializeField] private GameObject _body;
	[SerializeField] private Animator _animationController;

	public CameraController _cameraController;
	private Quaternion _facing;

	private float angle;

	[SerializeField] private SlidingSettings _slidingSettings;

	void Start()
	{
		controller = GetComponent<CharacterController>();
		gameObject.transform.position = new Vector3(0, 5, 0);
		_facing = transform.rotation;

	}

	private float x_horizontal;
	private float y_vertical;
	void Update()
	{
		if (controller.isGrounded)
		{
			if (CheckMovementButtons())
			{
				x_horizontal = Input.GetAxis("Horizontal");
				y_vertical = Input.GetAxis("Vertical");
				RotatePlayerWithCameraDirection();
				RotateBody(x_horizontal, y_vertical);

				_animationController.SetBool("IsMoving", true);

				_animationController.SetFloat("X", x_horizontal);
				_animationController.SetFloat("Y", y_vertical);
			}
			else
			{

				StopingInertion();

				_animationController.SetBool("IsMoving", false);
			}

			MoveInDirection(x_horizontal, y_vertical);

		}


		// Apply gravity
		moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);

		controller.Move(moveDirection * Time.deltaTime);
		// Move the controller

	}


	private void StopingInertion()
	{
		_slidingSettings.CheckAxesToStopIt(ref x_horizontal);
		_slidingSettings.CheckAxesToStopIt(ref y_vertical);
	}


	private bool CheckMovementButtons()
	{
		bool simultaneouslyDA = (Input.GetKey(KeyCode.D) &&
					  Input.GetKey(KeyCode.A));
		bool simultaneouslyWS = (Input.GetKey(KeyCode.W) &&
					  Input.GetKey(KeyCode.S));

		bool any = Input.GetKey(KeyCode.W) ||
				   Input.GetKey(KeyCode.S) ||
				   Input.GetKey(KeyCode.A) ||
				   Input.GetKey(KeyCode.D);
		//Debug.LogError("ad: " + simultaneouslyDA + " ws: " + simultaneouslyWS + " any: " + any);

		return any && !simultaneouslyWS && !simultaneouslyDA;

	}

	private void RotateBody(float x, float z)
	{
		var from = new Vector3(0, 1);
		var to = new Vector3(x, z);

		angle = Vector3.Angle(from, to);
		angle = (x < 0) ? -angle : angle;
		_body.transform.localRotation = Quaternion.Euler(0, angle, 0);
	}

	private void MoveInDirection(float x, float z)
	{
		moveDirection = new Vector3(x, 0.0f, z);
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection = moveDirection * speed;
	}

	private void RotatePlayerWithCameraDirection()
	{
		var rotation = Quaternion.LookRotation(_cameraController.GetDirectionToTarget());
		rotation *= _facing;
		transform.rotation = rotation;

	}
}

[Serializable]
public class SlidingSettings
{
	[Range(0, 10)]
	public float StopingSpeed = 3;

	[Range(0, 1)]
	public float StopChekingValue = 0.01f;

	public void CheckAxesToStopIt(ref float axisValue)
	{
		if (Mathf.Abs(axisValue) > StopChekingValue)
		{
			axisValue = (axisValue > 0) ?
				axisValue - Time.deltaTime * StopingSpeed :
				axisValue + Time.deltaTime * StopingSpeed;
		}
		else
		{
			axisValue = 0;
		}
	}

}
