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
			Destroy(player.TargetToPunch.gameObject);
		}
	}
}
