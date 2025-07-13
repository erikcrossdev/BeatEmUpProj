using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currency;

	public static System.Action<int> OnCurrencyAdded;
	public static System.Action<int> OnBuyPowerUp;

	private int _currentCurrency;
	public int CurrentCurrency => _currentCurrency;

	private GameplayBalanceSettings _balanceSettings;
	private void Start()
	{
		_balanceSettings = ServiceLocator.Instance.GetService<GameplayBalanceSettings>();
		_currentCurrency = _balanceSettings.GameplayBalance.InitialCurrency;
		_currency.SetText(_currentCurrency.ToString());
	}
	private void OnEnable()
	{
		OnCurrencyAdded += UpdateScoreDisplay;
		OnBuyPowerUp += UpdateCurrency;
	}

	private void OnDisable()
	{
		OnCurrencyAdded -= UpdateScoreDisplay;
		OnBuyPowerUp -= UpdateCurrency;
	}

	private void UpdateCurrency(int valueToDecrease) {
		_currentCurrency -= valueToDecrease;
		PowerUpsManager.CheckIfCanBuyPowerUp.Invoke();
		_currency.SetText(_currentCurrency.ToString());
	}

	private void UpdateScoreDisplay(int score)
	{
		_currentCurrency += score;
		PowerUpsManager.CheckIfCanBuyPowerUp.Invoke();
		_currency.SetText(_currentCurrency.ToString());
	}
	
}
