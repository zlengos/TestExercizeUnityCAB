using System;

namespace Tools
{
    public static class EventsManager
    {
        public static Action<int> OnHeartsUpdated;
        public static Action<int> OnSkillUpdated;
        public static int LastChangeValue = 0;
    }
}