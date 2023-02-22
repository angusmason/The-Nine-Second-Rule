using System;
using TMPro;
using UnityEngine;

namespace TNSR
{
    public class Countdown : MonoBehaviour
    {
        const int TimeAvailable = 9;
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
            countUpText.text = Time.ToString(@"s\.ff");
            countDownText.text = TimeSpan
                .FromSeconds(
                    Mathf.Round(
                        (float)(TimeSpan.FromSeconds(TimeAvailable) - Time)
                            .TotalSeconds
                    * 100) / 100
                )
                .ToString(@"s\.ff");
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
