using System.Collections;
using UnityEngine;
using Player;
public class TargetCharacter : MonoBehaviour
{
    
    [SerializeField] private GameObject _ragdollParent;
    [SerializeField] private Rigidbody _parentRigidbody;
	[SerializeField] private SphereCollider _spherePick;
	[SerializeField] private CapsuleCollider _capsuleCollider;
	private PlayerBehaviour _playerBehaviour;
	private Rigidbody[] _ragdoll;
	void Awake()
    {       
        _ragdoll = _ragdollParent.GetComponentsInChildren<Rigidbody>();		
		ToggleRagdoll(false);
		_spherePick.enabled = false;
	}
	private void Start()
	{
		_playerBehaviour = ServiceLocator.Instance.GetService<PlayerBehaviour>();
	}

	private void ActivatePickArea() {
		Destroy(gameObject,0.5f);
		_capsuleCollider.enabled = false; //remove punchable object detection
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
