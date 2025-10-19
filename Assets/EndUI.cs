using System.Collections;
using TMPro;
using UnityEngine;

public class EndUI : MonoBehaviour {

    [SerializeField] private TMP_Text _stateLabel;
    [SerializeField] private TMP_Text _descriptionLabel;
    [SerializeField] private TMP_Text _thxLabel;

    private string _winCitate = "Through focus and patience, you’ve mastered the art of calm, letting serenity flow through every thought.";
    private string _loseCitate = "Despite your effort, the constant noise shattered your calm—your Zen is broken!";
    
    public void SetState(bool win) {
        _stateLabel.SetText(win ? "YOU WIN!" : "YOU LOSE!");

        StartCoroutine(TypeText(_descriptionLabel, win ? _winCitate : _loseCitate));
    }
    
    private IEnumerator TypeText(TMP_Text label, string text) {
        Game.Instance.AudioManager.SetKeyboardSFX(true);
        
        label.text = "";
        foreach (char c in text) {
            label.text += c;
            yield return new WaitForSecondsRealtime(.005f);
        }
        
        Game.Instance.AudioManager.SetKeyboardSFX(false);

        yield return new WaitForSeconds(3f);
        
        _stateLabel.gameObject.SetActive(false);
        _descriptionLabel.gameObject.SetActive(false);
        _thxLabel.gameObject.SetActive(true);
    }
}
