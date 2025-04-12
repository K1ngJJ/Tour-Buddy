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
            var databasePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "alarms.db3");
            _database = new SQLiteConnection(databasePath);
            _database.CreateTable<AlarmModel>();
        }

        public async Task<List<AlarmModel>> LoadAllAsync()
        {
            return await Task.FromResult(_database.Table<AlarmModel>().ToList());
        }

        public async Task SaveAsync(AlarmModel alarm)
        {
            if (alarm.Id == 0)
            {
                _database.Insert(alarm); // SQLite will auto-assign ID if AlarmModel uses [PrimaryKey, AutoIncrement]
            }
            else
            {
                _database.Update(alarm);
            }

            await Task.CompletedTask;
        }

        public async Task DeleteAsync(AlarmModel alarm)
        {
            _database.Delete<AlarmModel>(alarm.Id);
            await Task.CompletedTask;
        }
    }
}
