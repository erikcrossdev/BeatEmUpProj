using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
	[Header("Components")]
	[SerializeField] private RectTransform _background;
	[SerializeField] private RectTransform _handle;

	[Header("Settings")]
	[SerializeField] private float _handleRange = 100f;
	[SerializeField] private Vector2 _padding = new Vector2(100, 100); // Margens X e Y

	private Vector2 _inputVector;
	private Vector2 _backgroundCenter;
	private bool _isActive;

	public float Horizontal => _isActive ? _inputVector.x : 0f;
	public float Vertical => _isActive ? _inputVector.y : 0f;
	public Vector2 Direction => _isActive ? _inputVector : Vector2.zero;
	private void Awake()
	{
		if (_background == null || _handle == null)
		{
			Debug.LogError("Joystick components not assigned!");
			enabled = false;
			return;
		}

		SetupJoystickPosition();
	}

	private void SetupJoystickPosition()
	{
		// Fix to inferior border
		_background.anchorMin = new Vector2(0, 0);
		_background.anchorMax = new Vector2(0, 0);
		_background.pivot = new Vector2(0.5f, 0.5f);
		_background.anchoredPosition = new Vector2(
			_padding.x + (_background.sizeDelta.x * 0.5f),
			_padding.y + (_background.sizeDelta.y * 0.5f)
		);

		_backgroundCenter = _background.position;
		ResetHandle();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		OnDrag(eventData);
		_isActive = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		Vector2 touchPosition = eventData.position;
		Vector2 direction = touchPosition - _backgroundCenter;

		float maxDistance = _handleRange * Mathf.Min(
			_background.lossyScale.x,
			_background.lossyScale.y
		);

		// Limits the movement 
		if (direction.magnitude > maxDistance)
		{
			direction = direction.normalized * maxDistance;
		}

		_inputVector = direction / maxDistance;
		_handle.position = _backgroundCenter + direction;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		ResetHandle();
		_isActive = false;
	}

	private void ResetHandle()
	{
		_handle.position = _backgroundCenter;
		_inputVector = Vector2.zero;
	}

}