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
        void Start()
        {
            image = GetComponent<Image>();
            image.color = Color.black;
            StartCoroutine(FadeOut());
        }

        IEnumerator FadeOut()
        {
            for (float alpha = 1; alpha >= 0; alpha -= 0.01f)
            {
                Color colour = image.color;
                colour.a = alpha;
                image.color = colour;
                Alpha = alpha;
                yield return new WaitForSeconds(0.01f);
            }
        }

        public void FadeIn(Action endCallback)
        {
            IEnumerator coroutine()
            {
                for (float alpha = 0; alpha <= 1; alpha += 0.01f)
                {
                    Color colour = image.color;
                    colour.a = alpha;
                    image.color = colour;
                    Alpha = alpha;
                    yield return new WaitForSeconds(0.01f);
                }
                endCallback();
            }
            StartCoroutine(coroutine());
        }
    }
}
