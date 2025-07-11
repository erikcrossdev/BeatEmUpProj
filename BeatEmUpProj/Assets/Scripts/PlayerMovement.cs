using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] private float _moveSpeed = 5f;
	[SerializeField] private Animator _animator;
	private VirtualJoystick _joystick;

	private Rigidbody rb;
	private int _movementHash;

	void Start()
    {
		rb = GetComponent<Rigidbody>();
		_movementHash = Animator.StringToHash(Constants.MOVEMENT_ANIM_PARAM);//Use hash for optimization reasons
		_joystick = ServiceLocator.Instance.GetService<VirtualJoystick>();
		rb.freezeRotation = true;
		rb.linearDamping = 5f; //when player stops
	}

	private void FixedUpdate()
	{
		Vector2 direction = _joystick.Direction(); //Get Joystick Direction to move the player
		// new movement vector
		Vector3 movement = new Vector3(direction.x, 0, direction.y) * _moveSpeed;
		// apply velocity
		rb.linearVelocity = movement;

		_animator.SetFloat(_movementHash, movement.magnitude);

		// calculate the magnitude only to rotate, if needed
		if (direction.magnitude > 0.1f)
		{
			transform.rotation = Quaternion.LookRotation(movement);
		}
	}
}
