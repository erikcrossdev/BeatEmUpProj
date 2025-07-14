using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
	public class PlayerStackController : MonoBehaviour
	{
		private Stack<RagdollController> _ragdollStack = new Stack<RagdollController>();
		[SerializeField] private float _ragdollSpacing = 0.6f; 

		public int CurrentStackSize
		{
			get { return _ragdollStack.Count; }
		}

		[SerializeField] private GameObject _prefab;
		[SerializeField] private Transform _root;
		[SerializeField] private FixedJoint _fixedJoint;


		[Header("Inertia effect")]
		[SerializeField] private float _offset;
		[SerializeField] private float _jointBreakForce = Mathf.Infinity;
		private GameplayBalanceSettings _balanceSettings;
		private WaitForSeconds _delay;

		private void Start()
		{
			_delay = new WaitForSeconds(0.25f);
			_balanceSettings = ServiceLocator.Instance.GetService<GameplayBalanceSettings>();
			
			// Configura joint do root para ser mais forte
			if (_fixedJoint != null)
			{
				_fixedJoint.breakForce = _jointBreakForce;
				_fixedJoint.breakTorque = _jointBreakForce;
				_fixedJoint.enablePreprocessing = true;
			}
		}
		public void AddRagdollToStack()
		{
			if (_ragdollStack.Count >= PowerUpsManager.CurrentStackCapacity) return;

			Vector3 position = new Vector3(
				_root.position.x,
				_root.position.y + (_ragdollSpacing * _ragdollStack.Count), // usa spacing fixo
				_root.position.z);

			var ragdoll = Manager.ObjectPoolManager.SpawnObject(_prefab, position, _root).GetComponent<RagdollController>();

			if (ragdoll != null)
			{
				_ragdollStack.Push(ragdoll);
				ragdoll.Initialize(_ragdollStack.Count - 1, PowerUpsManager.CurrentStackCapacity);

				if (_ragdollStack.Count == 1)
				{
					_fixedJoint.connectedBody = ragdoll.HipsJoint;
					ragdoll.SetFollowTarget(_root);
				}
				else
				{
					var previousRagdoll = _ragdollStack.ToArray()[1]; // pega o de baixo da pilha (não o topo)
					previousRagdoll.AddRigidBodyToFixedJoint(ragdoll.HipsJoint);

					previousRagdoll.FixedJoint.breakForce = _jointBreakForce;
					previousRagdoll.FixedJoint.breakTorque = _jointBreakForce;

					ragdoll.SetFollowTarget(previousRagdoll.transform);
				}
			}
		}

		private IEnumerator RemoveFromStack()
		{
			int initialCount = _ragdollStack.Count;
			for (int i = 0; i < initialCount; i++)
			{
				yield return _delay;
				if (_ragdollStack.Count > 0)
				{
					var removedRagdoll = _ragdollStack.Pop();
					removedRagdoll.ResetRagdoll();
					Manager.ObjectPoolManager.ReturnObjectToPool(removedRagdoll.gameObject);
				}
				CurrencyManager.OnCurrencyAdded.Invoke(_balanceSettings.GameplayBalance.MoneyForEachRagdoll);
			}
			_fixedJoint.connectedBody = null;
			PopUpMessage.OnResetMessage.Invoke();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag(Constants.MONEY_TAG))
			{
				StartCoroutine(RemoveFromStack());
			}
		}
	}
}
