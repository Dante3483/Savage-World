using SavageWorld.Runtime.Console.Commands;
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
        private StringBuilder _logsStringBuilder;
        private bool _isLogsUpdated;
        private List<string> _history;
        private string _tempCommand;
        private int _currentHistoryIndex;
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
        }

        private void Update()
        {
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
            if (_isLogsUpdated)
            {
                Instance._logs.SetText(Instance._logsStringBuilder);
            }
        }
        #endregion

        #region Public Methods
        public void Toggle(CallbackContext context)
        {
            if (!context.action.triggered)
            {
                return;
            }
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

        public static void LogText(string text)
        {
            lock (Instance._logsStringBuilder)
            {
                Instance._logsStringBuilder.AppendLine(text);
                Instance._isLogsUpdated = true;
            }
        }

        public static void LogText(string text, Color color)
        {
            LogText($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>");
        }

        public static void LogError(string text)
        {
            LogText(text, Color.yellow);
        }

        public static void ClearText()
        {
            Instance._logsStringBuilder.Clear();
        }
        #endregion

        #region Private Methods
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
                LogText(inputValue);
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
        #endregion
    }
}
