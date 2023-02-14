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
        public TimeSpan Time => DateTime.Now - startTime;

        void Start()
        {
            startTime = DateTime.Now;
            UpdateText();
        }

        void Update()
        {
            if (!counting)
            {
                ResetTime();
                return;
            }
            UpdateText();
            if (Time >= TimeSpan.FromSeconds(TimeAvailable))
            {
                TimeUp?.Invoke(this, EventArgs.Empty);
            }
        }

        void UpdateText()
        {
            countUpText.text = Time.ToString(@"ss\.ff");
            countDownText.text = TimeSpan
                .FromSeconds(
                    Mathf.RoundToInt(
                        (float)(TimeSpan.FromSeconds(TimeAvailable) - Time)
                            .TotalSeconds
                    )
                )
                .ToString(@"%s");
        }

        public void ResetTime() => startTime = DateTime.Now;
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
