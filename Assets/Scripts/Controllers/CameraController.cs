using System;
using System.Runtime.InteropServices;
using Extensions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class CameraController : MonoBehaviour
{
	public Transform Target;

	[Range(1f, 50f)] public float Distance;

	[Range(10f, 45f)] public float VerticalLock;
	[SerializeField] private EnemyManager _enemyManager;
	[SerializeField] private Transform _cameraParentTr;
	[SerializeField] private Transform _cameraTransform;

	public bool IsLock => _lockPoint != null;
	public Vector3 Offset;

	private Quaternion _rotation;
	private LockPoint _lockPoint;

	[Range(0f, 1f)] [SerializeField] private float _followCameraDampTime;
	[Range(0, 40)] [SerializeField] private int _decreaseDirectionValue;

	private void Awake()
	{
		_cameraParentTr.LookAt(Target.position + Offset);
		_rotation = _cameraParentTr.rotation;
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void FollowCamera(Vector3 xz_movDir)
	{
		_cameraTransform.position = new Vector3
		(CustomMathF.DoubleSmoothStep(_cameraTransform.transform.position.x - xz_movDir.x / _decreaseDirectionValue, _cameraParentTr.position.x, _followCameraDampTime),
		_cameraTransform.position.y,
		CustomMathF.DoubleSmoothStep(_cameraTransform.transform.position.z - xz_movDir.z / _decreaseDirectionValue, _cameraParentTr.position.z, _followCameraDampTime));
	}

	public static float QuadraticSmoothStep(float from, float to, float t)
	{
		t = Mathf.Clamp01(t);
		t = t * t;
		return (float)((double)to * (double)t + (double)from * (1.0 - (double)t));
	}

	[Header("Additional rotation settings")]
	[Range(0, 45)] [SerializeField] private float _additionalRotationSpeed;

	[Range(0, 1f)] [SerializeField] private float _mouseYAxisValueToStopRotation = 0.01f;
	[Range(0, 1f)] [SerializeField] private float _startRotationWithHorizontalAxisMorThan = 0.1f;
	[Range(0, 1f)] [SerializeField] private float _stopRotationWithVerticalAxisMorThan = 0.3f;


	private void Update()
	{

		if (Input.GetMouseButtonDown(2))
		{
			if (IsLock)
			{
				_lockPoint = null;
			}
			else
			{
				var enemy = _enemyManager.GetEnemyForLock(Target.position + Offset, _rotation);
				if (enemy != null) _lockPoint = enemy.GetLockPoint(_cameraParentTr.position);
			}
		}

		var nextRotation = _rotation;

		if (IsLock)
		{
			var dif = _lockPoint.Position - (Target.position + Offset);
			if (dif.magnitude < 10f)
			{
				nextRotation = Quaternion.LookRotation(dif.normalized);
				nextRotation = Quaternion.RotateTowards(_rotation, nextRotation, 360f * Time.deltaTime);
			}
			else
			{
				_lockPoint = null;
			}
		}
		else
		{
			_rotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X"), Vector3.up) * _rotation;
			var delta = Input.GetAxis("Mouse Y");
			nextRotation = _rotation * Quaternion.AngleAxis(delta, Vector3.left);

			var x_horizontal = Input.GetAxis("Horizontal");
			var y_vertical = Input.GetAxis("Vertical");

			if (Mathf.Abs(y_vertical) < _stopRotationWithVerticalAxisMorThan &&
				Mathf.Abs(x_horizontal) > _startRotationWithHorizontalAxisMorThan &&
				Mathf.Abs(delta) < 0.01)//add rotation if runnig left or right
			{
				nextRotation =
					Quaternion.AngleAxis(_additionalRotationSpeed * Time.deltaTime * x_horizontal,
						Vector3.up) * nextRotation;
			}
		}

		var nextAngle = Quaternion.Angle(nextRotation,
			Quaternion.LookRotation(Vector3.right) * nextRotation *
			Quaternion.Inverse(Quaternion.LookRotation(Vector3.right)));

		if (nextAngle < VerticalLock) _rotation = nextRotation;
		else
			_lockPoint = null;
	}

	private void LateUpdate()
	{
		RaycastHit hit;
		var dist = Distance;

		if (Physics.SphereCast(new Ray(Target.position + Offset, _rotation * Vector3.back), .25f, out hit, Distance))
			dist = hit.distance;

		_cameraParentTr.position = Target.position + Offset - _rotation * Vector3.forward * dist;
		_cameraParentTr.LookAt(Target.position + Offset);
	}

	public Vector3 GetDirectionToTarget()
	{
		return new Vector3(Target.transform.position.x - _cameraParentTr.position.x, 0f,
			Target.transform.position.z - _cameraParentTr.position.z);
	}
}