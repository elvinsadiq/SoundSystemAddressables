using DynamicBox.EventManagement;

namespace DynamicBox.GameEvents
{
  public class SetThemeSongVolumeEvent : GameEvent
  {
    public readonly float ThemeSongVolume;

    public SetThemeSongVolumeEvent(float themeSongVolume)
    {
      ThemeSongVolume = themeSongVolume;
    }
  }
}
