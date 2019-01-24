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

	private float angle;

	void Start()
	{
		controller = GetComponent<CharacterController>();
		gameObject.transform.position = new Vector3(0, 5, 0);

	}

	void Update()
	{
		if (controller.isGrounded)
		{

			// We are grounded, so recalculate
			// move direction directly from axes
			var x_horizontal = Input.GetAxis("Horizontal");
			var y_vertical = Input.GetAxis("Vertical");

			var x_raw = (int)Input.GetAxisRaw("Horizontal");
			var y_raw = (int)Input.GetAxisRaw("Vertical");

			moveDirection = new Vector3(x_horizontal, 0.0f, y_vertical);
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection = moveDirection * speed;

			if (x_raw != 0 || y_raw != 0)
			{
				var from = new Vector3(0, 1);
				var to = new Vector3(x_horizontal, y_vertical);

				angle = Vector3.Angle(from, to);
				angle = (x_horizontal < 0) ? -angle : angle;
				_body.transform.rotation = Quaternion.Euler(0, angle, 0);

				_animationController.SetBool("IsMoving", true);

			}
			else
			{
				_animationController.SetBool("IsMoving", false);
			}

			_animationController.SetFloat("X", x_horizontal);
			_animationController.SetFloat("Y", y_vertical);
		}

		// Apply gravity
		moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);

		// Move the controller
		controller.Move(moveDirection * Time.deltaTime);
	}
}
