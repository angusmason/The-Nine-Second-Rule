using System;
using TMPro;
using UnityEngine;

namespace TNSR
{
    public class Countdown : MonoBehaviour
    {
        const int TimeAvailable = 9;
        const float flashRate = 0.2f;
        DateTime startTime;
        [SerializeField] TextMeshProUGUI countUpText;
        [SerializeField] TextMeshProUGUI countDownText;
        public event EventHandler TimeUp;
        bool counting;
        public TimeSpan Time { get; set; }

        void Start()
        {
            ResetTime();
        }

        void Update()
        {
            if (counting)
                UpdateTime();
            UpdateText();
            if (Time >= TimeSpan.FromSeconds(TimeAvailable))
            {
                TimeUp?.Invoke(this, EventArgs.Empty);
            }
        }

        void UpdateTime() => Time = DateTime.Now - startTime;

        void UpdateText()
        {
            var timeLeft = (float)(TimeSpan.FromSeconds(TimeAvailable) - Time)
                .TotalSeconds;
            countUpText.text = Time.ToString(@"s\.ff");
            countDownText.text = TimeSpan
                .FromSeconds(Mathf.Round(timeLeft * 100) / 100)
                .ToString(@"s\.ff");

            countDownText.color = timeLeft < 3 && Mathf.Round(timeLeft / flashRate) % 2 == 0  ? Color.red : Color.white;
        }

        public void ResetTime()
        {
            startTime = DateTime.Now;
            UpdateTime();
        }
        public void StartCounting()
        {
            counting = true;
            UpdateText();
        }
        public void StopCounting()
        {
            counting = false;
            UpdateText();
        }
    }
}
