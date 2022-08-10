using DynamicBox.EventManagement;

namespace DynamicBox.GameEvents
{
  public class SetSoundEffectVolumeEvent : GameEvent
  {
    public readonly float SoundEffectVolume;

    public SetSoundEffectVolumeEvent(float soundEffectVolume)
    {
      SoundEffectVolume = soundEffectVolume;
    }
  }
}

