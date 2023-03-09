using System;
using System.Collections;
using TMPro;
using TNSR;
using UnityEngine;

public class NewBest : MonoBehaviour
{
    ParticleSystem system;
    TextMeshProUGUI text;
    ParticleSystem.EmissionModule module;
    public bool Showing;
    Countdown countdown;
    public event DoneEventHandler OnDone;
    public delegate void DoneEventHandler(object sender, EventArgs args);
    void Start()
    {
        system = GetComponent<ParticleSystem>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.gameObject.SetActive(true);
        text.transform.localScale = Vector3.zero;
        module = system.emission;
        module.enabled = false;
        countdown = FindFirstObjectByType<Countdown>();
    }

    void Update()
    {
        if (!Showing)
            return;
        text.transform.localScale = Vector3.one * Mathf.Lerp(
            text.transform.localScale.x,
            1,
            Time.deltaTime * 10
        );
        text.text =$"NEW BEST\nOF {countdown.Time:s'.'ff}!";
        if (1 - text.transform.localScale.x >= 0.01f)
            return;
        text.transform.localScale = Vector3.one;
        IEnumerator KeepOnScreen()
        {
            yield return new WaitForSeconds(3);
            OnDone?.Invoke(this, EventArgs.Empty);
        }
        StartCoroutine(KeepOnScreen());
    }
}
