namespace Uber.Utils
{
    public enum TenderStatue
    {
        WaitingForPassenger = 1, // Tender is waiting for passenger to accept or reject
        Rejected, // Tender has been rejected by the passenger
        WaitingForDriverConfirimation, // Tender is waiting for the driver to accept or reject
        Cancelled, // Tender has been cancelled by the driver 
        driverGotAnotherTrip, // Driver has accepted another trip, so the tender is no longer valid,
        DriverConfirm, // Tender has been accepted by the driver

    }
}
