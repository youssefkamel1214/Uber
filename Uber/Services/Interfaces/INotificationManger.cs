
namespace Uber.Services.Interfaces
{
    public interface INotificationManger
    {
        public Task notifyDriverChannel(string driverId, Dictionary<string, object> dataToSendToDriverChannel);
        public Task notifyDriversChannel(Dictionary<string, object> dict);
        public Task notifyTripChannel(string tripID, Dictionary<string, object> dict);
    }
}
