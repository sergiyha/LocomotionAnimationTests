using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform Target;

	[Range(1f, 50f)] public float Distance;

	[Range(10f, 45f)] public float VerticalLock;

	public Vector3 Offset;

	private Quaternion _rotation;

	private void Start()
	{
		transform.LookAt(Target.position + Offset);
		_rotation = transform.rotation;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		_rotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X"), Vector3.up) * _rotation;
		var delta = Input.GetAxis("Mouse Y");

		var nextRotation = _rotation * Quaternion.AngleAxis(delta, Vector3.left);

		var nextAngle = Quaternion.Angle(nextRotation,
			Quaternion.LookRotation(Vector3.right) * nextRotation *
			Quaternion.Inverse(Quaternion.LookRotation(Vector3.right)));

		if (nextAngle < VerticalLock) _rotation = nextRotation;
	}

	private void LateUpdate()
	{
		RaycastHit hit;
		var dist = Distance;

		if (Physics.SphereCast(new Ray(Target.position + Offset, _rotation * Vector3.back), .5f, out hit, Distance))
			dist = hit.distance;

		transform.position = Target.position + Offset - _rotation * Vector3.forward * dist;
		transform.LookAt(Target.position + Offset);
	}
}