using Authnitication.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
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
    public class TenderController : ControllerBase
    {
        private readonly webSocketManager _webSocketManager; // Inject the manager
        private readonly IWebSocketService _tenderSocketService;
        private readonly ITripService _tripService;
        private readonly ITripRepository _tripRepository;
        private readonly IAuthniticationService _authService;
        private readonly ITenderRepository _tenderRepository;

        public TenderController(webSocketManager webSocketManager, IWebSocketService tenderSocketService, ITripService tripService, ITripRepository tripRepository, IAuthniticationService authService, ITenderRepository tenderRepository)
        {
            _webSocketManager = webSocketManager;
            _tenderSocketService = tenderSocketService;
            _tripService = tripService;
            _tripRepository = tripRepository;
            _authService = authService;
            _tenderRepository = tenderRepository;
        }
        [HttpPost("ConfirmTender")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> confirmTender([FromBody] Guid tenderID)
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();
            var userId = await _authService.getUID(token);

            var result = await _tripService.DriverConfirmTender(tenderID);
            if (result.success)
                return Ok(result);
            else
                return BadRequest(result);
        }
        [HttpPost("addTender")]
        [Authorize(Roles ="Driver")]
        public async Task<IActionResult> AddTender([FromBody] AddTenderRequestDto tenderRequest)
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();
            var userId = await _authService.getUID(token);

            var result = await _tripService.createTenderOnTrip(new Tender
                    {
                        DriverId = userId,
                        TripId = tenderRequest.TripId,
                        TenderTime = DateTime.UtcNow,
                        OfferedPrice = tenderRequest.OfferedPrice
                    });
            if (result.success)
                return Ok(result);
            else
                return BadRequest(result);
        }
        [HttpPost("AcceptTenderOffer")]
        [Authorize(Roles = "Passenger")]
        public async Task<IActionResult> acceptTenderOffer([FromBody] Guid tenderID)
        {
            //string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            //string token = authorizationHeader.Substring("Bearer ".Length).Trim();
            //var userId = await _authService.getUID(token);

            var result = await _tripService.acceptTenderOffer(tenderID);
            if (result.success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("LithenOnTenders")]
        [Authorize(Roles = "Passenger")]
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
            WebSocket socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var receiveBuffer = new byte[1024 * 4];

            await _tenderSocketService.assignSocketAsync(socket);
            _webSocketManager.AddConnection(tripId.ToString(), _tenderSocketService);
            var tenders= await _tenderRepository.GetTendersByTripIdAsync(tripId);
            await _tenderSocketService.Broadcastdata(tenders);
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
                _webSocketManager.RemoveConnection(tripId.ToString());
                _tenderSocketService.closeConnection();
            }
        }
    }
}
