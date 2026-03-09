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

        this._timerText.text = this.GetTimerString(this._timeElapsed);
    }

    private List<int> GetTimerValues(float rawTime)
    {
        List<int> timerValues = new List<int>();

        int minutesValue = Mathf.FloorToInt(rawTime / 60.0f);
        timerValues.Add(minutesValue);        

        int secondsValue = Mathf.FloorToInt(rawTime - (minutesValue * 60));
        timerValues.Add(secondsValue);

        int millisecondsValue = (int)((rawTime - (minutesValue * 60) - secondsValue) * 100);

        timerValues.Add(millisecondsValue);

        return timerValues;
    }

    public string GetTimerString(float rawTime)
    {
        List<int> timerValues = this.GetTimerValues(rawTime);

        string minutesValue = (timerValues[0] > 9) ? timerValues[0].ToString() : "0" + timerValues[0].ToString();
        string secondsValue = (timerValues[1] > 9) ? timerValues[1].ToString() : "0" + timerValues[1].ToString();
        string millisecondsValue = (timerValues[2] > 9) ? timerValues[2].ToString() : "0" + timerValues[2].ToString();

        return minutesValue + ":" + secondsValue + ":" + millisecondsValue;
    }

    public float GetRawTime()
    {
        return this._timeElapsed;
    }

    public string GetCurrentTimerString()
    {
        return this.GetTimerString(this._timeElapsed);
    }

    public string ConvertTimeToString(float rawTime)
    {
        return this.GetTimerString(rawTime);
    }
}
