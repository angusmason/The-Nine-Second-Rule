using System;
using System.Collections;
using TMPro;
using TNSR.Levels;
using UnityEditor;
using UnityEngine;

namespace TNSR
{
    public class NewBest : MonoBehaviour
    {
        ParticleSystem system;
        TextMeshProUGUI text;
        ParticleSystem.EmissionModule module;
        bool showing;
        Countdown countdown;
        public event DoneEventHandler OnDone;
        public delegate void DoneEventHandler(object sender, EventArgs args);
        bool keeping;
        void Start()
        {
            system = GetComponent<ParticleSystem>();
            text = GetComponentInChildren<TextMeshProUGUI>();
            text.gameObject.SetActive(true);
            text.transform.localScale = Vector3.zero;
            module = system.emission;
            module.enabled = false;
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
            text.text = $"NEW BEST\nOF {countdown.Time:s'.'ff}!";
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

        public void Show() => showing = true;

        [MenuItem("TNSR/Modify Level One Time")]
        static void ModifyLevelOneTime() => LevelSaver.UpdateData(new LevelDatum(0, TimeSpan.FromSeconds(9)), true);
    }
}
