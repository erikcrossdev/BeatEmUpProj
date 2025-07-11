using UnityEngine;

public class PunchTarget : MonoBehaviour
{
	private PlayerBehaviour player;

	private void Start()
	{
		player = ServiceLocator.Instance.GetService<PlayerBehaviour>();
	}
	void PunchATarget()
	{
		if (player.TargetToPunch != null)
		{

			TargetCharacter targetScript = player.TargetToPunch.gameObject.GetComponent<TargetCharacter>();

			if (targetScript != null)
			{
				targetScript.ToggleRagdoll(true);
				Debug.Log("Foi");
			}
			else {
				Debug.Log("Eita");
			}
			
			//Destroy(player.TargetToPunch.gameObject);
		}
	}
}
