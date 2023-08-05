using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    #region Private fields
    [SerializeField] private byte _minutes;
    [SerializeField] private byte _hours;
    [SerializeField] private float _dayLightValue;
    [SerializeField] private Color _currentColor;
    [SerializeField] private Color _dayColor;
    [SerializeField] private Color _eveningColor;
    [SerializeField] private Color _nightColor;
    #endregion

    #region Public fields
    public static TimeManager Instance;
    #endregion

    #region Properties
    public float DayLightValue
    {
        get
        {
            return _dayLightValue;
        }

        set
        {
            _dayLightValue = value;
        }
    }

    public Color CurrentColor
    {
        get
        {
            return _currentColor;
        }

        set
        {
            _currentColor = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(CalculateTime());
        _hours = 6;
    }

    private IEnumerator CalculateTime()
    {
        float percentage;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (!GameManager.Instance.IsGameSession)
            {
                continue;
            }
            _minutes++;

            if (_minutes >= 60)
            {
                _hours++;
                _minutes = 0;
            }

            if (_hours >= 24)
            {
                _hours = 0;
            }

            //Night
            if (_hours >= 0 && _hours <= 3)
            {
                DayLightValue = 0f;
                CurrentColor = _nightColor;
            }

            //Morning
            if (_hours >= 4 && _hours <= 6)
            {
                percentage = (_minutes + (60 * (_hours - 4))) / 180f;
                DayLightValue = Mathf.Lerp(0f, 1f, percentage);
                CurrentColor = Color.Lerp(_nightColor, _dayColor, percentage);
            }

            //Day
            if (_hours >= 7 && _hours <= 17)
            {
                DayLightValue = 1f;
                CurrentColor = _dayColor;
            }

            //Evening
            if (_hours >= 18 && _hours <= 23)
            {
                percentage = (_minutes + (60 * (_hours - 18))) / 360f;
                DayLightValue = Mathf.Lerp(1f, 0f, percentage);
                if (_hours >= 18 && _hours <= 20)
                {
                    percentage = (_minutes + (60 * (_hours - 18))) / 180f;
                    CurrentColor = Color.Lerp(_dayColor, _eveningColor, percentage);
                }
                if (_hours >= 21 && _hours <= 23)
                {
                    percentage = (_minutes + (60 * (_hours - 21))) / 180f;
                    CurrentColor = Color.Lerp(_eveningColor, _nightColor, percentage);
                }
            }
        }
    }
    #endregion
}
