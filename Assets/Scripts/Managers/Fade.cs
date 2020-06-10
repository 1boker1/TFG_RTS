using UnityEngine;
using System;
using System.Collections;

public class Fade : MonoBehaviour
{
    public static Fade Instance;
    public bool Persistent;

    public bool IsPlaying;

    public Animation FadeAnimation;

    public AnimationClip FadeInClip;
    public AnimationClip FadeOutClip;

    public delegate void OnFadeEnd();
    public static event OnFadeEnd OnFadeEnded;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        if (Persistent)
            DontDestroyOnLoad(this.gameObject);
    }

    public void FadeIn()
    {
        StartCoroutine(StartFade(FadeInClip));
    }

    public void FadeOut()
    {
        StartCoroutine(StartFade(FadeOutClip));
    }

    public void AddEventOnEndFade(Action EventOnEndFade)
    {
        if (!IsPlaying)
            OnFadeEnded += new OnFadeEnd(EventOnEndFade);
    }

    private IEnumerator StartFade(AnimationClip clip)
    {
        if (IsPlaying)
           yield break;

        FadeAnimation.clip = clip;
        FadeAnimation.Rewind();
        FadeAnimation.Play();

        IsPlaying = true;

        yield return new WaitForSeconds(clip.length);

        IsPlaying = false;

        OnFadeEnded?.Invoke();
        RemoveAllListeners();
    }

    private void RemoveAllListeners()
    {
        if (OnFadeEnded == null)
            return;

        foreach (Delegate del in OnFadeEnded.GetInvocationList())
        {
            OnFadeEnded -= (OnFadeEnd)del;
        }
    }
}
