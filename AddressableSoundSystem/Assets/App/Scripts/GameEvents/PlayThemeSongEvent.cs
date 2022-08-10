using DynamicBox.EventManagement;

namespace DynamicBox.GameEvents
{
  public class PlayThemeSongEvent : GameEvent
  {
    public readonly int ThemeSongIndex;

    public PlayThemeSongEvent(int themeSongIndex)
    {
      ThemeSongIndex = themeSongIndex;
    }
  }
}