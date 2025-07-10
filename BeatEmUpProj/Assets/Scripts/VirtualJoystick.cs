using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
	[SerializeField] private Image _joystickBackground;
	[SerializeField] private Image _joystickHandle;
	[SerializeField] private float _joystickRange = 100f;

	private Vector2 _inputVector;
	private RectTransform _backgroundRect;

	private void Start()
	{
		_backgroundRect = _joystickBackground.GetComponent<RectTransform>();
	}

	public void OnDrag(PointerEventData eventData)
	{
		Vector2 localPoint;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
			_backgroundRect,
			eventData.position,
			eventData.pressEventCamera,
			out localPoint))
		{
			// normalize point on drag
			localPoint.x = (localPoint.x / _backgroundRect.sizeDelta.x) * 2;
			localPoint.y = (localPoint.y / _backgroundRect.sizeDelta.y) * 2;

			_inputVector = (localPoint.magnitude > 1f) ? localPoint.normalized : localPoint;

			// handle movement
			_joystickHandle.rectTransform.anchoredPosition =
				new Vector2(_inputVector.x * _joystickRange, _inputVector.y * _joystickRange);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		OnDrag(eventData);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_inputVector = Vector2.zero;
		_joystickHandle.rectTransform.anchoredPosition = Vector2.zero;
	}

	public float Horizontal()
	{
		return _inputVector.x;
	}

	public float Vertical()
	{
		return _inputVector.y;
	}

	public Vector2 Direction()
	{
		return new Vector2(Horizontal(), Vertical());
	}
}