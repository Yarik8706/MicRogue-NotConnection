using UnityEngine;
using UnityEngine.UI;

namespace Canvas
{
    public class GameRullsActiveButton : MonoBehaviour
    {
        [Header("Components")]
        public GameObject blackout;
        public GameObject[] gameRulls;
        public GameObject activeGameRull;

        private bool _isShowingGameRulls;
        private int _passedGameRulls;

        private void Start()
        {
            _passedGameRulls = 0;
            _isShowingGameRulls = false;
        }

        private void Update()
        {
            if (!_isShowingGameRulls) return;
            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2)) return;
            if (Input.touchCount > 0)
            {
                if (!Input.touches[0].phase.Equals(TouchPhase.Began)) return;
            }
            activeGameRull.SetActive(false);
            if (++_passedGameRulls >= gameRulls.Length)
            {
                blackout.SetActive(false);
                _isShowingGameRulls = false;
                _passedGameRulls = 0;
                return;
            }
            activeGameRull = gameRulls[_passedGameRulls];
            activeGameRull.SetActive(true);
        }

        public void StartShowGameRulls()
        {
            if (_isShowingGameRulls)
            {
                return;
            }
            activeGameRull = gameRulls[0];
            activeGameRull.SetActive(true);
            blackout.SetActive(true);
            _isShowingGameRulls = true;
        }
    }
}