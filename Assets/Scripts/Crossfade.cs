using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TNSR
{
    public class Crossfade : MonoBehaviour
    {
        Image image;
        public float Alpha;
        public bool Fading;
        void Start()
        {
            image = GetComponent<Image>();
            image.color = Color.black;
            StartCoroutine(FadeOut());
        }

        IEnumerator FadeOut()
        {
            Fading = true;
            for (float alpha = 1; alpha >= 0; alpha -= 0.01f)
            {
                var colour = image.color;
                colour.a = alpha;
                image.color = colour;
                Alpha = alpha;
                yield return new WaitForSeconds(0.01f);
            }
            Fading = false;
        }

        public void FadeIn(Action endCallback, Action<float> alphaCallback = null)
        {
            IEnumerator Coroutine()
            {
                Fading = true;
                for (float alpha = 0; alpha <= 1; alpha += 0.01f)
                {
                    var colour = image.color;
                    colour.a = alpha;
                    image.color = colour;
                    Alpha = alpha;
                    alphaCallback?.Invoke(alpha);
                    yield return new WaitForSeconds(0.01f);
                }
                endCallback();
                Fading = false;
            }
            StartCoroutine(Coroutine());
        }
    }
}
