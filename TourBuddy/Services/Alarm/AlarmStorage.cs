using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourBuddy.Models;

namespace TourBuddy.Services.Alarm
{
    public class AlarmStorage : IAlarmStorage
    {
        private readonly SQLiteConnection _database;

        public AlarmStorage()
        {
            // Get database path from local storage folder
            var databasePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "alarms.db3");
            _database = new SQLiteConnection(databasePath);

            // Create the table if it doesn't exist
            _database.CreateTable<AlarmModel>();
        }

        public async Task<List<AlarmModel>> LoadAllAsync()
        {
            return await Task.Run(() => _database.Table<AlarmModel>().ToList());
        }

        public async Task SaveAsync(AlarmModel alarm)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (alarm.Id == 0)
                    {
                        _database.Insert(alarm); // SQLite will auto-assign ID if AlarmModel uses [PrimaryKey, AutoIncrement]
                    }
                    else
                    {
                        _database.Update(alarm);
                    }
                });
            }
            catch (Exception ex)
            {
                // Log or handle any exceptions
                Console.WriteLine($"Error saving alarm: {ex.Message}");
            }
        }

        public async Task DeleteAsync(AlarmModel alarm)
        {
            try
            {
                await Task.Run(() => _database.Delete<AlarmModel>(alarm.Id));
            }
            catch (Exception ex)
            {
                // Log or handle any exceptions
                Console.WriteLine($"Error deleting alarm: {ex.Message}");
            }
        }
    }
}
