using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI fpsText;
	private float deltaTime;
	void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
		float fps = 1.0f / deltaTime;
		fpsText.text = Mathf.Ceil(fps).ToString();
	}

	void Start()
	{
		Application.targetFrameRate = 60;
	}
}
