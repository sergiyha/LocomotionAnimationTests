using System;
using System.Linq;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
	public float speed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;

	private Vector3 moveDirection = Vector3.zero;
	private CharacterController controller;
	[SerializeField] private GameObject _body;
	[SerializeField] private Animator _animator;

	public CameraController _cameraController;


	[SerializeField] private float _rotationSmoothness;

	[SerializeField] private SlidingSettings _slidingSettings;


	//Rolling
	private bool _isRolling;
	private BaseSmt _rollingSmt;

	void Start()
	{
		controller = GetComponent<CharacterController>();
		_rollingSmt = _animator.GetBehaviours<BaseSmt>().First(smt => smt.SMTName == "Rolling");
		;//.First(smt => smt.SMTName == "Rolling");
		_rollingSmt.StartSMT += OnRollingStarted;
		_rollingSmt.ExitSMT += OnRollingFinished;
		gameObject.transform.position = new Vector3(0, 5, 0);

	}


	private float x_inertion;
	private float y_inertion;

	private float x_horizontal;
	private float y_vertical;


	private Vector3 _cameraRotationVector;
	private Vector3 _playerDirection;

	void Update()
	{
		if (controller.isGrounded)
		{

			if (CheckMovementButtons())
			{
				x_horizontal = Input.GetAxis("Horizontal");
				y_vertical = Input.GetAxis("Vertical");
				_playerDirection = _cameraRotationVector = Quaternion.LookRotation(_cameraController.GetDirectionToTarget()) * new Vector3(x_horizontal, 0f, y_vertical);
				//rotate axis vector depends on camera direction


				_animator.SetBool("IsMoving", true);

				_animator.SetFloat("X", x_horizontal);
				_animator.SetFloat("Y", y_vertical);

				CheckRolling();
			}
			else
			{

				StopingInertion();

				_animator.SetBool("IsMoving", false);
			}

			MoveInDirection(_playerDirection.x, _playerDirection.z);
			RotateBody(_cameraRotationVector.x, _cameraRotationVector.z);

		}

		// Apply gravity
		moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);

		controller.Move(moveDirection * Time.deltaTime);
		// Move the controller

	}

	private void StopingInertion()
	{
		var x = _playerDirection.x;
		var z = _playerDirection.z;

		_slidingSettings.CheckAxesToStopIt(ref x);
		_slidingSettings.CheckAxesToStopIt(ref z);
		_playerDirection = new Vector3(x, 0, z);
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

	private void CheckRolling()
	{
		if (Input.GetKeyDown(KeyCode.Space) && !_isRolling)
		{
			_animator.SetTrigger("Roll");
		}
	}


	private void OnRollingStarted()
	{
		_isRolling = true;
	}

	private void OnRollingFinished()
	{
		_isRolling = false;
	}

	private void RotateBody(float x, float z)
	{
		var rotation = Quaternion.LookRotation(new Vector3(x, 0, z));
		_body.transform.rotation = Quaternion.Lerp(_body.transform.rotation, rotation, _rotationSmoothness * Time.deltaTime);
	}

	private void MoveInDirection(float x, float z)
	{
		moveDirection = new Vector3(x, 0.0f, z);
		//moveDirection = transform.TransformDirection(moveDirection);
		moveDirection = moveDirection * speed;
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
