using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
	public class PlayerStackController : MonoBehaviour
	{
		private Stack<RagdollController> _ragdollStack = new Stack<RagdollController>();

		public int CurrentStackSize
		{
			get { return _ragdollStack.Count; }
		}
		// Start is called once before the first execution of Update after the MonoBehaviour is created
		[SerializeField] private GameObject _prefab;
		[SerializeField] private Transform _root;
		[SerializeField] private FixedJoint _fixedJoint;
		[SerializeField] private float _offset;
		private GameplayBalanceSettings _balanceSettings;
		private WaitForSeconds _delay;

		public static Action<int> OnIncreaseStackSize;

		private void OnEnable()
		{
			OnIncreaseStackSize += IncreaseStackSize;
		}

		private void OnDisable()
		{
			OnIncreaseStackSize += IncreaseStackSize;
		}

		private void IncreaseStackSize(int value) {
			PowerUpsManager.CurrentStackCapacity += value;
		}

		private void Start()
		{
			_delay = new WaitForSeconds(0.25f);
			_balanceSettings = ServiceLocator.Instance.GetService<GameplayBalanceSettings>();
		}
		public void AddRagdollToStack()
		{
			if (_ragdollStack.Count >= PowerUpsManager.CurrentStackCapacity) return;
			Vector3 position = new Vector3(
										_root.transform.position.x,
										_root.transform.position.y + (_offset * _ragdollStack.Count),
										_root.transform.position.z);
			var ragdoll = Manager.ObjectPoolManager.SpawnObject(_prefab, position, _root).GetComponent<RagdollController>();
			if (ragdoll != null)
			{
				_ragdollStack.Push(ragdoll);
				//connect joints
				if (_ragdollStack.Count == 1)
				{
					_fixedJoint.connectedBody = ragdoll.HipsJoint;
				}
				else
				{
					_ragdollStack.Peek().FixedJoint.connectedBody = ragdoll.HipsJoint;
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
					Manager.ObjectPoolManager.ReturnObjectToPool(removedRagdoll.gameObject);
				}
				CurrencyManager.OnCurrencyAdded.Invoke(_balanceSettings.GameplayBalance.MoneyForEachRagdoll);
			}
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
