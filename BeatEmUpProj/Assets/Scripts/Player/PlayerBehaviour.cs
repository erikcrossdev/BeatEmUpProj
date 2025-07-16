using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerBehaviour : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		[Header("Combat")]
		[SerializeField] private List<Transform> _targets = new List<Transform>();
		[SerializeField] private float _radiusToInteract = 5f;
		[SerializeField] private float _viewAngle = 60f; //field of view of the player
		[SerializeField] private float _checkInterval = 0.3f;
		[SerializeField] private LayerMask _punchLayer;

		private float _checkTimer;
		private Transform _targetToPunch;
		public Transform TargetToPunch => _targetToPunch;

		private List<Transform> _detectedObjects = new List<Transform>();
		private Collider[] _collidersInRadius = new Collider[10]; // Ajuste conforme necessário

		private int _punchHash;

		public PlayerStackController StackController { get; private set; }

		private void Start()
		{
			_punchHash = Animator.StringToHash(Constants.PUNCH_ANIM_PARAM);
			StackController = GetComponent<PlayerStackController>();
		}

		void Update()
		{
			_checkTimer += Time.deltaTime;

			if (_checkTimer >= _checkInterval)
			{
				DetectEnemiesToPunch();
				_checkTimer = 0f; 
			}
		}

		public void RemoveTargetToPunch()
		{
			_targetToPunch = null;
		}

		private void DetectEnemiesToPunch()
		{
			_detectedObjects.Clear();
			//overlapShpereNonAlloc is better for a large amount of game objects
			int numColliders = Physics.OverlapSphereNonAlloc(transform.position, _radiusToInteract, _collidersInRadius, _punchLayer);
			//Debug.Log("num colliders " + numColliders);
			for (int i = 0; i < numColliders; i++)
			{
				Transform target = _collidersInRadius[i].transform;
				if (IsObjectInFrontAndInRadius(target, _viewAngle))
				{
					if (StackController.CurrentStackSize < PowerUpsManager.CurrentStackCapacity)
					{
						_animator.SetTrigger(_punchHash);
						_targetToPunch = target;
					}
					else {
						PopUpMessage.OnFadeAnim.Invoke();
					}
				}
			}
		}

		private bool IsObjectInFrontAndInRadius(Transform target, float angle)
		{
			Vector3 directionToTarget = (target.position - transform.position).normalized;
			float angleBetween = Vector3.Angle(transform.forward, directionToTarget);
			return angleBetween <= angle; //use FOV to define if it is in front of the player
		}

		private void OnDrawGizmos()
		{
			//visualize punch radius
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, _radiusToInteract);
		}

	}
}