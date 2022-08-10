using DynamicBox.Managers;
using System.Collections;
using UnityEngine;

public class MusicController : MonoBehaviour
{
  [Header("Parameters")]
  [SerializeField] private float themeSongFadeOutTime;

  private IEnumerator waitCoroutine;
  private GameObject soundManagerObject;

  [Header("Links")]
  [SerializeField] private AudioSource audioSource;

  public void SetAudioSourceClip(AudioClip clip)
  {
    audioSource.clip = clip;
  }

  public void SetAudioSourceVolume(float volume)
  {
    audioSource.volume = volume;
  }

  public void SetSoundManager(GameObject soundManager)
  {
    soundManagerObject = soundManager;
  }

  public void PlayThemeSong()
  {
    audioSource.Play();
    waitCoroutine = WaitForClipLenght();
    StartCoroutine(waitCoroutine);
  }

  public bool CheckAudioSourceClip()
  {
    if (audioSource.clip != null)
    {
      return false;
    }
    else
    {
      return true;
    }
  }

  private IEnumerator WaitForClipLenght()
  {
    yield return new WaitForSeconds(audioSource.clip.length-themeSongFadeOutTime);
    StartCoroutine(FadeOutAndStop());
  }

  public void StopThemeSong()
  {
    StopCoroutine(waitCoroutine);
    StartCoroutine(FadeOutAndStop());
  }

  private IEnumerator FadeOutAndStop()
  {
    float startVolume = audioSource.volume;

    while (audioSource.volume > 0)
    {
      audioSource.volume -= startVolume * Time.deltaTime / themeSongFadeOutTime;

      yield return null;
    }

    audioSource.Stop();
    audioSource.clip = null; 
    audioSource.volume = startVolume;
    OnMusicOver();
  }

  private void OnMusicOver()
  {
    soundManagerObject.GetComponent<SoundManager>().MusicOver();
  }
}
