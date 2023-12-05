using UnityEngine;

namespace Game.MessageSystem.Test
{
    public class ScoreController : MonoBehaviour
    {
        [SerializeField]
        private int _score = 0;
    
        private void Awake()
        {
            MessageManager.Instance.Register<int>(TestMessageDefine.ChangeScore, ChangeScore);
        }

        private void Start()
        {
            _score = 0;
            MessageManager.Instance.Send(TestMessageDefine.OnScoreChange, _score);
        }

        private void ChangeScore(int changeScore)
        {
            _score += changeScore;
            MessageManager.Instance.Send(TestMessageDefine.OnScoreChange, _score);
        }
    }
}

