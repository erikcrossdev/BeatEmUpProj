using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpsManager : MonoBehaviour
{
    [SerializeField] Button _buyPowerUp;
	[SerializeField] TextMeshProUGUI _buttonText;
    [SerializeField] TextMeshProUGUI _capacityText;

	private CurrencyManager _currencyManager;
	private GameplayBalanceSettings _balanceSettings;
	public static int CurrentStackCapacity;

	private int _currentPowerUpPrice;

    public static Action<int> OnCurrentStackIncrease;
	public static Action CheckIfCanBuyPowerUp;

	//Use string builder to avoid garbage collection on create new strings objects
	private static readonly StringBuilder _stringBuilder = new StringBuilder(20);

	private void Start()
	{
		_balanceSettings = ServiceLocator.Instance.GetService<GameplayBalanceSettings>();
		_currencyManager = ServiceLocator.Instance.GetService<CurrencyManager>();
		_buyPowerUp.onClick.AddListener(BuyPowerUp);
		CurrentStackCapacity = _balanceSettings.GameplayBalance.InitialStackSize;
		CalculatePrice(CurrentStackCapacity);
	}

	private void CalculatePrice(int currentStackSize) {
		
		_currentPowerUpPrice = _balanceSettings.GameplayBalance.MoneyForEachRagdoll * currentStackSize;
		_capacityText.SetText(CurrentStackCapacity.ToString());
		UpdateButtonText();
	}

	private void UpdateButtonText()
	{
		_stringBuilder.Clear();
		_stringBuilder.Append(Constants.BUY_FOR);
		_stringBuilder.Append(_currentPowerUpPrice.ToString()); 
		_buttonText.text = _stringBuilder.ToString();
	}

	private void BuyPowerUp() {
		if (_currentPowerUpPrice > _currencyManager.CurrentCurrency) return;

		CurrencyManager.OnBuyPowerUp.Invoke(_currentPowerUpPrice);
		PopUpMessage.OnResetMessage.Invoke();
		Player.PlayerColorSwapper.OnColorChange.Invoke();
		CurrentStackCapacity += _balanceSettings.GameplayBalance.StackIncreasePerPurchase;
		CalculatePrice(CurrentStackCapacity);
		CheckIfCanBuyPowerUp.Invoke();

	}

	private void CheckIfHaveEnoughMoney() {
		_buyPowerUp.interactable = _currentPowerUpPrice <= _currencyManager.CurrentCurrency;
	}

	private void OnEnable()
	{
		OnCurrentStackIncrease += UpdateCapacity;
		CheckIfCanBuyPowerUp += CheckIfHaveEnoughMoney;
	}

	private void OnDisable()
	{
		OnCurrentStackIncrease -= UpdateCapacity;
		CheckIfCanBuyPowerUp -= CheckIfHaveEnoughMoney;
	}

	private void UpdateCapacity(int increase)
	{
		CurrentStackCapacity += increase;
		_capacityText.text = CurrentStackCapacity.ToString();
	}
	void Update()
    {
        
    }
}
