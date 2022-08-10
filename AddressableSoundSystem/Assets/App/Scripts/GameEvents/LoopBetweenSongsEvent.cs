using DynamicBox.EventManagement;
using System.Collections.Generic;

namespace DynamicBox.GameEvents
{
  public class LoopBetweenSongsEvent : GameEvent
  {
    public readonly List<int> ThemeSongIndexes;

    public LoopBetweenSongsEvent(List<int> themeSongIndexes)
    {
      ThemeSongIndexes = themeSongIndexes;
    }
  }
}
