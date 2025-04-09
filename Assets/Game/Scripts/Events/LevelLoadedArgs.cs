using System;

namespace Garawell.Managers.Events
{
    public class LevelLoadedArgs : EventArgs
    {
        public int tutorialLevelCount;
        public int totalLevelCount;
        public bool shuffle;
        public bool sceneMode;
        public bool useManagerSceneLightSettings;

        public LevelLoadedArgs(int tutorialLevelCount, int totalLevelCount, bool shuffle, bool sceneMode, bool useManagerSceneLightSettings)
        {
            this.tutorialLevelCount = tutorialLevelCount;
            this.totalLevelCount = totalLevelCount;
            this.shuffle = shuffle;
            this.sceneMode = sceneMode;
            this.useManagerSceneLightSettings = useManagerSceneLightSettings;
        }
    }
}

