using DynamicBox.EventManagement;

namespace DynamicBox.GameEvents
{
  public class PlaySoundEffectEvent : GameEvent
  {
    public readonly int SoundEffectIndex;

    public PlaySoundEffectEvent(int soundEffectIndex)
    {
      SoundEffectIndex = soundEffectIndex;
    }
  }
}

