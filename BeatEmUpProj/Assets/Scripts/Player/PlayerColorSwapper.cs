using System;
using UnityEngine;

namespace Player{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class PlayerColorSwapper : MonoBehaviour
    {
		private SkinnedMeshRenderer _renderer;
		private MaterialPropertyBlock _propBlock;
		public static Action OnColorChange;
		private GameplayBalanceSettings _balance;
		private int _currentIndex;

		private void OnEnable()
		{
			OnColorChange += ChangeColor;
		}
		private void OnDisable()
		{
			OnColorChange -= ChangeColor;
		}

		void Start()
        {
			_balance = ServiceLocator.Instance.GetService<GameplayBalanceSettings>();
			_renderer = GetComponent<SkinnedMeshRenderer>();
			_propBlock = new MaterialPropertyBlock();
		}

		public void ChangeColor()
		{
			_currentIndex = Mathf.Clamp(_currentIndex + 1,	0, 	_balance.GameplayBalance.PowerUpColorsFeedback.Length - 1);
			var newColor = _balance.GameplayBalance.PowerUpColorsFeedback[_currentIndex];
			_renderer.GetPropertyBlock(_propBlock);
			_propBlock.SetColor(Constants.COLOR_PROP, newColor);
			_renderer.SetPropertyBlock(_propBlock);
		}
	}
}
