using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TNSR
{
    public class Crossfade : MonoBehaviour
    {
        Image image;
        void Start()
        {
            image = GetComponent<Image>();
            image.color = Color.black;
            StartCoroutine(FadeIn());
        }

        IEnumerator FadeIn()
        {
            for (float i = 1; i >= 0; i -= 0.01f)
            {
                Color colour = image.color;
                colour.a = i;
                image.color = colour;
                yield return new WaitForSeconds(0.01f);
            }
        }

        public void FadeOut(Action callback)
        {
            IEnumerator coroutine()
            {
                for (float i = 0; i <= 1; i += 0.01f)
                {
                    Color colour = image.color;
                    colour.a = i;
                    image.color = colour;
                    yield return new WaitForSeconds(0.01f);
                }
                callback();
            }
            StartCoroutine(coroutine());
        }
    }
}
