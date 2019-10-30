using System;

namespace DFC.App.JobProfileSkills.MessageFunctionApp.Models
{
    public class SegmentClientOptions
    {
        public Uri BaseAddress { get; set; }

        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 10);         // default to 10 seconds
    }
}