using System;

namespace AsanaNet.Objects
{
    [Serializable]
    public class AsanaScheduleDates : AsanaObject
    {
        [AsanaDataAttribute("due_on")] //
        public DateTime DueOn { get; set; }

        [AsanaDataAttribute("should_skip_weekends")] //
        public bool SkipWeekends { get; set; }
    }
}
