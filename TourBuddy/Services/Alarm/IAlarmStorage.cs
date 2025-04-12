using System.Collections.Generic;
using System.Threading.Tasks;
using TourBuddy.Models;

namespace TourBuddy.Services.Alarm
{
    public interface IAlarmStorage
    {
        Task<List<AlarmModel>> LoadAllAsync();
        Task SaveAsync(AlarmModel alarm);
        Task DeleteAsync(AlarmModel alarm);
    }
}
