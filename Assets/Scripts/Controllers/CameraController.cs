using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform Target;

	[Range(1f, 50f)] public float Distance;

	[Range(10f, 45f)] public float VerticalLock;
	[SerializeField] private EnemyManager _enemyManager;

	public bool IsLock => _lockPoint != null;
	public Vector3 Offset;

	private Quaternion _rotation;
	private LockPoint _lockPoint;

	private void Awake()
	{
		transform.LookAt(Target.position + Offset);
		_rotation = transform.rotation;
		Cursor.lockState = CursorLockMode.Locked;
	}

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
				var enemy = _enemyManager.GetEnemyForLock(transform.position, _rotation);
				if (enemy != null) _lockPoint = enemy.GetLockPoint(transform.position);
			}
		}


		if (IsLock)
		{
			_rotation = Quaternion.LookRotation((_lockPoint.Position - Target.position + Offset).normalized);
		}
		else
		{
			_rotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X"), Vector3.up) * _rotation;
			var delta = Input.GetAxis("Mouse Y");

			var nextRotation = _rotation * Quaternion.AngleAxis(delta, Vector3.left);

			var nextAngle = Quaternion.Angle(nextRotation,
				Quaternion.LookRotation(Vector3.right) * nextRotation *
				Quaternion.Inverse(Quaternion.LookRotation(Vector3.right)));

			if (nextAngle < VerticalLock) _rotation = nextRotation;
		}
	}

	private void LateUpdate()
	{
		RaycastHit hit;
		var dist = Distance;

		if (Physics.SphereCast(new Ray(Target.position + Offset, _rotation * Vector3.back), .25f, out hit, Distance))
			dist = hit.distance;

		transform.position = Target.position + Offset - _rotation * Vector3.forward * dist;
		transform.LookAt(Target.position + Offset);
	}

	public Vector3 GetDirectionToTarget()
	{
		return new Vector3(Target.transform.position.x - transform.position.x, 0f,
			Target.transform.position.z - transform.position.z);
	}
}