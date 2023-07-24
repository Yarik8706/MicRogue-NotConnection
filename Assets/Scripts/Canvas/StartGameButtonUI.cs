using System;
using DG.Tweening;
using MainScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Canvas
{
    public class StartGameButtonUI : MonoBehaviour
    {
        [SerializeField] private GameObject trainingQuestionUI;
        [SerializeField] private GameObject[] changeActiveObjects;

        private Vector3 startPosition;

        public void Active()
        {
            if (PlayerPrefsSafe.GetInt(TrainingController.PrefsTrainingName, 0) == 1)
            {
                SceneManager.LoadScene(1);
                return;
            }
            trainingQuestionUI.SetActive(true);
            foreach (var changeActiveObject in changeActiveObjects)
            {
                changeActiveObject.SetActive(false);
            }
        }

        public void StartTrained()
        {
            SceneManager.LoadScene(1);
        }

        public void CloseDialog()
        {
            PlayerPrefsSafe.SetInt(TrainingController.PrefsTrainingName, 1);
            SceneManager.LoadScene(1);
        }
    }
}