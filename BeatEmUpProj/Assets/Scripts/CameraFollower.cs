using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollower : MonoBehaviour
{
	[Header("Follow Player Settings")]
	
	[SerializeField] private float _distance = 5f; 
	[SerializeField] private float _height = 2f; 
	[SerializeField] private float _smoothSpeed = 5f; 

	[Header("Rotation")]
	[SerializeField] private bool _lookAtPlayer = true; 
	[SerializeField] private float _rotationDamping = 3f; 

	private Vector3 _offset;
	private Transform _playerTransform;

	private void Start()
	{
		_playerTransform = ServiceLocator.Instance.GetService<PlayerBehaviour>().transform;
		
		//Initial Position
		CalculateOffset();
	}
	
	private void FixedUpdate()
	{
		if (_playerTransform == null) return;

		// Update camera position
		Vector3 desiredPosition = _playerTransform.position + _offset;
		transform.position = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);

		if (_lookAtPlayer)
		{
			Quaternion desiredRotation = Quaternion.LookRotation(_playerTransform.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, _rotationDamping * Time.deltaTime);
		}
	}

	private void CalculateOffset()
	{
		_offset = -_playerTransform.forward * _distance;
		_offset.y = _height;
	}
}
