using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingPanel : MonoBehaviour
{
	public static FadingPanel Ins;
	[SerializeField] private CanvasGroup canvasGroup;
	private Tween fadeTween;
    void Awake()
    {
		Ins = this;
    }
    public void FadeIn(float duration)
	{
		Fade(1f, duration, () =>
		{
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		});
	}

	public void FadeOut(float duration)
	{
		Fade(0f, duration, () =>
		{
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		});
	}

	private void Fade(float endValue, float duration, TweenCallback onEnd)
	{
		if (fadeTween != null)
		{
			fadeTween.Kill(false);
		}

		fadeTween = canvasGroup.DOFade(endValue, duration);
		fadeTween.onComplete += onEnd;
	}

	public IEnumerator FadePanel()
	{
		FadeIn(0.75f);
		yield return new WaitForSeconds(0.75f);
		FadeOut(0.75f);
	}
}