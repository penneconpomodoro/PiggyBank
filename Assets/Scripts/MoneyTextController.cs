using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyTextController : MonoBehaviour
{
    private const int durationForTempEarnedMoney = 120;
    private GameDirector gameDirector;
    private int counter;
    private long oldEarnedMoney;
    private long tempEarnedMoney;

    // Start is called before the first frame update
    void Start()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        counter = 0;
        oldEarnedMoney = 0;
        tempEarnedMoney = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Text moneyText = GetComponent<Text>();
        moneyText.text = string.Format("たまったお金：{0:#,0}円", gameDirector.EarnedMoney);
        if (oldEarnedMoney < gameDirector.EarnedMoney)
        {
            counter = durationForTempEarnedMoney;
            tempEarnedMoney = gameDirector.EarnedMoney - oldEarnedMoney;
        }
        counter--;
        counter = Mathf.Max(0, counter);
        if (counter > 0)
        {
            moneyText.text += string.Format("\n  増えたお金：{0:+#,0}円", tempEarnedMoney);
        }
        oldEarnedMoney = gameDirector.EarnedMoney;
    }
}
