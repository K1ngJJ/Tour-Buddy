using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourBuddy.Models
{
    public class AlarmModel
    {
        // Alarm Time (e.g., the time when the alarm should trigger)
        public TimeSpan AlarmTime { get; set; }

        // Label for the alarm (e.g., "Wake up", "Meeting", etc.)
        public string AlarmLabel { get; set; }

        // Selected Sound (e.g., "Beep", "Chime", "Ring", etc.)
        public string SelectedSound { get; set; }

        // Volume for the alarm sound (e.g., 0-100 range)
        public double Volume { get; set; }

        // Flag to enable or disable the alarm
        public bool IsEnabled { get; set; }

        // Unique ID for the alarm (could be used for notifications)
        public int NotificationId { get; set; }
    }
}
