using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using DynamicBox.EventManagement;
using DynamicBox.GameEvents;
using DynamicBox.Controllers;
using System.Collections;

namespace DynamicBox.Managers
{
  //Creating this class because Audio clip doesn't have default class
  [Serializable]
  public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
  {
    public AssetReferenceAudioClip(string guid) : base(guid) { }
  }

  public class SoundManager : MonoBehaviour
  {
    [Header("Parameters")]
    [SerializeField] private int defaultPoolSize;
    [SerializeField] private float clearPoolDelay;
    [SerializeField] private List<AssetReferenceAudioClip> soundEffectReferences;
    [SerializeField] private List<AssetReferenceAudioClip> themeSongReferences;
    [SerializeField] private List<int> loopSongsList; // Make this variable private

    private int previousThemeSongIndex;
    private int releaseThemeSongIndex;
    private int currentLoopIndex;
    private float currentSoundVolume = 1f; // Take this value from SaveManager
    private bool stopThemeSong;
    private bool playThemeSong;
    private bool loopBetween;
    private Dictionary<int, GameObject> loadedSFXAssetsDictionary = new Dictionary<int, GameObject>();

    [Header("Links")]
    [SerializeField] private GameObject musicController;
    [SerializeField] private GameObject sfxControllerPrefab;
    [SerializeField] private GameObject soundPool;

    #region Unity Methods

    void OnEnable()
    {
      EventManager.Instance.AddListener<PlaySoundEffectEvent>(PlaySoundEffectHandler);
      EventManager.Instance.AddListener<PlayThemeSongEvent>(PlayThemeSongHandler);
      EventManager.Instance.AddListener<StopThemeSongEvent>(StopThemeSongHandler);
      EventManager.Instance.AddListener<LoopBetweenSongsEvent>(LoopBetweenSongsHandler);
      EventManager.Instance.AddListener<SetThemeSongVolumeEvent>(SetThemeSongVolumeHandler);
      EventManager.Instance.AddListener<SetSoundEffectVolumeEvent>(SetSoundEffectVolumeHandler);
    }

    void OnDisable()
    {
      EventManager.Instance.RemoveListener<PlaySoundEffectEvent>(PlaySoundEffectHandler);
      EventManager.Instance.RemoveListener<PlayThemeSongEvent>(PlayThemeSongHandler);
      EventManager.Instance.RemoveListener<StopThemeSongEvent>(StopThemeSongHandler);
      EventManager.Instance.RemoveListener<LoopBetweenSongsEvent>(LoopBetweenSongsHandler);
      EventManager.Instance.RemoveListener<SetThemeSongVolumeEvent>(SetThemeSongVolumeHandler);
      EventManager.Instance.RemoveListener<SetSoundEffectVolumeEvent>(SetSoundEffectVolumeHandler);
    }

    void Start()
    {
      Addressables.InitializeAsync();

      GameObject newPool = Instantiate(soundPool);
      soundPool = newPool;

      GameObject newMusicController = Instantiate(musicController);
      newMusicController.GetComponent<MusicController>().SetSoundManager(gameObject);
      musicController = newMusicController;

      for (int i = 0; i < defaultPoolSize; i++)
      {
        GameObject newSfxController = Instantiate(sfxControllerPrefab, soundPool.transform);
        newSfxController.GetComponent<SFXController>().SetSoundManager(gameObject);
        newSfxController.SetActive(false);
      }
    }

    #endregion

    private void PlaySoundEffect(int soundEffectIndex)
    {
      try
      {
        soundEffectReferences[soundEffectIndex].LoadAssetAsync<AudioClip>().Completed += (clip) =>
        {
          bool found = false;
          for (int i = 0; i < soundPool.transform.childCount; i++)
          {
            //If Empty SFXController is found in the pool.
            if (soundPool.transform.GetChild(i).GetComponent<SFXController>().CheckAudioSourceClip())
            {
              //Take first available child
              soundPool.transform.GetChild(i).gameObject.SetActive(true);
              soundPool.transform.GetChild(i).GetComponent<SFXController>().SetAudioSourceClip(clip.Result, soundEffectIndex);
              loadedSFXAssetsDictionary.Add(soundEffectIndex, soundPool.transform.GetChild(i).gameObject);
              soundPool.transform.GetChild(i).GetComponent<SFXController>().PlaySound();
              found = true;
              break;
            }
          }

          //If Empty SFXController is not found in the pool
          if (!found)
          {
            GameObject newSfxController = Instantiate(sfxControllerPrefab, soundPool.transform);
            newSfxController.GetComponent<SFXController>().SetAudioSourceClip(clip.Result, soundEffectIndex);
            newSfxController.GetComponent<SFXController>().SetSoundManager(gameObject);
            newSfxController.GetComponent<SFXController>().SetAudioSourceVolume(currentSoundVolume);
            newSfxController.GetComponent<SFXController>().PlaySound();
            loadedSFXAssetsDictionary.Add(soundEffectIndex, newSfxController);
            StartCoroutine(ClearSoundPool());
          }
        };
      }
      catch
      {
        Debug.Log("You are trying to load asset which is already loaded");
      }
    }

