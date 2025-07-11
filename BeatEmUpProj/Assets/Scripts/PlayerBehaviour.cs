using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] private float _moveSpeed = 5f;
	[SerializeField] private Animator _animator;
	private VirtualJoystick _joystick;

	private Rigidbody rb;
	

	[Header("Combat")]
	[SerializeField] private List<Transform> _targets = new List<Transform>();
	[SerializeField] private float _radiusToInteract = 5f;
	[SerializeField] private float _viewAngle = 60f;
	[SerializeField] private float _checkInterval = 0.3f;
	[SerializeField] private LayerMask _pickLayer;
	[SerializeField] private LayerMask _punchLayer;

	private float _nextCheckTime;
	private Transform _targetToPunch;
	public Transform TargetToPunch => _targetToPunch;

	private List<Transform> _detectedObjects = new List<Transform>();
	private Collider[] _collidersInRadius = new Collider[10]; // Ajuste conforme necessário
	

	//Animation hashes
	private int _movementHash;
	private int _punchHash;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		_movementHash = Animator.StringToHash("Movement");//Use hash for optimization reasons
		_punchHash = Animator.StringToHash("Punch");
		_joystick = ServiceLocator.Instance.GetService<VirtualJoystick>();
		rb.freezeRotation = true;
		rb.linearDamping = 5f; // Ajuda a parar o personagem quando soltar o joystick
	}

	private void FixedUpdate()
	{
		Vector2 direction = _joystick.Direction(); //Get Joystick Direction to move the player

		// new movement vector
		Vector3 movement = new Vector3(direction.x, 0, direction.y) * _moveSpeed;

		// apply velocity
		rb.linearVelocity = movement;

		
		_animator.SetFloat(_movementHash, movement.magnitude);
		

		// calculate the magnitude only to rotate if needed
		if (direction.magnitude > 0.1f)
		{
			transform.rotation = Quaternion.LookRotation(movement);
		}
	}

	void Update()
	{
		if (Time.time >= _nextCheckTime) //avoid calling this logic every frame
		{
			DetectEnemiesToPunch(_punchHash);
			DetectEnemiesToPunch(_pickLayer);
			_nextCheckTime = Time.time + _checkInterval;
		}
	}

	private void DetectEnemiesToPunch(LayerMask layer)
	{
		_detectedObjects.Clear();
		//overlapShpereNonAlloc is better for a large amount of game objects
		int numColliders = Physics.OverlapSphereNonAlloc(transform.position, _radiusToInteract, _collidersInRadius, layer);
		Debug.Log("num colliders " + numColliders);
		for (int i = 0; i < numColliders; i++)
		{
			Transform target = _collidersInRadius[i].transform;
			if (IsObjectInFrontAndInRadius(target, _viewAngle))
			{
				int objectLayer = 1 << target.gameObject.layer; //move one bit with left shift
				if ((_punchLayer.value & objectLayer) != 0) //use And bitwise operation, if is zero means that the layer are not equal
				{
					_animator.SetTrigger(_punchHash);
					_targetToPunch = target;
				}
				else if ((_pickLayer.value & objectLayer) != 0)//use And bitwise operation
				{
					Debug.Log("PICK!!!");
				}
			}
			else {
				Debug.Log("Outside radius!!!");
			}
		}
	}

	private bool IsObjectInFrontAndInRadius(Transform target, float angle)
	{
		Vector3 directionToTarget = (target.position - transform.position).normalized;
		float angleBetween = Vector3.Angle(transform.forward, directionToTarget);
		Debug.Log($"{angleBetween <= angle}, {angle}, {angleBetween}");
		return angleBetween <= angle;
	}

	private void OnDrawGizmos()
	{
		//visualize punch radius
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, _radiusToInteract);
	}

}