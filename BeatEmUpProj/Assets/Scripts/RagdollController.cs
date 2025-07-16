using UnityEngine;

public class RagdollController : MonoBehaviour
{
	public FixedJoint FixedJoint { get; private set; }
	[SerializeField] private Rigidbody _hipsJoint;
	public Rigidbody HipsJoint => _hipsJoint;
	public Rigidbody RootRigidbody { get; private set; }

	[Header("Inertia settings")]

	[SerializeField, Range(0.2f, 0.6f)] private float _positionOffset = 0.5f;
	[SerializeField, Range(2f, 4f)] private float _lerpInertiaSpeed = 2f;// the velocity of the lerp 
	[SerializeField, Range(0.5f, 5.0f)] private float _maxInertia = 1.2f; //limits the max offset
	[SerializeField, Range(30.0f, 80.0f)] private float _rotationAmount = 40f; //how much the ragdoll turns
	[SerializeField, Range(1.0f, 3.0f)] private float _stabilizeSpeed = 3f; //how fast the pile stabilizes
	[SerializeField, Range(2.0f, 15.0f)] private float _velocityMultiplier = 5.0f; //inertia velocity
	[SerializeField, Range(4f ,10.0f)] private float _swayFrequency = 6f; //balance velocity
	[SerializeField, Range(0.3f, 2.0f)] private float _swayAmplitude = 0.3f;//balance velocity

	private Transform _target;
	private Vector3 _lastTargetPos;
	private float _stackPositionFactor;
	private Vector3 _currentOffset;

	private void Awake()
	{
		RootRigidbody = GetComponent<Rigidbody>();
		FixedJoint = GetComponent<FixedJoint>();

		if (RootRigidbody != null)
		{
			RootRigidbody.isKinematic = true;
			RootRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			RootRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
		}

	}
	public void Initialize(float stackPosition, int stackCount)
	{
		_stackPositionFactor = stackPosition / stackCount;

		//adjust offset on the pile
		transform.localPosition = new Vector3(0, _positionOffset * stackPosition, 0);
				
	}
	public void AddRigidBodyToFixedJoint(Rigidbody body)
	{
		FixedJoint.connectedBody = body;
	}

	public void ResetRagdoll()
	{
		FixedJoint.connectedBody = null;

		_target = null;
		_lastTargetPos = Vector3.zero;
		_currentOffset = Vector3.zero;

		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}
	public void SetFollowTarget(Transform target)
	{
		_target = target;
		_lastTargetPos = target.position;
	}
	private void FixedUpdate()
	{
		if (_target == null) return;

		float verticalOffset = _positionOffset * (_stackPositionFactor * PowerUpsManager.CurrentStackCapacity);
		//makes the first element movement more rigid
		if (_stackPositionFactor <= 0f)
		{
			transform.localPosition = new Vector3(0f, verticalOffset, 0f);
			transform.localRotation = Quaternion.identity;
			return;
		}
		//calculate position comparing to last fixed update
		Vector3 worldDelta = _target.position - _lastTargetPos;
		_lastTargetPos = _target.position;

		Vector3 localDelta = _target.InverseTransformDirection(worldDelta);//transforms to world space to know if the ragdoll is moving to the front/back/left/right

		Vector3 desiredOffset = new Vector3(-localDelta.x, 0f, -localDelta.z) * _velocityMultiplier;//add inertia, inverting the localDelta positions
		float lerpFactor = Mathf.Lerp(0.2f, 1f, _stackPositionFactor); //lerp acording to the position in the stack
		desiredOffset *= lerpFactor; 

		desiredOffset = Vector3.ClampMagnitude(desiredOffset, _maxInertia); //limits the offset to avoid undesired effects

		_currentOffset = Vector3.Lerp(_currentOffset, desiredOffset, Time.fixedDeltaTime * _stabilizeSpeed); //smooths using the stabilized speed

		//use sin and cos curves to create a more "cartoonish" effect
		float swayTime = Time.time * _swayFrequency + _stackPositionFactor;
		float swayAmount = _swayAmplitude * _stackPositionFactor;
		Vector3 sway = new Vector3(
			Mathf.Sin(swayTime) * swayAmount,
			0f,
			Mathf.Cos(swayTime) * swayAmount
		);

		Vector3 finalOffset = _currentOffset + sway; //sums offsets effects

		Vector3 desiredLocalPos = new Vector3(finalOffset.x, verticalOffset, finalOffset.z);
		transform.localPosition = Vector3.Lerp(transform.localPosition, desiredLocalPos, Time.deltaTime * _lerpInertiaSpeed);

		float yaw = finalOffset.x * _rotationAmount * _stackPositionFactor; //turn to the sides 
		float pitch = -finalOffset.z * _rotationAmount * _stackPositionFactor; //turn to front and back

		Quaternion desiredRot = Quaternion.Euler(pitch, 0f, yaw);
		transform.localRotation = Quaternion.Slerp(transform.localRotation, desiredRot, Time.deltaTime * _lerpInertiaSpeed); //smooth rotation
	}


}
