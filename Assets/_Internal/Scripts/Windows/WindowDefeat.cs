using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WindowDefeat : MonoBehaviour
{
    public TextMeshProUGUI scoreField;
    public TextMeshProUGUI timerField;
    public TextMeshProUGUI enemiesField;
    public TextMeshProUGUI accuracyField;

    public void ReloadValues()
    {
        scoreField.text = GameHandler.Instance.GetScore().ToString();
        timerField.text = GameHandler.Instance.GetTimer();
        enemiesField.text = GameHandler.Instance.GetKills().ToString();
        accuracyField.text = GameHandler.Instance.GetAccuracy().ToString("0.00") + "%";
    }
}
