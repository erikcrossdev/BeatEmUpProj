using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Player;

public class PunchTarget : MonoBehaviour
{
	private PlayerBehaviour player;


	private void Start()
	{
		player = ServiceLocator.Instance.GetService<PlayerBehaviour>();
	}
	void PunchATarget() //called on animation event on puch frame
	{
		if (player.TargetToPunch != null)
		{
			TargetCharacter targetScript = player.TargetToPunch.gameObject.GetComponent<TargetCharacter>();

			if (targetScript != null)
			{
				targetScript.ToggleRagdoll(true);
				player.StackController.AddRagdollToStack();
				player.RemoveTargetToPunch();
			}			
		}
	}
}
