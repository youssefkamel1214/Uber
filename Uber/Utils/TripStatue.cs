namespace Uber.Utils
{
    public enum TripStatue
    {
        DriverWaiting = 1,// Waiting for a driver to accept the trip
        DriverAccepted,// Driver has accepted the trip
        TripBegin,// Trip has started , after picup
        TripCompleted,// Trip has been completed
        TripCancelled,// Trip has been cancelled by either the driver or the rider
    }
}
