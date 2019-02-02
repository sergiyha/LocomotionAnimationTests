using System;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
	[SerializeField] private AnimationEventsHandler _animationEventHandler;


	//Rolling
	private bool _isRolling;

	[SerializeField] private float _rollingSpeed;

	private BaseSmt _rollingSmt;

	void Start()
	{
		controller = GetComponent<CharacterController>();
		//_rollingSmt = _animator.GetBehaviours<BaseSmt>().First(smt => smt.SMTName == "Rolling");
		;//.First(smt => smt.SMTName == "Rolling");
		 //_rollingSmt.StartSMT += OnRollingStarted;
		 //_rollingSmt.ExitSMT += OnRollingFinished;
		 //_animationEventHandler.RollingAnimationStarted += OnRollingStarted;
		_animationEventHandler.RollingAnimationFinished += OnRollingFinished;

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
			Debug.Log("2" + moveDirection);

			if (CheckMovementButtons() && !_isRolling)
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
				if (!_isRolling)
					StopingInertion();


				_animator.SetBool("IsMoving", false);
			}

			MoveInDirection();
			RotateBody(_cameraRotationVector.x, _cameraRotationVector.z);

		}
		Debug.Log("3" + moveDirection);
		// Apply gravity
		moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);

		Debug.Log("4" + moveDirection);
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
		//if (_isRolling)
		//{
		//}

		if (Input.GetKeyDown(KeyCode.Space) && !_isRolling)
		{
			OnRollingStarted();
			var normalized = _playerDirection.normalized;
			_playerDirection = new Vector3(normalized.x * _rollingSpeed, 0, normalized.z * _rollingSpeed);
			_animator.SetTrigger("Roll");
		}
	}


	private void OnRollingStarted()
	{
		_isRolling = true;
		Debug.LogError("start");
	}

	private void OnRollingFinished()
	{
		_isRolling = false;
		_playerDirection = _playerDirection / _slidingSettings.StopingRolingSpeed;

		Debug.LogError("finish");

	}

	private void RotateBody(float x, float z)
	{
		var rotation = Quaternion.LookRotation(new Vector3(x, 0, z));
		_body.transform.rotation = Quaternion.Lerp(_body.transform.rotation, rotation, _rotationSmoothness * Time.deltaTime);
	}

	private void MoveInDirection()
	{
		moveDirection = _playerDirection * speed;
	}

}

[Serializable]
public class SlidingSettings
{
	[Range(0, 10)]
	public float StopingSpeed = 3;

	[Range(0, 1)]
	public float StopChekingValue = 0.01f;

	[Range(0, 4)]
	public float StopingRolingSpeed = 1.0f;


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
