using System.Collections;
using UnityEngine;
using Player;
using Manager;
public class TargetCharacter : MonoBehaviour
{
    
    [SerializeField] private GameObject _ragdollParent;
    [SerializeField] private Rigidbody _parentRigidbody;
	[SerializeField] private SphereCollider _spherePick;
	[SerializeField] private CapsuleCollider _capsuleCollider;
	private PlayerBehaviour _playerBehaviour;
	private Rigidbody[] _ragdoll;

	WaitForSeconds _sleep;
	void Awake()
    {       
        _ragdoll = _ragdollParent.GetComponentsInChildren<Rigidbody>();		
		ToggleRagdoll(false);
		_spherePick.enabled = false;
	}
	private void Start()
	{
		_sleep = new WaitForSeconds(0.7f);
		_playerBehaviour = ServiceLocator.Instance.GetService<PlayerBehaviour>();
		Manager.ObjectPoolManager.AddExistingObjectToPool(gameObject);
	}

	private IEnumerator DisableRoutine() {
		yield return _sleep;
		Manager.ObjectPoolManager.ReturnObjectToPool(gameObject);
		
	}

	private void ActivatePickArea() {
		_capsuleCollider.enabled = false; //remove punchable object detection
		if (gameObject.activeSelf)
		{
			StartCoroutine(DisableRoutine());
		}
	}

	public void ToggleRagdoll(bool activation) {
        for (int i = 0; i < _ragdoll.Length; i++)
        {
            _ragdoll[i].isKinematic = !activation;
        }
        if (activation) {
			_parentRigidbody.AddForce(_playerBehaviour.transform.forward, ForceMode.Impulse);
					
			ActivatePickArea();
		}
    }    
}