    private void PlayThemeSong(int themeSongIndex)
    {
      themeSongReferences[themeSongIndex].LoadAssetAsync<AudioClip>().Completed += (clip) =>
      {
        previousThemeSongIndex = themeSongIndex;
        musicController.GetComponent<MusicController>().SetAudioSourceClip(clip.Result);
        musicController.GetComponent<MusicController>().PlayThemeSong();
      };
    }

    private void ReleaseAudioClipAsset(AssetReferenceAudioClip asset)
    {
      asset.ReleaseAsset();
    }
    private void LoopBetweenThemeSongs()
    {
      if (currentLoopIndex + 1 <= loopSongsList.Count - 1)
      {
        currentLoopIndex += 1;
        previousThemeSongIndex = loopSongsList[currentLoopIndex];
        PlayThemeSong(loopSongsList[currentLoopIndex]);
      }
      else
      {
        currentLoopIndex = 0;
        previousThemeSongIndex = loopSongsList[currentLoopIndex];
        PlayThemeSong(loopSongsList[currentLoopIndex]);
      }
    }

    public void MusicOver()
    {
      if (stopThemeSong)
      {
        ReleaseAudioClipAsset(themeSongReferences[previousThemeSongIndex]);
      }
      else if (loopBetween)
      {
        ReleaseAudioClipAsset(themeSongReferences[previousThemeSongIndex]);
        LoopBetweenThemeSongs();
      }
      else if (playThemeSong)
      {
        ReleaseAudioClipAsset(themeSongReferences[releaseThemeSongIndex]);
        releaseThemeSongIndex = previousThemeSongIndex;
        PlayThemeSong(previousThemeSongIndex);
      }
    }

    public void ReleaseSoundEffectAsset(int soundIndex)
    {
      loadedSFXAssetsDictionary.Remove(soundIndex);
      ReleaseAudioClipAsset(soundEffectReferences[soundIndex]);
    }

    private IEnumerator ClearSoundPool()
    {
      yield return new WaitForSeconds(clearPoolDelay);

      if (soundPool.transform.childCount <= defaultPoolSize) { yield break; }

      bool destroyed = false;
      //Iterate BackWards
      for (int i = soundPool.transform.childCount - 1; i >= 0; i--)
      {
        if (soundPool.transform.GetChild(i).GetComponent<SFXController>().CheckAudioSourceClip())
        {
          Destroy(soundPool.transform.GetChild(i).gameObject);
          destroyed = true;
          StartCoroutine(ClearSoundPool());
          break;
        }
      }

      if (!destroyed)
      {
        StartCoroutine(ClearSoundPool());
      }
    }

    #region Event Handlers

    private void PlaySoundEffectHandler(PlaySoundEffectEvent eventDetails)
    {
      if (loadedSFXAssetsDictionary.TryGetValue(eventDetails.SoundEffectIndex, out GameObject controller))
      {
        controller.GetComponent<SFXController>().PlaySound();
      }
      else
      {
        PlaySoundEffect(eventDetails.SoundEffectIndex);
      }
    }

    private void PlayThemeSongHandler(PlayThemeSongEvent eventDetails)
    {
      playThemeSong = true;
      stopThemeSong = false;
      loopBetween = false;

      if (!musicController.GetComponent<MusicController>().CheckAudioSourceClip())
      {
        musicController.GetComponent<MusicController>().StopThemeSong();
        previousThemeSongIndex = eventDetails.ThemeSongIndex;
      }
      else
      {
        releaseThemeSongIndex = eventDetails.ThemeSongIndex;
        PlayThemeSong(eventDetails.ThemeSongIndex);
      }
    }

    private void StopThemeSongHandler(StopThemeSongEvent eventDetails)
    {
      stopThemeSong = true;
      playThemeSong = false;
      loopBetween = false;
      if (!musicController.GetComponent<MusicController>().CheckAudioSourceClip())
      {
        musicController.GetComponent<MusicController>().StopThemeSong();
      }
    }
    private void LoopBetweenSongsHandler(LoopBetweenSongsEvent eventDetails)
    {
      loopSongsList = eventDetails.ThemeSongIndexes;
      loopBetween = true;
      stopThemeSong = false;
      playThemeSong = false;
      if (!musicController.GetComponent<MusicController>().CheckAudioSourceClip())
      {
        currentLoopIndex = -1;
        musicController.GetComponent<MusicController>().StopThemeSong();
      }
      else
      {
        currentLoopIndex = 0;
        releaseThemeSongIndex = loopSongsList[currentLoopIndex];
        PlayThemeSong(loopSongsList[currentLoopIndex]);
      }
    }

    private void SetThemeSongVolumeHandler(SetThemeSongVolumeEvent eventDetails)
    {
      musicController.GetComponent<MusicController>().SetAudioSourceVolume(eventDetails.ThemeSongVolume);
    }

    private void SetSoundEffectVolumeHandler(SetSoundEffectVolumeEvent eventDetails)
    {
      for (int i = 0; i < soundPool.transform.childCount; i++)
      {
        soundPool.transform.GetChild(i).GetComponent<SFXController>().SetAudioSourceVolume(eventDetails.SoundEffectVolume);
      }

      currentSoundVolume = eventDetails.SoundEffectVolume;
    }

    #endregion
  }
}