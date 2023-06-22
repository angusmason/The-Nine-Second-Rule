using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace TNSR
{
    public class EndText : MonoBehaviour
    {
        TextMeshProUGUI text;
        bool showing;
        Countdown countdown;
        public event EventHandler OnDone;
        bool keeping;
        void Start()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
            text.gameObject.SetActive(true);
            text.transform.localScale = Vector3.zero;
            countdown = FindFirstObjectByType<Countdown>();
            keeping = false;
        }

        void Update()
        {
            text.transform.localScale = Vector3.one * Mathf.Lerp(
                text.transform.localScale.x,
                showing ? 1 : 0,
                Time.deltaTime * 10
            );
            if (!showing)
                return;
            if (text.transform.localScale.x < 0.99f)
                return;
            text.transform.localScale = Vector3.one;
            if (keeping)
                return;
            StartCoroutine(KeepOnScreen());
            IEnumerator KeepOnScreen()
            {
                keeping = true;
                yield return new WaitForSeconds(2);
                showing = false;
                yield return new WaitUntil(() => text.transform.localScale.x <= 0.01f);
                OnDone?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Show(string text)
        {
            this.text.text = text;
            showing = true;
        }
    }
}
