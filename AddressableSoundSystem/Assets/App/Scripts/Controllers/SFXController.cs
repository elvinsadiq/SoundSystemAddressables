using System.Collections;
using UnityEngine;
using DynamicBox.Managers;

namespace DynamicBox.Controllers
{
  public class SFXController : MonoBehaviour
  {
    [Header("Parameters")]
    [SerializeField] private float releaseSoundEffectDelay;

    private int soundEffectIndex;
    private bool canReleaseSoundEffect;
    private bool isPlay = false;
    private GameObject soundManagerObject;

    [Header("Links")]
    [SerializeField] private AudioSource audioSource;

    public void SetAudioSourceClip(AudioClip soundEffect, int index)
    {
      audioSource.clip = soundEffect;
      soundEffectIndex = index;
    }

    public void SetAudioSourceVolume(float volume)
    {
      audioSource.volume = volume;
    }

    public void SetSoundManager(GameObject soundManager)
    {
      soundManagerObject = soundManager;
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

    public void PlaySound()
    {
      AudioClip clip = audioSource.clip;
      audioSource.PlayOneShot(clip);
      canReleaseSoundEffect = false;

      if (!isPlay)
      {
        StartCoroutine(ReleaseSoundEffect());
      }

      isPlay = true;
    }

    private IEnumerator ReleaseSoundEffect()
    {
      yield return new WaitForSeconds(releaseSoundEffectDelay);
      if (canReleaseSoundEffect)
      {
        audioSource.clip = null;
        soundManagerObject.GetComponent<SoundManager>().ReleaseSoundEffectAsset(soundEffectIndex);
        isPlay = false;
        gameObject.SetActive(false);
      }
      else
      {
        StartCoroutine(ReleaseSoundEffect());
        canReleaseSoundEffect = true;
      }
    }
  }
}

