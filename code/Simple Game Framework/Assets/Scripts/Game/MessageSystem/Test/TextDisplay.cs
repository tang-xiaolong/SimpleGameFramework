using UnityEngine;
using UnityEngine.UI;

namespace Game.MessageSystem.Test
{
    public class TextDisplay : MonoBehaviour
    {
        public Text TxtDisplay;

        private void Awake()
        { 
            MessageManager.Instance.Register<int>(TestMessageDefine.OnScoreChange, OnScoreChange);
        }

        private void OnScoreChange(int score)
        {
            TxtDisplay.text = score.ToString();
        }
    }
}