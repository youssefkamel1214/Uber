using Authnitication.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using Uber.Models.Domain;
using Uber.Models.DTO.Reqeusts;
using Uber.Models.Responses;
using Uber.Repositories;
using Uber.Repositories.Interfaces;
using Uber.Services;
using Uber.Services.Interfaces;
using Uber.Utils;
using Uber.WebSockets;

namespace Uber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        ITripService _tripService;
        private readonly webSocketManager _webSocketManager; 
        private readonly IWebSocketService _webSocketservice;
        private readonly ITripRepository _tripRepository;
        private readonly ITenderRepository _tenderRepository;

        public TripController(ITripService tripService,webSocketManager webSocketManager,
            IWebSocketService webSocketservice, ITripRepository tripRepository, ITenderRepository tenderRepository)
        {
            _tripService = tripService;
            _webSocketManager = webSocketManager;
            _webSocketservice = webSocketservice;
            _tripRepository = tripRepository;
            _tenderRepository = tenderRepository;
        }
        [HttpPost]
        [Route("CompleteTrip")]
        [Authorize(Roles ="Driver")]
        public async Task<IActionResult> CompleteTrip([FromQuery] Guid tripId) 
        {
            var user = HttpContext.User;
            var userId = user.FindFirst("UID")!.Value;
            var result = await _tripService.endTrip(tripId,userId);
            return result.HandleRespose();
        }
        [HttpPost]
        [Route ("ConfirmPickUp")]
        [Authorize(Roles ="Driver")]
        public async Task<IActionResult> ConfirmPickUp([FromQuery] Guid tripId) 
        {
            var user = HttpContext.User;
            var userId = user.FindFirst("UID")!.Value;
            var result = await _tripService.startTrip(tripId,userId);
            return result.HandleRespose();
        }
        [HttpPost]
        [Route("MakeReview")]
        [Authorize]
        public async Task<IActionResult> MakeReview([FromBody] ReviewRequestDto requestDto)
        {
            var user = HttpContext.User;
            var userId = user.FindFirst("UID")!.Value;
            Review review = new Review
            {
                TripId = requestDto.TripId,
                ReviewerId = userId,
                Rating = requestDto.Rating,
                Comment = requestDto.Comment
            };
            var result = await _tripService.AddReviewforTrip(review);
            return result.HandleRespose();
        }
        [HttpPost]
        [Route("CancelTrip")]
        [Authorize]
        public async Task<IActionResult> requestTrip([FromQuery] Guid tripId)
        {
            var user = HttpContext.User;
            var userId = user.FindFirst("UID")!.Value;

            var result = await _tripService.cancelTrip(tripId,userId);
            return result.HandleRespose();

        }
        [HttpPost]
        [Route("requestTrip")]
        [Authorize(Roles ="Passenger")]
        public async Task<IActionResult> requestTrip([FromBody] RequestTripDto requestTrip) 
        {
            var user = HttpContext.User;
            var userId = user.FindFirst("UID")!.Value;

            var result = await _tripService.createTripRequest(new Trip{
               Source = requestTrip.source,
               Destination = requestTrip.destination,
               PassengerId = userId
            });
            return result.HandleRespose();

        }
        [HttpGet]
        [Route("LithenOnTrips")]
        [Authorize(Roles = "Driver")]
        public async Task lithenOnTrips() 
        {

            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }
            var user = HttpContext.User;
            var userId = user.FindFirst("UID")!.Value;
            WebSocket socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var receiveBuffer = new byte[1024 * 4];

            await _webSocketservice.assignSocketAsync(socket);
            _webSocketManager.AddConnectionDriver(userId, _webSocketservice);
            var trips=await _tripRepository.getavailbleTripsAsync(userId);
            var datatosend = new Dictionary<string,object>();
            datatosend.Add("type", "AvailbleTrips");
            datatosend.Add("count",trips.Count());
            datatosend.Add("data", trips);
            await  _webSocketservice.Broadcastdata(datatosend);
            try
            {
                while (socket.State == WebSocketState.Open)
                {

                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                    _webSocketservice.closeConnection();
                }
            }
            finally
            {
                _webSocketManager.RemoveConnectionDriver(userId);
            }
        }
        [HttpGet("LithenOnTripChannel")]
        [Authorize]
        public async Task Connect([FromQuery] Guid tripId)
        {
            var trip = await _tripRepository.getTripByIdAsync(tripId);
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }
            if (trip == null)
            {
                HttpContext.Response.StatusCode = 404;
                return;
            }
            var user = HttpContext.User;
            var userId = user.FindFirst("UID")!.Value;            
            if (trip.PassengerId!=userId&&trip.DriverId != userId) 
            {
                HttpContext.Response.StatusCode = 403;
                return;
            }
            WebSocket socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var receiveBuffer = new byte[1024 * 4];
            await _webSocketservice.assignSocketAsync(socket);
            _webSocketManager.AddConnectionOnTripId(tripId.ToString(), _webSocketservice);
            if (trip.Status == TripStatue.DriverWaiting) 
            {
                List<TenderDataResponse> tenders = await _tenderRepository.GetTendersByTripIdAsync(tripId);
                var dataToSend = new Dictionary<string, object>();
                dataToSend.Add("type", "AvailbleTenders");
                dataToSend.Add("count", tenders.Count());
                dataToSend.Add("data", tenders);
    
                await _webSocketservice.Broadcastdata(dataToSend);
            }
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {    
                        break;
                    }
                }
            }
            finally
            {
                _webSocketManager.RemoveConnectionOnTrip(tripId.ToString(),_webSocketservice);
            }
        }
    }
}
