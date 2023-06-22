using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
        public TimeSpan TimeTaken;
        [HideInInspector] public bool Finished;
        [SerializeField] RectTransform clockHand;
        Image clockImage;
        Image clockHandImage;
        Transform clock;
        Vector2 clockPhase;
        Vector2 originalPosition;

        void Start()
        {
            ResetTime();
            clockImage = clockHand.parent.Find("Clock").GetComponent<Image>();
            clockHandImage = clockHand.GetComponent<Image>();
            clockPhase = new Vector2(
                Random.Range(-2 * Mathf.PI, 2 * Mathf.PI),
                Random.Range(-2 * Mathf.PI, 2 * Mathf.PI)
            );
            clock = clockHand.parent;
            originalPosition = clock.localPosition;
        }

        void Update()
        {
            if (counting)
                UpdateTime();
            else if (!Finished)
                ResetTime();
            UpdateText();
            if (TimeTaken >= TimeSpan.FromSeconds(TimeAvailable))
                TimeUp?.Invoke(this, EventArgs.Empty);
        }

        void UpdateTime() => TimeTaken = DateTime.Now - startTime;

        void UpdateText()
        {
            var timeLeft = (float)(TimeSpan.FromSeconds(TimeAvailable) - TimeTaken)
                .TotalSeconds;
            countUpText.text = TimeTaken.ToString(@"s\.ff");
            countDownText.text = TimeSpan
                .FromSeconds(Mathf.Round(timeLeft * 100) / 100)
                .ToString(@"s\.ff");

            countUpText.color
            = countDownText.color
            = clockImage.color
            = clockHandImage.color
            = timeLeft < 3
                && Mathf.Round(timeLeft / flashRate) % 2 == 0 ? Color.red : Color.white;

            clockHand.localEulerAngles = Vector3.forward * Mathf.LerpAngle(
                clockHand.localEulerAngles.z,
                timeLeft / TimeAvailable * 360,
                Time.deltaTime * 20
            );

            var progress = Mathf.Pow((float)TimeTaken.TotalMilliseconds / (TimeAvailable * 1000), 7);

            clock.localPosition = new Vector2(
                Mathf.PerlinNoise1D(Time.time * progress * 8 + clockPhase.x) * 30,
                Mathf.PerlinNoise1D(Time.time * progress * 8 + clockPhase.y) * 30
            ) * progress + originalPosition;
        }

        public void ResetTime()
        {
            startTime = DateTime.Now;
            UpdateTime();
        }
        public void StartCounting() => counting = true;
        public void StopCounting() => counting = false;
    }
}
