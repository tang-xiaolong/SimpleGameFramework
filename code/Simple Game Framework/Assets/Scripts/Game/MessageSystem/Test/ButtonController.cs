using UnityEngine;
using UnityEngine.UI;

namespace Game.MessageSystem.Test
{
    public class ButtonController : MonoBehaviour
    {
        public Button Btn;
        private int _changeScore = 1;

        private void Awake()
        {
            Btn.onClick.AddListener(ChangeScore);
            MessageManager.Instance.Register<int>(TestMessageDefine.OnScoreChange, OnScoreChange);
        }

        private void OnScoreChange(int score)
        {
            if (score <= 0)
            {
                if (_changeScore == -1)
                {
                    _changeScore = 1;
                }
            }
            else if(score >= 5)
            {
                if (_changeScore == 1)
                {
                    _changeScore = -1;
                }
            }
        }

        private void ChangeScore()
        {
            MessageManager.Instance.Send(TestMessageDefine.ChangeScore, _changeScore);
        }
    }
}