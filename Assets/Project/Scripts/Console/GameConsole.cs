using SavageWorld.Runtime.Console.Commands;
using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Utilities.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace SavageWorld.Runtime.Console
{
    public class GameConsole : Singleton<GameConsole>
    {
        #region Fields
        [SerializeField]
        private string _prefix;
        [SerializeField]
        private DeveloperCommand[] _commands;
        [SerializeField]
        private RectTransform _content;
        [SerializeField]
        private TMP_Text _logs;
        [SerializeField]
        private TMP_InputField _commandInputField;
        private PlayerInputActions _inputActions;
        private StringBuilder _logsStringBuilder;
        private bool _isLogsUpdated;
        private List<string> _history;
        private string _tempCommand;
        private int _currentHistoryIndex;
        private bool _lastFocus;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        protected override void Awake()
        {
            base.Awake();
            _logsStringBuilder = new();
            _history = new();
            _inputActions = GameManager.Instance.InputActions;
            _inputActions.Debug.Enable();
        }

        private void OnEnable()
        {
            _inputActions.Debug.ToggleDeveloperConsole.performed += OnToggleDeveloperConsole;
        }

        private void OnDisable()
        {
            _inputActions.Debug.ToggleDeveloperConsole.performed -= OnToggleDeveloperConsole;
        }

        private void Update()
        {
            bool focus = _commandInputField.isFocused;
            if (_lastFocus != focus)
            {
                FocusChanged(focus);
                _lastFocus = focus;
            }
            if (_content.gameObject.activeSelf)
            {
                if (Input.GetKeyUp(KeyCode.Return))
                {
                    ProcessCommand(_commandInputField.text);
                    _commandInputField.text = string.Empty;
                    _commandInputField.ActivateInputField();
                }
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    Toggle();
                }
                if (Input.GetKeyUp(KeyCode.UpArrow) && _currentHistoryIndex > 0)
                {
                    if (_currentHistoryIndex == _history.Count)
                    {
                        _tempCommand = _commandInputField.text;
                    }
                    _currentHistoryIndex--;
                    _commandInputField.text = _history[_currentHistoryIndex];
                    _commandInputField.caretPosition = _commandInputField.text.Length;
                }
                if (Input.GetKeyUp(KeyCode.DownArrow) && _currentHistoryIndex < _history.Count)
                {
                    _currentHistoryIndex++;
                    if (_currentHistoryIndex == _history.Count)
                    {
                        _commandInputField.text = _tempCommand;
                        _commandInputField.caretPosition = _commandInputField.text.Length;
                    }
                    else
                    {
                        _commandInputField.text = _history[_currentHistoryIndex];
                        _commandInputField.caretPosition = _commandInputField.text.Length;
                    }
                }
            }
            _logs.SetText(_logsStringBuilder);
            _isLogsUpdated = false;
            if (_isLogsUpdated)
            {
                _logs.SetText(_logsStringBuilder);
                _isLogsUpdated = false;
            }
        }
        #endregion

        #region Public Methods
        public static void Log(string text)
        {
            lock (Instance._logsStringBuilder)
            {
                Instance._logsStringBuilder.AppendLine(text);
                Instance._isLogsUpdated = true;
            }
        }

        public static void Log(string text, Color color)
        {
            Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>");
        }

        public static void LogError(string text)
        {
            Log(text, Color.yellow);
        }

        public static void ClearText()
        {
            Instance._isLogsUpdated = true;
            Instance._logsStringBuilder.Clear();
        }
        #endregion

        #region Private Methods
        private void FocusChanged(bool focus)
        {
            if (focus)
            {
                _inputActions.Player.Disable();
                _inputActions.UI.Disable();
                _inputActions.Interactions.Disable();
            }
            else
            {
                _inputActions.Player.Enable();
                _inputActions.UI.Enable();
                _inputActions.Interactions.Enable();
            }
        }

        private void Toggle()
        {
            if (_content.gameObject.activeSelf)
            {
                _content.gameObject.SetActive(false);
            }
            else
            {
                _content.gameObject.SetActive(true);
                _commandInputField.ActivateInputField();
            }
        }

        private void ProcessCommand(string inputValue)
        {
            if (!inputValue.StartsWith(_prefix))
            {
                Log(inputValue);
                return;
            }
            string inputValueWithoutPrefix = inputValue.Remove(0, _prefix.Length);
            string[] inputSplit = inputValueWithoutPrefix.Split(' ');
            string commandInput = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();
            ProcessCommand(commandInput, args);
            AddCommandToHistory(inputValue);
        }

        private void ProcessCommand(string commandInput, string[] args)
        {
            foreach (IDeveloperCommand command in _commands)
            {
                if (!commandInput.Equals(command.CommandWord, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                if (command.Process(args))
                {
                    return;
                }
            }
        }

        private void AddCommandToHistory(string command)
        {
            _history.Add(command);
            _currentHistoryIndex = _history.Count;
        }

        private void OnToggleDeveloperConsole(CallbackContext context)
        {
            if (_commandInputField.isFocused)
            {
                return;
            }
            Toggle();
            if ((int)context.ReadValue<float>() == 2)
            {
                _commandInputField.text = "/";
                _commandInputField.caretPosition = 1;
            }
        }
        #endregion
    }
}
