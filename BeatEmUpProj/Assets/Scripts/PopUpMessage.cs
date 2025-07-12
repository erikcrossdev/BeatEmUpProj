using System;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent (typeof(Animator))]
public class PopUpMessage : MonoBehaviour
{
    private int _triggerHash;
    private Animator _animator;

    public static Action OnFadeAnim;
	public static Action OnResetMessage;

	private bool _messageShown = false;

	private void OnEnable()
	{
        OnFadeAnim += PlayAnimation;
		OnResetMessage += BuyPowerUpOrEmptyStack;
	}

	private void OnDisable()
	{
		OnFadeAnim -= PlayAnimation;
		OnResetMessage -= BuyPowerUpOrEmptyStack;
	}

    private void PlayAnimation() {
		if (_messageShown) return;
        _animator.SetTrigger(_triggerHash);
		_messageShown = true;

	}

	private void BuyPowerUpOrEmptyStack() {
		_messageShown = false;
	}

	void Start()
    {
        _animator = GetComponent<Animator> ();
        _triggerHash = Animator.StringToHash(Constants.FADE_TRIGGER);
	}

  
}
