using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField] private AudioSource _closeAdSource;
    [SerializeField] private AudioSource _dzenColllectSource;
    [SerializeField] private AudioSource _keyboardSource;

    public void PlayCloseAdSFX() {
        _closeAdSource.pitch = Random.Range(0.95f, 1.05f);
        _closeAdSource.PlayOneShot(_closeAdSource.clip);
    }
    
    public void PlayDzenCollctSFX() {
        _dzenColllectSource.pitch = Random.Range(0.995f, 1.025f);
        _dzenColllectSource.PlayOneShot(_dzenColllectSource.clip);
    }
    
    public void SetKeyboardSFX(bool state) {
        if (state)
            _keyboardSource.Play();
        else
            _keyboardSource.Stop();
    }
}
