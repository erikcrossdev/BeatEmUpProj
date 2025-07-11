using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currency;
	public static System.Action<int> OnCurrencyAdded;
	private int _currentCurrency;

	private void Start()
	{
		_currentCurrency = 0;
		_currency.SetText(_currentCurrency.ToString());
	}
	private void OnEnable()
	{
		OnCurrencyAdded += UpdateScoreDisplay;
	}

	private void OnDisable()
	{
		OnCurrencyAdded -= UpdateScoreDisplay;
	}

	private void UpdateScoreDisplay(int score)
	{
		_currentCurrency += score;
		_currency.SetText(_currentCurrency.ToString());
	}
	
}
