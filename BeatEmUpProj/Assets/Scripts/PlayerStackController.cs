using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStackController : MonoBehaviour
{
	private Stack<RagdollController> _ragdollStack = new Stack<RagdollController>();
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	[SerializeField] private GameObject _prefab;
	[SerializeField] private Transform _root;
	[SerializeField] private FixedJoint _fixedJoint;
	[SerializeField] private float _offset;
	private WaitForSeconds _delay;

	[SerializeField] private int _maxStackSize;

	private void Start()
	{
		_delay = new WaitForSeconds(0.25f);
	}
	public void AddRagdollToStack()
	{
		if (_ragdollStack.Count >= _maxStackSize) return;
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
			else {
				_ragdollStack.Peek().FixedJoint.connectedBody = ragdoll.HipsJoint;
			}
		}
	}

	private IEnumerator RemoveFromStack() {
		int initialCount = _ragdollStack.Count;
		for (int i = 0; i < initialCount; i++)
		{
			yield return _delay;
			if (_ragdollStack.Count > 0)
			{
				var removedRagdoll = _ragdollStack.Pop();
				Manager.ObjectPoolManager.ReturnObjectToPool(removedRagdoll.gameObject);
			}
			CurrencyManager.OnCurrencyAdded.Invoke(10);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag(Constants.MONEY_TAG)) {
			StartCoroutine(RemoveFromStack());
		}
	}
}
