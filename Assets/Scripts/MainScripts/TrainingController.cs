using System;
using System.Collections;
using Canvas;
using Enemies;
using Other;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainScripts
{
    public class TrainingController : MonoBehaviour
    {
        [SerializeField] private Cause[] initialDialog;
        [SerializeField] private Cause[] moveDialog;
        [SerializeField] private Cause[] attackDialog;
        [SerializeField] private Cause[] endDialog;
        [SerializeField] private TrainingControllerUI dialogController;

        public static TrainingController instance;
        public const string PrefsTrainingName = "Training";

        internal TrainingPoint trainingPoint;
        
        private int _trainingPart = 0;
        private TheEnemy firstEnemy;

        private void Awake()
        {
            instance = this;
        }

        public IEnumerator Active()
        {
            yield return new WaitUntil(() => GameManager.player.isActive);
            trainingPoint.onPointEvent.AddListener(TrainingPointMoveEnded);
            dialogController.StartDialog(initialDialog, () => trainingPoint.gameObject.SetActive(true));
            
            yield return new WaitUntil(() => _trainingPart == 1);
            trainingPoint.gameObject.SetActive(false);
            dialogController.StartDialog(moveDialog, () => { });
            
            yield return new WaitUntil(() => !GameManager.player.isActive);
            yield return new WaitForSeconds(1.7f);
            dialogController.StartDialog(attackDialog, () => { });
            
            GameplayEventManager.OnGetAllEnemies.Invoke();
            firstEnemy = GameController.instance.allEnemies[0];
            yield return new WaitUntil(() => firstEnemy == null);
            dialogController.StartDialog(endDialog, () => _trainingPart++);
            
            yield return new WaitUntil(() => _trainingPart == 2);
            PlayerPrefsSafe.SetInt(PrefsTrainingName, 1);
            GameManager.instance.screenFader.fadeState = ScreenFader.FadeState.In;
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void TrainingPointMoveEnded()
        {
            _trainingPart++;
        }
    }
}