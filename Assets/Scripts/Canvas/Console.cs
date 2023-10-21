using System;
using System.Collections.Generic;
using MainScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Canvas
{
    public class Console : MonoBehaviour
    {
        [SerializeField] private GameObject consoleLog;
        [SerializeField] private GameObject consoleLogsContainer;
        [SerializeField] private GameObject console;
        [SerializeField] private GameObject openConsoleButton;

        private readonly List<GameObject> _logs = new();

        public static Console instance;

        private void Start()
        {
            instance = this;
        }

        private void Awake()
        {
            Application.logMessageReceived += CreateNewLog;
        }

        public void ActiveConsoleButton()
        {
            openConsoleButton.SetActive(true);
        }
        
        private void OnDestroy()
        {
            Application.logMessageReceived -= CreateNewLog;
        }

        public void CreateNewLog(string condition, string stacktrace, LogType type)
        {
            var log = Instantiate(consoleLog, consoleLogsContainer.transform);
            log.GetComponent<TMP_Text>().text
                = "Log: " + condition + " \nLog type: " + type;
            _logs.Add(log);
        }

        public void ChangeConsoleActive()
        {
            console.SetActive(!console.activeSelf);
        }

        public void NextRoom()
        {
            CameraSwipeControl.sensitivity = 0.2f;
            // foreach (var exit in GameManager.instance.activeRoomController.exits)
            // {
            //     if (exit.isActive) StartCoroutine(GameManager.instance.NextRoom(exit.exitLocation));
            // }
        }

        public void ResetConsole()
        {
            CameraSwipeControl.sensitivity = 20;
            // while (_logs.Count > 0)
            // {
            //     Destroy(_logs[0]);
            // }
        }

        public void RestartGame()
        {
            CameraSwipeControl.sensitivity = 200;
            // StartCoroutine(GameManager.instance.TurnStarted());
        }

        [ContextMenu("CreateError")]
        public void CreateError()
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}