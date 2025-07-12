using UnityEngine;

[CreateAssetMenu(fileName = "GameplayBalance", menuName = "Scriptable Objects/GameplayBalance")]
public class GameplayBalance : ScriptableObject
{
    [SerializeField] private int _MoneyForEachRagdoll = 10;
    public int MoneyForEachRagdoll => _MoneyForEachRagdoll;
    [SerializeField] private int _InitialStackSize = 3;
    public int InitialStackSize => _InitialStackSize;

	[SerializeField] private int _stackIncreasePerPurchase = 1;
	public int StackIncreasePerPurchase => _stackIncreasePerPurchase;
}
