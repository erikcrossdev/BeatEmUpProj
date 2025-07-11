using UnityEngine;

public class TargetCharacter : MonoBehaviour
{
    
    [SerializeField] private GameObject _ragdollParent;
    [SerializeField] private Rigidbody _parentRigidbody;
	[SerializeField] private SphereCollider _spherePick;
    private PlayerBehaviour _playerBehaviour;
	private Rigidbody[] _ragdoll;
    private bool _isDead = false;
	public string targetLayerName = "PickTarget"; 
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
		_spherePick.enabled = true;
	}

	public void ToggleRagdoll(bool activation) {
        for (int i = 0; i < _ragdoll.Length; i++)
        {
            _ragdoll[i].isKinematic = !activation;
        }
        if (activation) {
			_parentRigidbody.AddForce(_playerBehaviour.transform.forward, ForceMode.Impulse);
			_isDead = true;

			int layerID = LayerMask.NameToLayer(targetLayerName);
			if (layerID == -1)
			{
				Debug.LogError($"Layer '{targetLayerName}' não existe!");
				return;
			}
			_ragdollParent.layer = layerID;
			ActivatePickArea();
		}
    }


    
}
