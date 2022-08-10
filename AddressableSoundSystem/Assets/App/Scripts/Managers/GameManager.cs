using UnityEngine;
using DynamicBox.EventManagement;
using DynamicBox.GameEvents;
using System.Collections.Generic;

namespace DynamicBox.Managers
{
  public class GameManager : MonoBehaviour
  {
    [Header("Parameters")]
    [SerializeField] private int soundIndex;
    [SerializeField] private int themeSongIndex;
    [SerializeField] private float themeSongVolume;
    [SerializeField] private float soundEffectVolume;
    [SerializeField] private List<int> themeSongIndexes;

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Q))
      {
        EventManager.Instance.Raise(new PlaySoundEffectEvent(0));
      }
      if (Input.GetKeyDown(KeyCode.W))
      {
        EventManager.Instance.Raise(new PlaySoundEffectEvent(1));
      }
      if (Input.GetKeyDown(KeyCode.E))
      {
        EventManager.Instance.Raise(new PlaySoundEffectEvent(2));
      }
      if (Input.GetKeyDown(KeyCode.R))
      {
        EventManager.Instance.Raise(new PlaySoundEffectEvent(3));
      }
      if (Input.GetKeyDown(KeyCode.T))
      {
        EventManager.Instance.Raise(new PlaySoundEffectEvent(4));
      }
    }

    public void PlayThemeSong()
    {
      EventManager.Instance.Raise(new PlayThemeSongEvent(themeSongIndex));
    }

    public void PlaySoundEffect()
    {
      EventManager.Instance.Raise(new PlaySoundEffectEvent(soundIndex));
    }

    public void StopThemeSong()
    {
      EventManager.Instance.Raise(new StopThemeSongEvent());
    }

    public void LoopBetween()
    {
      EventManager.Instance.Raise(new LoopBetweenSongsEvent(themeSongIndexes));
    }

    public void SetThemeSongVolume()
    {
      EventManager.Instance.Raise(new SetThemeSongVolumeEvent(themeSongVolume));
    }

    public void SetSoundEffectVolume()
    {
      EventManager.Instance.Raise(new SetSoundEffectVolumeEvent(soundEffectVolume));
    }
  }
}

