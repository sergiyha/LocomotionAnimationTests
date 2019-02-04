using System;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float speed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;

	private Vector3 _moveDirection = Vector3.zero;
	private CharacterController _controller;
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

	private void Start()
	{
		_controller = GetComponent<CharacterController>();
		//_rollingSmt = _animator.GetBehaviours<BaseSmt>().First(smt => smt.SMTName == "Rolling");
		; //.First(smt => smt.SMTName == "Rolling");
		//_rollingSmt.StartSMT += OnRollingStarted;
		//_rollingSmt.ExitSMT += OnRollingFinished;
		//_animationEventHandler.RollingAnimationStarted += OnRollingStarted;
		_animationEventHandler.RollingAnimationFinished += OnRollingFinished;

		gameObject.transform.position = new Vector3(0, 5, 0);
		_playerDirection = _cameraRotationVector =
			Quaternion.LookRotation(_cameraController.GetDirectionToTarget()) * transform.forward;
	}

	private Vector3 _cameraRotationVector;
	private Vector3 _playerDirection;

	private void Update()
	{
		if (_controller.isGrounded)
		{
			//Debug.Log("2" + moveDirection);

			if (CheckMovementButtons() && !_isRolling)
			{
				var x_horizontal = Input.GetAxis("Horizontal");
				var y_vertical = Input.GetAxis("Vertical");
				_playerDirection = _cameraRotationVector =
					Quaternion.LookRotation(_cameraController.GetDirectionToTarget()) *
					new Vector3(x_horizontal, 0f, y_vertical);
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

				_cameraController.FollowCamera(_playerDirection);
			_moveDirection = _playerDirection * speed;
			RotateBody(_cameraRotationVector.x, _cameraRotationVector.z);
		}

		//Debug.Log("3" + moveDirection);
		// Apply gravity
		_moveDirection.y = _moveDirection.y - gravity * Time.deltaTime;

		//Debug.Log("4" + moveDirection);
		_controller.Move(_moveDirection * Time.deltaTime);
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
		var simultaneouslyDA = Input.GetKey(KeyCode.D) &&
		                       Input.GetKey(KeyCode.A);
		var simultaneouslyWS = Input.GetKey(KeyCode.W) &&
		                       Input.GetKey(KeyCode.S);

		var any = Input.GetKey(KeyCode.W) ||
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
			OnRollingStarted();
			var normalized = _playerDirection.normalized;
			_playerDirection = new Vector3(normalized.x * _rollingSpeed, 0, normalized.z * _rollingSpeed);
			_animator.SetTrigger("Roll");
		}
	}


	private void OnRollingStarted()
	{
		_isRolling = true;
		//Debug.LogError("start");
	}

	private void OnRollingFinished()
	{
		_isRolling = false;
		_playerDirection = _playerDirection / _slidingSettings.StopingRolingSpeed;

		//Debug.LogError("finish");
	}

	private void RotateBody(float x, float z)
	{
		var rotation = Quaternion.LookRotation(new Vector3(x, 0, z));
		_body.transform.rotation =
			Quaternion.Lerp(_body.transform.rotation, rotation, _rotationSmoothness * Time.deltaTime);
	}
}

[Serializable]
public class SlidingSettings
{
	[Range(0, 10)] public float StopingSpeed = 3;

	[Range(0, 1)] public float StopChekingValue = 0.01f;

	[Range(0, 4)] public float StopingRolingSpeed = 1.0f;


	public void CheckAxesToStopIt(ref float axisValue)
	{
		if (Mathf.Abs(axisValue) > StopChekingValue)
			axisValue = axisValue > 0
				? axisValue - Time.deltaTime * StopingSpeed
				: axisValue + Time.deltaTime * StopingSpeed;
		else
			axisValue = 0;
	}
}