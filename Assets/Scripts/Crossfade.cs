using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TNSR
{
    public class Crossfade : MonoBehaviour
    {
        Image image;
        public float Alpha
        {
            get => image.color.a;
            set => image.color = new Color(image.color.r, image.color.g, image.color.b, value);
        }
        public Fading FadingState;
        const float FadeSpeedSeconds = 1f;
        const float FadeStep = 0.01f;

        public enum Fading { NotFading, FadingOut, FadingIn }
        void Start()
        {
            image = GetComponent<Image>();
            image.color = Color.black;
            FadeOut();
        }

        public void FadeOut(Action endCallback = null, Action<float> stepCallback = null)
        {
            IEnumerator FadeOutCoroutine()
            {
                FadingState = Fading.FadingOut;
                while (Alpha >= 0)
                {
                    if (FadingState == Fading.FadingIn)
                        yield break;
                    Alpha -= FadeStep;
                    stepCallback?.Invoke(Alpha);
                    yield return new WaitForSeconds(FadeSpeedSeconds * FadeStep);
                }
                endCallback?.Invoke();
                FadingState = Fading.NotFading;
            }
            StartCoroutine(FadeOutCoroutine());
        }

        public void FadeIn(Action endCallback = null, Action<float> stepCallback = null)
        {
            IEnumerator FadeInCoroutine()
            {
                FadingState = Fading.FadingIn;
                while (Alpha <= 1)
                {
                    if (FadingState == Fading.FadingOut)
                        yield break;
                    Alpha += FadeStep;
                    stepCallback?.Invoke(Alpha);
                    yield return new WaitForSeconds(FadeSpeedSeconds * FadeStep);
                }
                endCallback?.Invoke();
                FadingState = Fading.NotFading;
            }
            StartCoroutine(FadeInCoroutine());
        }
    }
}
