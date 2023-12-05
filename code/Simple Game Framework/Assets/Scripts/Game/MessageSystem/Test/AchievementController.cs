using UnityEngine;

namespace Game.MessageSystem.Test
{
    public class AchievementController : MonoBehaviour
    {
        private int _targetCount = 6;
        private int _curCount = 0;
        private void Awake()
        {
            MessageManager.Instance.Register<int>(TestMessageDefine.OnScoreChange, WhenChangeScore);
        }

        private void WhenChangeScore(int score)
        {
            _curCount++;
            if (_curCount == _targetCount)
            {
                Debug.Log("达成成就千变万化，增加5分");
                MessageManager.Instance.Send(TestMessageDefine.ChangeScore, 5);
            }
        }
    }
}