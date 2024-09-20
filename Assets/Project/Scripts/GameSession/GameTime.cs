using SavageWorld.Runtime.Utilities.Others;
using UnityEngine;

namespace SavageWorld.Runtime.GameSession
{
    public class GameTime : Singleton<GameTime>
    {
        #region Fields
        [SerializeField]
        private float _cooldown;
        [SerializeField]
        private ulong _globalTime;
        [SerializeField]
        private int _days;
        [SerializeField]
        private int _minutes;
        [SerializeField]
        private int _hours;
        [SerializeField]
        private float _dayLightValue;
        [SerializeField]
        private Color _currentColor;
        [SerializeField]
        private Color _dayColor;
        [SerializeField]
        private Color _eveningColor;
        [SerializeField]
        private Color _nightColor;
        private float _currentCooldown;
        #endregion

        #region Properties
        public float DayLightValue
        {
            get
            {
                return _dayLightValue;
            }
        }

        public Color CurrentColor
        {
            get
            {
                return _currentColor;
            }
        }

        public ulong GlobalTime
        {
            get
            {
                return _globalTime;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        private void Start()
        {
            ResetTime();
        }

        private void FixedUpdate()
        {
            _currentCooldown += Time.fixedDeltaTime;
            IncreaseTime();
            ParseTime();
        }

        private void OnEnable()
        {
            ResetTime();
        }
        #endregion

        #region Public Methods
        public void ResetTime()
        {
            _globalTime = 360;
            _currentCooldown = 0f;
            ParseTime();
            SetSkyColor();
        }

        public void SetTime(ulong value)
        {
            _globalTime = value;
            ParseTime();
        }
        #endregion

        #region Private Methods
        private void IncreaseTime()
        {
            if (_currentCooldown >= _cooldown)
            {
                _currentCooldown = 0f;
                _globalTime++;
                SetSkyColor();
            }
        }

        private void ParseTime()
        {
            _days = (int)(_globalTime / 1440);
            _hours = (int)(_globalTime / 60 % 24);
            _minutes = (int)(_globalTime % 60);
        }

        private void SetSkyColor()
        {
            SetNight();
            SetMorning();
            SetDay();
            SetEvening();
        }

        private void SetEvening()
        {
            if (_hours >= 18 && _hours <= 23)
            {
                float percentage = (_minutes + 60 * (_hours - 18)) / 360f;
                _dayLightValue = Mathf.Lerp(1f, 0f, percentage);
                if (_hours >= 18 && _hours <= 20)
                {
                    percentage = (_minutes + 60 * (_hours - 18)) / 180f;
                    _currentColor = Color.Lerp(_dayColor, _eveningColor, percentage);
                }
                if (_hours >= 21 && _hours <= 23)
                {
                    percentage = (_minutes + 60 * (_hours - 21)) / 180f;
                    _currentColor = Color.Lerp(_eveningColor, _nightColor, percentage);
                }
            }
        }

        private void SetDay()
        {
            if (_hours >= 7 && _hours <= 17)
            {
                _dayLightValue = 1f;
                _currentColor = _dayColor;
            }
        }

        private void SetMorning()
        {
            if (_hours >= 4 && _hours <= 6)
            {
                float percentage = (_minutes + 60 * (_hours - 4)) / 180f;
                _dayLightValue = Mathf.Lerp(0f, 1f, percentage);
                _currentColor = Color.Lerp(_nightColor, _dayColor, percentage);
            }
        }

        private void SetNight()
        {
            if (_hours >= 0 && _hours <= 3)
            {
                _dayLightValue = 0f;
                _currentColor = _nightColor;
            }
        }
        #endregion
    }
}