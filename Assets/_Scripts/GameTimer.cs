using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{    
    private TextMeshProUGUI _timerText;

    private float _timeElapsed = 0.0f;

    private void Awake()
    {
        this._timerText = GetComponent<TextMeshProUGUI>();
    }

    public void StartTimer()
    {
        this._timeElapsed = 0.0f;
        this._timerText.text = "";
        StartCoroutine(this.RunTimer());
    }

    public void StopTimer()
    {
        StopAllCoroutines();
    }

    private IEnumerator RunTimer()
    {
        yield return new WaitForFixedUpdate();
        this._timeElapsed += Time.fixedDeltaTime;
    }

    void FixedUpdate()
    {
        this._timeElapsed += Time.fixedDeltaTime;

        this._timerText.text = this.GetTimerString();
    }

    private List<int> GetTimerValues()
    {
        List<int> timerValues = new List<int>();

        int minutesValue = Mathf.FloorToInt(this._timeElapsed / 60.0f);
        timerValues.Add(minutesValue);        

        int secondsValue = Mathf.FloorToInt(this._timeElapsed - (minutesValue * 60));
        timerValues.Add(secondsValue);

        int millisecondsValue = (int)((this._timeElapsed - (minutesValue * 60) - secondsValue) * 100);

        timerValues.Add(millisecondsValue);

        return timerValues;
    }

    private string GetTimerString()
    {
        List<int> timerValues = this.GetTimerValues();

        string minutesValue = (timerValues[0] > 9) ? timerValues[0].ToString() : "0" + timerValues[0].ToString();
        string secondsValue = (timerValues[1] > 9) ? timerValues[1].ToString() : "0" + timerValues[1].ToString();
        string millisecondsValue = (timerValues[2] > 9) ? timerValues[2].ToString() : "0" + timerValues[2].ToString();

        return minutesValue + ":" + secondsValue + ":" + millisecondsValue;
    }
}
