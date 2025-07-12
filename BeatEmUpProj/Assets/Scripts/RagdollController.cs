using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public FixedJoint FixedJoint { get; private set; }
   
    [SerializeField] private Rigidbody _hipsJoint;
    public Rigidbody HipsJoint => _hipsJoint;

	public Rigidbody RootRigidbody { get; private set; }


	private void Awake()
	{
		RootRigidbody = gameObject.GetComponent<Rigidbody>();
		FixedJoint = GetComponent<FixedJoint>();

	}
	public void AddRigidBodyToFixedJoint(Rigidbody body) {
		FixedJoint.connectedBody = body;
    }

}
