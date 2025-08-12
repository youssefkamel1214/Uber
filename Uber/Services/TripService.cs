using System.Threading.Tasks;
using Uber.Models.Domain;
using Uber.Models.Responses;
using Uber.Repositories.Interfaces;
using Uber.Services.Interfaces;
using Uber.Utils;
using Uber.WebSockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Uber.Services
{
    public class TripService : ITripService
    {
        private readonly ITenderRepository _tenderRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly webSocketManager _webSocketManager; // Inject the manager
        private readonly IDriverRepository _driverRepository;
        private readonly IPassengerRepository _passengerRepository;

        public TripService(ITenderRepository tenderRepository, ITripRepository tripRepository, IReviewRepository reviewRepository, 
            webSocketManager webSocketManager, IDriverRepository driverRepository, IPassengerRepository passengerRepository)
        {
            _tenderRepository = tenderRepository;
            _tripRepository = tripRepository;
            _reviewRepository = reviewRepository;
            _webSocketManager = webSocketManager;
            _driverRepository = driverRepository;
            _passengerRepository = passengerRepository;
        }

        public async Task<TripServiceResponse> acceptTenderOffer(Guid tenderId)
        {
            var existingTender=await _tenderRepository.GetTenderByIdAsync(tenderId);
            if(existingTender==null)
            {
                return new TripServiceResponse
                {
                    success = false,
                    Error = ["there not Tender with this driverID"]
                };
            }
            var existingtrip= await _tripRepository.getTripByIdAsync(existingTender.TripId);
            if (existingtrip == null)
            {
                throw new Exception($"Trip not Found for id :{existingTender.TripId}");
            }
            if (existingtrip.Status != TripStatue.DriverWaiting) 
            {
                return new TripServiceResponse
                {
                    success = false,
                    Error = [$"invalid Trip Statue :{existingtrip.Status}"]
                };
            }
            if (existingTender.staute == TenderStatue.WaitingForPassenger)
            {
                if (existingTender.staute==TenderStatue.Expired)
                {
                    return new TripServiceResponse
                    {
                        success = false,
                        Error = [$"Tender has expired at {existingTender.TenderTime.AddMinutes(5).ToLocalTime()}"]
                    };
                }
                var driver =await  _driverRepository.getDriverByIdAsync(existingTender.DriverId);
                if (!driver.IsActive ||! driver.isAvailable) 
                {
                    existingTender.staute= TenderStatue.driverGotAnotherTrip;
                    return new TripServiceResponse
                    {
                        success = false,
                        Error = [$"Driver Got another Trip"]
                    };
                }
                existingTender.staute=TenderStatue.WaitingForDriverConfirimation;
                await _tenderRepository.updateTender(existingTender);
                existingtrip.Status = TripStatue.WaitingForConifirmationOnTender;
                await _tripRepository.updatetripAsync(existingtrip);
                return new TripServiceResponse
                {
                    success=true,
                    Message=$"Waiting For Driver to Confirm Tender You Cant Accept Any Tender for 60 Socends until Confirmation Times is up"
                };
            }
            else 
            {
                return new TripServiceResponse
                {
                    success = false,
                    Error = [$"invalid Tender Statue :{existingTender.staute}"]
                };
            }
        }
        
        public async Task<TripServiceResponse> AddReviewforTrip(Review review)
        {
            var existingreviews = await _reviewRepository.GetReviewsOnTrip(review.TripId);
            var canmakeReview = !existingreviews.Any(r=>r.type==review.type);
            if (!canmakeReview)
            {
                return new TripServiceResponse {
                    success = false,
                    Error = ["user already made review on this trip" ]
                };
            }
            var addedReview = await _reviewRepository.addReview(review);
            return  new TripServiceResponse
            {
                success = true,
                Message =$"reveiw added Succesfully with Review ID {addedReview.ReviewId}"
            }; 
        }

        public async Task<TripServiceResponse> cancelTrip(Guid tripId, CancelledBy cancelledBy)
        {
           //var trip = await _tripRepository.getTripByIdAsync(tripId);
           // if (trip == null)
           // {
           //     throw new Exception($"Trip not found on id{tripId}");
           // }
           // var canceltime  = DateTime.UtcNow;
           // trip.Status = TripStatue.TripCancelled;
           // trip.EndTime = canceltime;
           // var cancellation = new Cancellation
           // {
           //     CancellationId = Guid.NewGuid(),
           //     TripId = tripId,
           //     cancelledBy = cancelledBy,
           //     CancellationTime = canceltime
           // };
           // canceltime=await _ca
           throw new NotImplementedException("Cancellation service is not implemented yet.");

        }

        public async Task<TripServiceResponse> createTenderOnTrip(Tender tender)
        {
            var existingtrip = await _tripRepository.getTripByIdAsync(tender.TripId);
            var driver=await _driverRepository.getDriverByIdAsync(tender.DriverId);
            if (existingtrip == null)
            {
                return new TripServiceResponse
                {
                    success = false,
                    Error = [$"Trip not found on id {tender.TripId}"]
                };
            }
            if(existingtrip.Status != TripStatue.DriverWaiting&& existingtrip.Status != TripStatue.WaitingForConifirmationOnTender)
            {
                return new TripServiceResponse
                {
                    success = false,
                    Error = [$"Trip is not in a valid state to create a tender"]
                };
            }
            if (!driver.isAvailable) 
            {
                return new TripServiceResponse
                {
                    success = false,
                    Error = [$"cant Add Tender While you in other Trip"]
                };
            }
            if (await _tenderRepository.isThereActiveDriverTenderForThisTrip(tender.TripId, tender.DriverId))
            {

                return new TripServiceResponse
                {
                    success = false,
                    Error = [$"There Already Active Tender for this Driver on this Trip"]
                };
            }
            if (tender.OfferedPrice < existingtrip.BasePrice)
            {
                return new TripServiceResponse
                {
                    success = false,
                    Error  = [$"Offered price cant be less than {existingtrip.BasePrice}"]
                };
            }

            var createdTender = await  _tenderRepository.AddTender(tender);
            //Create notification for Tender Lithener
            notify_Tender_listhners(createdTender);
            return new TripServiceResponse
            {
                success = true,
                Message=$"Tender Succesfully Added"
                
            };
        }

        private async Task notify_Tender_listhners(Tender tender)
        {
            var driver = await _driverRepository.getDriverByIdAsync(tender.DriverId);
            var TenderData = new TenderDataResponse
            {
                DriverName = driver.FirstName + " " + driver.LastName,
                DriverRating = driver.Rating,
                OfferedPrice = tender.OfferedPrice,
                ExpiryTime = tender.TenderTime.ToLocalTime().AddMinutes(5),
                DriverPhoneNumber = driver.PhoneNumber,
                licensePlate = driver.LicensePlate
            };
            var dict=new Dictionary<string, object>();
            dict.Add("type", "Driver and Tender Data");
            dict.Add("data", TenderData);
            _webSocketManager.BroadcastdataToTripChannel(tender.TripId.ToString(), dict);
        }
        private async Task notifyDriverwithnewConnection(Trip createdTrip)
        {
            var passenger= await _passengerRepository.getPassengerByIdAsync(createdTrip.PassengerId);
            var TipData = new TripDataResult 
            {
                PassengerName=$"{passenger.FirstName} {passenger.LastName}",
                BasePrice = createdTrip.BasePrice,
                destination=createdTrip.destination,
                source=createdTrip.source,
                Distance = createdTrip.Distance,
                PassengerRating=passenger.rating,
                RequestTime = createdTrip.RequestTime,
                Status = createdTrip.Status
            };
            var dict = new Dictionary<string, object>();
            dict.Add("type", "New Trip Requast");
            dict.Add("data", TipData);
            _webSocketManager.BroadcastdataToDriver(dict);
        }


        public async Task<TripServiceResponse> createTripRequest(Trip trip)
        {
            var existingtrip = await _tripRepository.findIFUserhasOpenedTripRequast(trip.PassengerId);
            if (existingtrip)
            {
                return new TripServiceResponse
                {
                    success = false,
                    Error = ["User has already opened Trip Reqeust"]
                };
            }
            trip.Status = TripStatue.DriverWaiting;
            trip.Distance=trip.source == trip.destination ? 0 : 50; // Example distance calculation
            trip.BasePrice=trip.Distance *10; // Example base price calculation
            var createdTrip = await _tripRepository.createtripRequest(trip);
            //notify all Driver of createdTrip  
            await notifyDriverwithnewConnection(createdTrip);
            return new CreatedTripRequestResponse
            { 
                success=true,
                PassengerId = trip.PassengerId,
                TripId = trip.TripId,
                source = trip.source,
                destination = trip.destination,
                Distance = trip.Distance,
                BasePrice = trip.BasePrice,
                Status = trip.Status.ToString(),
                RequestTime = trip.RequestTime
            };
        }

        public async Task<TripServiceResponse> DriverConfirmTender(Guid tenderId)
        {

            var tender = await _tenderRepository.GetTenderByIdAsync(tenderId);
            if (tender == null)
            {
                throw new Exception($"Tender not found on id{tenderId}");
            }
            var trip = await _tripRepository.getTripByIdAsync(tender.TripId);
            if (trip.Status == TripStatue.WaitingForConifirmationOnTender && tender.staute == TenderStatue.WaitingForDriverConfirimation)
            {
                trip.StartTime = DateTime.UtcNow;
                trip.Status = TripStatue.DriverAccepted;
                trip.FinalPrice = tender.OfferedPrice;
                trip.CancellationFee = 30;
                await _tripRepository.updatetripAsync(trip);
                tender.staute = TenderStatue.DriverConfirm;
                await _tenderRepository.updateTender(tender);
                return new AcceptedTripResponse 
                {
                    success = true,
                    DriverId=trip.DriverId,
                    PassengerId=trip.PassengerId,
                    BasePrice=trip.BasePrice,
                    source=trip.source,
                    destination=trip.destination,
                    Distance=trip.Distance,
                    StartTime=trip.StartTime,
                    TripId=trip.TripId,
                    RequestTime=trip.RequestTime,
                    CancellationFee=trip.CancellationFee,
                    FinalPrice=trip.FinalPrice,
                    Status=trip.Status,
                    
                };
            }
            else
            {
             return new TripServiceResponse { success = true,Error=["Tender is no longer valid"] };
            }
        }

        public async Task<TripServiceResponse> endTrip(Guid tripId)
        {
            var trip = await _tripRepository.getTripByIdAsync(tripId);
            if (trip == null)
            {
                throw new Exception($"Trip not found on id{tripId}");
            }
            if (trip.Status == TripStatue.TripBegin)
            {
                trip.EndTime = DateTime.UtcNow;
                trip.Status = TripStatue.TripCompleted;
                _webSocketManager.RemoveConnection(tripId.ToString());
                await  _tripRepository.updatetripAsync(trip);
                return new AcceptedTripResponse
                {
                    success = true,
                    DriverId = trip.DriverId,
                    PassengerId = trip.PassengerId,
                    BasePrice = trip.BasePrice,
                    source = trip.source,
                    destination = trip.destination,
                    Distance = trip.Distance,
                    StartTime = trip.StartTime,
                    TripId = tripId,
                    RequestTime = trip.RequestTime,
                    CancellationFee = trip.CancellationFee,
                    FinalPrice = trip.FinalPrice,
                    Status = trip.Status,

                };
            }
            return new TripServiceResponse 
            {
                success=false,
                Error = [$"trip isnt in valid statue"]
            };
        }

        public Task<TripServiceResponse> rejectTenderOffer(Guid tenderId)
        {
            throw new NotImplementedException();
        }
        // Here When Driver pickUp Passenger and start Trip
        public async Task<TripServiceResponse> startTrip(Guid tripId)
        {
            var trip = await _tripRepository.getTripByIdAsync(tripId);
            if (trip == null)
            {
                throw new Exception($"Trip not found on id{tripId}");
            }
            if (trip.Status == TripStatue.DriverAccepted)
            {
                trip.Status = TripStatue.TripBegin;
                await _tripRepository.updatetripAsync(trip);
                return new AcceptedTripResponse
                {
                    success = true,
                    DriverId = trip.DriverId,
                    PassengerId = trip.PassengerId,
                    BasePrice = trip.BasePrice,
                    source = trip.source,
                    destination = trip.destination,
                    Distance = trip.Distance,
                    StartTime = trip.StartTime,
                    TripId = tripId,
                    RequestTime = trip.RequestTime,
                    CancellationFee = trip.CancellationFee,
                    FinalPrice = trip.FinalPrice,
                    Status = trip.Status,

                };
            }
            return new TripServiceResponse
            {
                success = false,
                Error = [$"trip isnt in valid statue"]
            };
        }

        public async Task<bool> canDriverLithenToTrips(string driverId)
        {
            var driver= await _driverRepository.getDriverByIdAsync(driverId);
            return driver.isAvailable;
        }
    }
}
