using System.Net;
using System.Threading.Tasks;
using Uber.Data;
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
        private readonly ICancellRepository _cancellRepository;
        private readonly UberAuthDatabase _db;
        private readonly INotificationManger _notificationManger;
        private readonly IRouteDistanceService _routeDistanceService;

        public TripService(ITenderRepository tenderRepository, ITripRepository tripRepository, IReviewRepository reviewRepository,
            webSocketManager webSocketManager, IDriverRepository driverRepository, IPassengerRepository passengerRepository,
            ICancellRepository cancellRepository, UberAuthDatabase db, INotificationManger notificationManger, IRouteDistanceService routeDistanceService)
        {
            _tenderRepository = tenderRepository;
            _tripRepository = tripRepository;
            _reviewRepository = reviewRepository;
            _webSocketManager = webSocketManager;
            _driverRepository = driverRepository;
            _passengerRepository = passengerRepository;
            _cancellRepository = cancellRepository;
            _db = db;
            _notificationManger = notificationManger;
            _routeDistanceService = routeDistanceService;
        }

        public async Task<ApiResponse> acceptTenderOffer(Guid tenderId, string userId)
        {
            var existingTender=await _tenderRepository.GetTenderByIdAsync(tenderId);
            if(existingTender==null)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.NotFound,
                    Error = ["there not Tender with this Tender Id"]
                };
            }
            var existingtrip= await _tripRepository.getTripByIdAsync(existingTender.TripId);
            if (existingtrip == null)
            {
                throw new Exception($"Trip not Found for id :{existingTender.TripId}");
            }
            if (existingtrip.PassengerId != userId)
            {
                return new ApiResponse
                {
                    statue=HttpStatusCode.Forbidden,
                    Error = ["You dont have Permission for this Reqaust"]
                };
            }
            if (existingtrip.Status != TripStatue.DriverWaiting) 
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.BadRequest,
                    Error = [$"invalid Trip Statue :{existingtrip.Status}"]
                };
            }
            if (existingTender.staute==TenderStatue.Expired)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.Gone,
                    Error = [$"Tender has expired at {existingTender.TenderTime.AddMinutes(5).ToLocalTime()}"]
                };
            }
            if (existingTender.staute == TenderStatue.WaitingForPassenger)
            {
                var driver =await  _driverRepository.getDriverByIdAsync(existingTender.DriverId);
                if (!driver.IsActive ||! driver.isAvailable) 
                {
                    existingTender.staute= TenderStatue.driverGotAnotherTrip;
                    await _tenderRepository.updateTender(existingTender);
                    return new ApiResponse
                    {
                        statue = HttpStatusCode.Gone,
                        Error = [$"Driver Got another Trip"]
                    };
                }
                await using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    existingTender.staute=TenderStatue.WaitingForDriverConfirimation;
                    await _tenderRepository.updateTender(existingTender);
                    existingtrip.Status = TripStatue.WaitingForConifirmationOnTender;
                    await _tripRepository.updatetripAsync(existingtrip);
                    var datatoSent = new Dictionary<string, object>();
                    datatoSent.Add("Tender Id", existingTender.TenderId);
                    datatoSent.Add("Tender Statue", existingTender.staute.ToString());
                    datatoSent.Add("Tender  confirmation Time", DateTime.UtcNow.ToLocalTime().ToString());
                    datatoSent.Add("Time Remaning To Confirm Tender and Accept Trip is ", $"55 seconds");
                    await _webSocketManager.BroadcastdataToDriver(existingTender.DriverId,datatoSent);
                    await transaction.CommitAsync();
                    return new ApiResponse
                    {
                        statue = HttpStatusCode.OK,
                        Message=$"Waiting For Driver to Confirm Tender You Cant Accept Any Tender for 60 Socends until Confirmation Times is up"
                    };
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else 
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.BadRequest,
                    Error = [$"invalid Tender Statue :{existingTender.staute}"]
                };
            }
        }
        
        public async Task<ApiResponse> AddReviewforTrip(Review review)
        {        
            if ( await _reviewRepository.getIfThereReviewForSameTrip(review.TripId,review.ReviewerId))
            {
                return new ApiResponse {
                    statue = HttpStatusCode.BadRequest,
                    Error = ["user already made review on this trip" ]
                };
            }
            var exsitingtrip=await _tripRepository.getTripByIdAsync(review.TripId);
            if (exsitingtrip == null)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.NotFound,
                    Error = [$"trip was not found on id {review.TripId}"]
                };
            }
            if(exsitingtrip.Status != TripStatue.TripCompleted)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.BadRequest,
                    Error = ["Trip is not completed yet you cant make review"]
                };
            }
            if(exsitingtrip.DriverId!=review.ReviewerId&&exsitingtrip.PassengerId!=review.ReviewerId)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.Forbidden,
                    Error = ["user dont have permssion on this trip"]
                };
            }
            review.type = exsitingtrip.DriverId == review.ReviewerId ?ReviewType.aboutPassenger:ReviewType.aboutDriver;
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var addedReview = await _reviewRepository.addReview(review);
                if (review.type == ReviewType.aboutPassenger && exsitingtrip.PassengerId != null)
                {
                    var passenger = await _passengerRepository.getPassengerByIdAsync(exsitingtrip.PassengerId);
                    if (passenger != null)
                    {
                        passenger.rating = (passenger.rating??0 * passenger.NumberOfReviews + review.Rating) / (passenger.NumberOfReviews + 1);
                        passenger.NumberOfReviews++;
                        await _passengerRepository.updatePassenger(passenger);
                    }

                }
                else if (exsitingtrip.DriverId != null)
                {
                    var driver = await _driverRepository.getDriverByIdAsync(exsitingtrip.DriverId);
                    if (driver != null)
                    {
                        driver.rating = (driver.rating??0 * driver.NumberOfReviews + review.Rating) / (driver.NumberOfReviews + 1);
                        driver.NumberOfReviews++;
                        await _driverRepository.updateDriver(driver);
                    }
                }
                await transaction.CommitAsync();
                return new ApiResponse
                {
                    statue = HttpStatusCode.Created,
                    data = addedReview
                };
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<ApiResponse> cancelTrip(Guid tripId, string UID)
        {
            var trip = await _tripRepository.getTripByIdAsync(tripId);
            if (trip == null)
            {
                return new ApiResponse 
                {
                    statue=HttpStatusCode.NotFound,
                    Error = [$"no Trip for id :{tripId}"]
                };
            }
            string cancelledBy ="";
            if (trip.DriverId == UID)
            {
                cancelledBy = UID;
            }
            else if (trip.PassengerId == UID) 
            {
                cancelledBy =UID;
            }
            else 
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.Forbidden,
                    Error = [$"you dont have premssion on this trip"]
                };
            }
            if (trip.Status == TripStatue.TripCancelled)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.BadRequest,
                    Error = [$"Trip was canCelledBefor :{trip.EndTime}"]
                };
            }
            else if (trip.Status == TripStatue.TripCompleted)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.BadRequest,
                    Error = [$"Trip  completed at :{trip.EndTime}"]
                };
            }
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                if (trip.DriverId != null)
                {
                    Driver? driver = await _driverRepository.getDriverByIdAsync(trip.DriverId);
                    if (driver != null) 
                    {
                        driver.isAvailable = true;
                        await _driverRepository.updateDriver(driver);
                    }
                }

                var canceltime = DateTime.UtcNow;
                var cancellation = new Cancellation
                {
                    TripId = tripId,
                    CancelledBy = cancelledBy,
                    CancellationTime = canceltime,
                    Fee = 0
                };
                if (cancelledBy == trip.DriverId)
                {
                    if (trip.Status == TripStatue.DriverAccepted)
                        cancellation.Fee = 100;
                    else
                    {
                        cancellation.Fee = 500;
                    }
                    trip.Status = TripStatue.DriverWaiting;
                    trip.DriverId = null;

                }
                else
                {
                    if (trip.Status == TripStatue.DriverAccepted)
                        cancellation.Fee = trip.FinalPrice / 10;
                    else if (trip.Status == Utils.TripStatue.TripBegin)
                    {
                        cancellation.Fee = trip.FinalPrice / 2;
                    }
                    trip.Status = TripStatue.TripCancelled;
                    trip.EndTime = cancellation.CancellationTime;
                }
                await _tripRepository.updatetripAsync(trip);
                cancellation = await _cancellRepository.AddCancellationAsync(cancellation);
                await transaction.CommitAsync();
                if (trip.Status == TripStatue.DriverWaiting) 
                {
                    var data=new Dictionary<string, object>();
                    data.Add("type","system Message");
                    data.Add("message",$"Driver Cancelled Trip Trip Statue now is :{trip.Status}");
                    _notificationManger.notifyTripChannel(trip.TripId.ToString(), data);
                }
                cancellation.Trip = null;
                return new ApiResponse
                {
                    statue = HttpStatusCode.Created,
                    data = cancellation
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
           
        }

        public async Task<ApiResponse> createTenderOnTrip(Tender tender)
        {
            var existingtrip = await _tripRepository.getTripByIdAsync(tender.TripId);
            var driver=await _driverRepository.getDriverByIdAsync(tender.DriverId);
            var cancellationForSameDriver = await _cancellRepository.getCancelletionByDriverIAndTripID(tender.DriverId,tender.TripId);
            if (existingtrip == null)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.NotFound,
                    Error = [$"Trip not found on id {tender.TripId}"]
                };
            }
            if(existingtrip.Status != TripStatue.DriverWaiting&& existingtrip.Status != TripStatue.WaitingForConifirmationOnTender)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.Gone,
                    Error = [$"Trip is not in a valid state to create a tender :{existingtrip.Status}"]
                };
            }
            if (cancellationForSameDriver != null) 
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.Gone,
                    Error = [$"Driver has cancelled this trip before you cant add Tender on this trip "]
                };
            }
            if (!driver.isAvailable) 
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.BadRequest,
                    Error = [$"cant Add Tender While you in other Trip"]
                };
            }
            if (await _tenderRepository.isThereActiveDriverTenderForThisTrip(tender.TripId, tender.DriverId))
            {

                return new ApiResponse
                {
                    statue = HttpStatusCode.BadRequest,
                    Error = [$"There Already Active Tender for this Driver on this Trip"]
                };
            }
            if (tender.OfferedPrice < existingtrip.BasePrice)
            {
                return new ApiResponse
                {

                    statue = HttpStatusCode.BadRequest,
                    Error  = [$"Offered price cant be less than {existingtrip.BasePrice}"]
                };
            }

            var createdTender = await  _tenderRepository.AddTender(tender);
            createdTender.Trip = null;
            createdTender.Driver = null;
            //Create notification for Tender Lithener
            var TenderData = new TenderDataResponse
            {
                TenderId = tender.TenderId,
                DriverName = driver.UserName,
                DriverRating = driver.rating,
                OfferedPrice = tender.OfferedPrice,
                ExpiryTime = tender.TenderTime.ToLocalTime().AddMinutes(5),
                DriverPhoneNumber = driver.PhoneNumber,
                licensePlate = driver.LicensePlate
            };
            var dict = new Dictionary<string, object>();
            dict.Add("type", "Driver and Tender Data");
            dict.Add("data", TenderData);
            _notificationManger.notifyTripChannel(tender.TripId.ToString(), dict);

            return new ApiResponse
            {
                statue = HttpStatusCode.Created,
                data= createdTender

            };
        }


        public async Task<ApiResponse> createTripRequest(Trip trip)
        {
            var existingtrip = await _tripRepository.findIFUserhasOpenedTripRequast(trip.PassengerId);
            if (existingtrip)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.BadRequest,
                    Error = ["User has already opened Trip Reqeust"]
                };
            }
            trip.Status = TripStatue.DriverWaiting;
            trip.Distance=trip.Source.Equals(trip.Destination) ? 0 :
                await _routeDistanceService.GetDistanceAsync(trip.Source.Latitude,trip.Source.Longtitde,trip.Destination.Latitude,trip.Destination.Longtitde) ; // Example distance calculation
            trip.BasePrice=trip.Distance *10; // Example base price calculation
            var createdTrip = await _tripRepository.createtripRequest(trip);
            //notify all Driver of createdTrip  
            var passenger = await _passengerRepository.getPassengerByIdAsync(createdTrip.PassengerId);
            var TipData = new TripDataResult
            {
                PassengerName = passenger.UserName,
                BasePrice = createdTrip.BasePrice,
                destination = createdTrip.Destination,
                source = createdTrip.Source,
                Distance = createdTrip.Distance,
                PassengerRating = passenger.rating,
                RequestTime = createdTrip.RequestTime,
                Status = createdTrip.Status
            };
            var dict = new Dictionary<string, object>();
            dict.Add("type", "New Trip Requast");
            dict.Add("data", TipData);
            _notificationManger.notifyDriversChannel(dict);

            var data= new CreatedTripRequestResponse
            { 
                PassengerId = trip.PassengerId,
                TripId = trip.TripId,
                source = trip.Source,
                destination = trip.Destination,
                Distance = trip.Distance,
                BasePrice = trip.BasePrice,
                Status = trip.Status.ToString(),
                RequestTime = trip.RequestTime
            };
            return new ApiResponse
            {
                statue = HttpStatusCode.Created,
                data = data,
            };
        }

        public async Task<ApiResponse> DriverConfirmTender(Guid tenderId, string userId)
        {

            var tender = await _tenderRepository.GetTenderByIdAsync(tenderId);
            if (tender == null)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.NotFound,
                    Error = [$"Tender not found on id{tenderId}"]
                };
            }
            if (tender.DriverId != userId)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.Forbidden,
                    Error = ["User dont have permssion on this Tender"]
                };
            }
            var trip = await _tripRepository.getTripByIdAsync(tender.TripId);
            if (trip.Status == TripStatue.WaitingForConifirmationOnTender && tender.staute == TenderStatue.WaitingForDriverConfirimation)
            {
                await using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    trip.StartTime = DateTime.UtcNow;
                    trip.Status = TripStatue.DriverAccepted;
                    trip.FinalPrice = tender.OfferedPrice;
                    trip.DriverId=tender.DriverId;
                    tender.staute = TenderStatue.DriverConfirm;
                    var restTenderswithTrips = await _tenderRepository.getRestOfTripsThathasTenders(tender);
                    await _tripRepository.updatetripAsync(trip);
                    await _tenderRepository.updateTender(tender);
                    await _tripRepository.updateRestOfTripsThathasTenders(tender);
                    await _tenderRepository.updateRestOfActiveTenders(tender);
                    var dataToSendToDriverChannel = new Dictionary<string, object>();
                    dataToSendToDriverChannel.Add("type", "System Message");
                    dataToSendToDriverChannel.Add("Message", "you should go listhen on Trip Channel for User Messages");
                    var dataToSendToTripChannel = new Dictionary<string, object>();
                    dataToSendToTripChannel.Add("type", "Trip Accepted from Driver ");
                    dataToSendToTripChannel.Add("data", trip);
                    transaction.Commit();
                    _notificationManger.notifyDriverChannel(tender.DriverId, dataToSendToDriverChannel);
                    _notificationManger.notifyTripChannel(trip.TripId.ToString(), dataToSendToTripChannel);
                    foreach (var tend in restTenderswithTrips) 
                    {
                        var datatosend=new Dictionary<string, object>();
                        dataToSendToTripChannel.Add("type", "Tender Statue changed ");
                        dataToSendToTripChannel.Add("data", tend);
                        _notificationManger.notifyTripChannel(tend.TripId.ToString(), datatosend);
                    }
                    var data=  new AcceptedTripResponse 
                    {
                    
                        DriverId=trip.DriverId,
                        PassengerId=trip.PassengerId,
                        BasePrice=trip.BasePrice,
                        source=trip.Source,
                        destination=trip.Destination,
                        Distance=trip.Distance,
                        StartTime=trip.StartTime,
                        TripId=trip.TripId,
                        RequestTime=trip.RequestTime,
                        FinalPrice=trip.FinalPrice,
                        Status=trip.Status,
                    
                    };
                    return new ApiResponse
                    {
                        statue=HttpStatusCode.OK,
                        data=data,
                    };
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
             return new ApiResponse { statue = HttpStatusCode.Gone, Error=["Tender is no longer valid"] };
            }
        }


        public async Task<ApiResponse> endTrip(Guid tripId,string driverId)
        {
            var trip = await _tripRepository.getTripByIdAsync(tripId);
            if (trip == null)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.NotFound,
                    Error = [$"There no trip with this Id{tripId}"]
                };
            }
            if (driverId != trip.DriverId)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.Forbidden,
                    Error = [$"Permssion Denied on this Trip"]
                };
            }
            if (trip.Status == TripStatue.TripBegin)
            {
                await using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    trip.EndTime = DateTime.UtcNow;
                    trip.Status = TripStatue.TripCompleted;
                    await  _tripRepository.updatetripAsync(trip);
                    var dataToSendToTripChannel = new Dictionary<string, object>();
                    dataToSendToTripChannel.Add("type", "Trip has Ended ");
                    dataToSendToTripChannel.Add("message", $"Thank for Trusting our Application dont forget Your Review Matters");
                    await transaction.CommitAsync();
                    await _webSocketManager.BroadcastdataToTripChannel(trip.TripId.ToString(), dataToSendToTripChannel);
                    _webSocketManager.RemoveAllConnectionOnTrip(tripId.ToString());
                    var data= new AcceptedTripResponse
                    {
                        DriverId = trip.DriverId,
                        PassengerId = trip.PassengerId,
                        BasePrice = trip.BasePrice,
                        source = trip.Source,
                        destination = trip.Destination,
                        Distance = trip.Distance,
                        StartTime = trip.StartTime,
                        TripId = tripId,
                        RequestTime = trip.RequestTime,
                        FinalPrice = trip.FinalPrice,
                        Status = trip.Status,

                    };
                    return new ApiResponse
                    {
                        statue=HttpStatusCode.OK,
                        data=data
                    };
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return new ApiResponse 
            {
                statue = HttpStatusCode.BadRequest,
                Error = [$"trip isnt in valid statue"]
            };
        }

        public async Task<ApiResponse> rejectTenderOffer(Guid tenderId, string userId)
        {
            var existingTender= await _tenderRepository.GetTenderByIdAsync(tenderId);
            if (existingTender == null) 
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.NotFound,
                    Error = [$"There no Tender with id :{tenderId}"]
                };
            }
            if (existingTender.staute == TenderStatue.WaitingForPassenger) 
            {
                existingTender.staute = TenderStatue.Rejected;
                if (await _tenderRepository.updateTender(existingTender))
                {
                    var datatoSent = new Dictionary<string, object>();
                    datatoSent.Add("Tender Id", existingTender.TenderId);
                    datatoSent.Add("Tender Statue", existingTender.staute.ToString());
                    datatoSent.Add("Tender Rejection Time", DateTime.UtcNow.ToLocalTime().ToString());
                    await _webSocketManager.BroadcastdataToDriver(existingTender.DriverId,datatoSent);
                    return new ApiResponse
                    {
                        statue = HttpStatusCode.OK,
                        Message = "Tender Rejected Successefully"
                    };
                }
                else
                {
                    throw new Exception($"Tender couldnt be rejected at id {tenderId}");
                }
            }
            return new ApiResponse
            {
                statue = HttpStatusCode.BadRequest,
                Error = [$"Invalid Tender Satue :{existingTender.staute.ToString()}"]
            };

        }
        // Here When Driver pickUp Passenger and start Trip
        public async Task<ApiResponse> startTrip(Guid tripId,string driverId)
        {
            var trip = await _tripRepository.getTripByIdAsync(tripId);
            if (trip == null)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.NotFound,
                    Error =[$"There no trip with this Id{tripId}"]
                };
            }
            if (driverId != trip.DriverId) 
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.Forbidden,
                    Error = [$"Permssion Denied on this Trip"]
                };
            }
            if (trip.Status == TripStatue.DriverAccepted)
            {
                trip.Status = TripStatue.TripBegin;
                await _tripRepository.updatetripAsync(trip);
                var data=new AcceptedTripResponse
                {
                    DriverId = trip.DriverId,
                    PassengerId = trip.PassengerId,
                    BasePrice = trip.BasePrice,
                    source = trip.Source,
                    destination = trip.Destination,
                    Distance = trip.Distance,
                    StartTime = trip.StartTime,
                    TripId = tripId,
                    RequestTime = trip.RequestTime,
                    FinalPrice = trip.FinalPrice,
                    Status = trip.Status,

                };
                return new ApiResponse
                {
                    statue = HttpStatusCode.OK,
                    data=data
                };
            }
            return new ApiResponse
            {
                statue = HttpStatusCode.BadRequest,
                Error = [$"trip isnt in valid statue"]
            };
        }

        public async Task<ApiResponse> updateExistedTender(Guid tenderId, string userId, decimal offeredPrice)
        {
            var tender = await _tenderRepository.GetTenderByIdAsync(tenderId);
            if (tender == null)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.NotFound,
                    Error = [$"Tender not found on id{tenderId}"]
                };
            }
            if (tender.DriverId != userId)
            {
                return new ApiResponse
                {
                    statue = HttpStatusCode.Forbidden,
                    Error = ["User dont have permssion on this Tender"]
                };
            }
            var trip = await _tripRepository.getTripByIdAsync(tender.TripId);
            if (trip.Status == TripStatue.DriverWaiting && tender.staute == TenderStatue.WaitingForPassenger)
            {
                if (offeredPrice < trip.BasePrice)
                {
                    return new ApiResponse
                    {
                        statue = HttpStatusCode.BadRequest,
                        Error = [$"offered Price could not be less than :{trip.BasePrice}"]
                    };
                }
                tender.OfferedPrice =offeredPrice;
                if (await _tenderRepository.updateTender(tender))
                    return new ApiResponse
                    {
                        statue = HttpStatusCode.OK,
                        data = tender,
                    };
                else
                    throw new Exception($"could not update tender data at time {DateTime.UtcNow} for Id :{tenderId}");
            }
            else
            {
                return new ApiResponse { statue = HttpStatusCode.Gone, Error = ["Tender is no longer valid"] };
            }
        }
    }
}
