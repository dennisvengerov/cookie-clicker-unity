using UnityEngine;
using TMPro;

public class StoreUpgrade : MonoBehaviour
{
    [Header("Components")]
    public TMP_Text priceText;
    public TMP_Text incomeInfoText;
   public int startPrice = 15;
   public float upgradePriceMultiplier;
   public float cookiesPerUpgrade=0.1f;

   

   int level = 0;


    private void Start(){
        UpdateUI();
    }

    public void ClickAction(){
        
    }

    void UpdateUI() {
        priceText.text = CalculatePrice().ToString();
        incomeInfoText.text = level.ToString() + " x " + cookiesPerUpgrade + "/s";
    }

    int CalculatePrice() {
        int price = Mathf.RoundToInt(startPrice*Mathf.Pow(upgradePriceMultiplier, level));
        return price;
    }

    public float CalculateIncomePerSecond(){
        return cookiesPerUpgrade*level;
    }
}


