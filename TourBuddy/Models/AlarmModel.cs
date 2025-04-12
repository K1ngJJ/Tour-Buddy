    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace TourBuddy.Models
    {
        public class AlarmModel
        {
            [PrimaryKey, AutoIncrement]  // Marking Id as the primary key and setting it to auto-increment
            public int Id { get; set; }

            public TimeSpan AlarmTime { get; set; }

            public string AlarmLabel { get; set; }

            public string SelectedSound { get; set; }

            public bool IsEnabled { get; set; }

            public int NotificationId { get; set; }

            public DateTime NotifyTime { get; set; }

        }
    }
