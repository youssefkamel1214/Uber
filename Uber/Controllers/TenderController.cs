using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uber.Models.Domain;
using Uber.Models.DTO.Reqeusts;
using Uber.Services.Interfaces;

namespace Uber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenderController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TenderController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpPost("ConfirmTender")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> confirmTender([FromBody] Guid tenderID)
        {
            var user = HttpContext.User;
            var userId = user.FindFirst("UID")!.Value;
            var result = await _tripService.DriverConfirmTender(tenderID,userId);
            
       
            return result.HandleRespose();
        }
        [HttpPost("updateTender")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> UpdateTender([FromQuery]Guid tenderId,[FromBody] decimal OfferedPrice)
        {
            var user = HttpContext.User;
            var userId = user.FindFirst("UID")!.Value;
            var result = await _tripService.updateExistedTender(tenderId,userId,OfferedPrice);
            return result.HandleRespose();

        }
        [HttpPost("addTender")]
        [Authorize(Roles ="Driver")]
        public async Task<IActionResult> AddTender([FromBody] AddTenderRequestDto tenderRequest)
        {
            var user = HttpContext.User;
            var userId = user.FindFirst("UID")!.Value;
            var result = await _tripService.createTenderOnTrip(new Tender
                    {
                        DriverId = userId,
                        TripId = tenderRequest.TripId,
                        TenderTime = DateTime.UtcNow,
                        OfferedPrice = tenderRequest.OfferedPrice
                    });
            return result.HandleRespose();

        }
        [HttpPost("AcceptTenderOffer")]
        [Authorize(Roles = "Passenger")]
        public async Task<IActionResult> acceptTenderOffer([FromBody] Guid tenderID)
        {
            var user = HttpContext.User;
            var userId = user.FindFirst("UID")!.Value;
            var result = await _tripService.acceptTenderOffer(tenderID, userId);
            return result.HandleRespose();

        }
        [HttpPost("RejectTenderOffer")]
        [Authorize(Roles = "Passenger")]
        public async Task<IActionResult> rejectTenderOffer([FromBody] Guid tenderID)
        {
            var user = HttpContext.User;
            var userId = user.FindFirst("UID")!.Value;
            var result = await _tripService.rejectTenderOffer(tenderID, userId);
            return result.HandleRespose();
        }



    }
}
