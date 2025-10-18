using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField] private AudioSource _closeAdSource;

    public void PlayCloseAdSFX() {
        _closeAdSource.pitch = Random.Range(0.95f, 1.075f);
        _closeAdSource.PlayOneShot(_closeAdSource.clip);
    }

}
