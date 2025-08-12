using Authnitication.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using Uber.Models.Domain;
using Uber.Models.DTO.Reqeusts;
using Uber.Models.Responses;
using Uber.Repositories.Interfaces;
using Uber.Services;
using Uber.Services.Interfaces;
using Uber.WebSockets;

namespace Uber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        ITripService _tripService;
        IAuthniticationService _authService;
        private readonly webSocketManager _webSocketManager; 
        private readonly IWebSocketService _driverSocketService;
        private readonly ITripRepository _tripRepository;

        public TripController(ITripService tripService, IAuthniticationService authService, 
            webSocketManager webSocketManager, IWebSocketService driverSocketService, ITripRepository tripRepository)
        {
            _tripService = tripService;
            _authService = authService;
            _webSocketManager = webSocketManager;
            _driverSocketService = driverSocketService;
            _tripRepository = tripRepository;
        }
        [HttpPost]
        [Route("requestTrip")]
        [Authorize(Roles ="Passenger")]
        public async Task<IActionResult> requestTrip([FromBody] RequestTripDto requestTrip) 
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            var userId =  await _authService.getUID(token);

            var result = await _tripService.createTripRequest(new Trip{
               source = requestTrip.source,
               destination = requestTrip.destination,
               PassengerId = userId
            });
            if(result.success)
                return Ok(result);
            else return BadRequest(result);
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
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            var userId = await _authService.getUID(token);
            WebSocket socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var receiveBuffer = new byte[1024 * 4];

            await _driverSocketService.assignSocketAsync(socket);
            _webSocketManager.AddConnectionDriver(userId, _driverSocketService);
            var trips=await _tripRepository.getavailbleTripsAsync();
            await  _driverSocketService.Broadcastdata(trips);
            try
            {
                while (socket.State == WebSocketState.Open)
                {

                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                    _driverSocketService.closeConnection();
                }
            }
            finally
            {
                _webSocketManager.RemoveConnectionDriver(userId);
            }
        }

    }
}
