using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DzenPhraseDisplay : MonoBehaviour {
    
    [Header("UI")]
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private TMP_Text _phraseLabel;

    private readonly float _moveDuration = 2f;
    private readonly float _fadeDuration = 1f;
    private readonly Vector2 _randomOffsetRange = new(100f, 50f);

    private readonly List<(float min, float max, string[] phrases)> _dzenPhrases = new() {
        (0f, 10f, new[] {
            "Breathe… everything is still.",
            "Soft whispers of peace fill your mind.",
            "Focus flows like a slow river."
        }),
        (10f, 20f, new[] {
            "A gentle calm surrounds you.",
            "Your thoughts drift slowly…",
            "Everything feels balanced and quiet."
        }),
        (20f, 30f, new[] {
            "Tiny ripples disturb your meditation.",
            "Mind wandering… stay present.",
            "Keep your focus steady."
        }),
        (30f, 40f, new[] {
            "Small disturbances creep into your calm.",
            "Notice the thoughts, then let them go.",
            "A gentle tension begins to stir."
        }),
        (40f, 50f, new[] {
            "The world is pulling at your attention.",
            "Concentrate! Tiny disturbances are growing.",
            "Your calm is being tested…"
        }),
        (50f, 60f, new[] {
            "Focus slips for a moment… regain it.",
            "Chaos tugs lightly at your mind.",
            "A storm of small thoughts swirls around you."
        }),
        (60f, 70f, new[] {
            "Distractions are everywhere… hold on!",
            "The storm of thoughts rages inside your mind.",
            "Tension spikes, your focus wavers!"
        }),
        (70f, 80f, new[] {
            "Mind feels stretched… resist the pull.",
            "Attention fragments into pieces… gather it.",
            "You are challenged to maintain clarity!"
        }),
        (80f, 90f, new[] {
            "Chaos meets clarity – can you hold it?",
            "Your mind dances between focus and distraction!",
            "Extreme distraction, but your zen is strong."
        }),
        (90f, 100f, new[] {
            "The world screams… yet your focus persists.",
            "Thoughts collide in a dazzling storm.",
            "Your inner calm fights the surrounding chaos."
        })
    };
    
    public void Init() {
        Game.Instance.DzenManager.DzenAmount.OnUpdate += OnDzenChange;
    }

    private void OnDisable() {
        Game.Instance.DzenManager.DzenAmount.OnUpdate -= OnDzenChange;
    }
    
    private void ShowPhrase() {
        _phraseLabel.gameObject.SetActive(true);
        
        string phrase = GetPhraseForDzen(Game.Instance.DzenManager.DzenAmount.Value());
        _phraseLabel.SetText(phrase);
        _phraseLabel.alpha = 0f;

        // Set random starting position
        Vector2 randomPos = new Vector2(
            Random.Range(-_canvasRect.rect.width / 3 + _randomOffsetRange.x, _canvasRect.rect.width / 3 - _randomOffsetRange.x),
            Random.Range(-_canvasRect.rect.height / 3 + _randomOffsetRange.y, _canvasRect.rect.height / 3 - _randomOffsetRange.y)
        );
        _phraseLabel.rectTransform.anchoredPosition = randomPos;

        Sequence seq = DOTween.Sequence();
        seq.Append(_phraseLabel.DOFade(1f, _fadeDuration).SetEase(Ease.InOutSine));
        seq.Append(_phraseLabel.rectTransform.DOAnchorPos(randomPos + new Vector2(Random.Range(-25f,25f), Random.Range(-25f,25f)), _moveDuration).SetEase(Ease.OutQuad));
        seq.Append(_phraseLabel.DOFade(0f, _fadeDuration).SetEase(Ease.InOutSine));
        seq.OnComplete(() => _phraseLabel.gameObject.SetActive(false));
    }

    private string GetPhraseForDzen(float dzen) {
        foreach (var entry in _dzenPhrases) {
            if (dzen >= entry.min && dzen < entry.max)
                return entry.phrases[Random.Range(0, entry.phrases.Length)];
        }
        
        return "";
    }
    
    private void OnDzenChange(int arg1, int arg2) {
        if (Random.value <= .35f)
            ShowPhrase();
    }
}
