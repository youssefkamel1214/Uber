using Authnitication.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Uber.Data;
using Uber.Models.Domain;
using Uber.Models.DTO.Reqeusts;
using Uber.Repositories.Interfaces;

namespace Uber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthniticationController : ControllerBase
    {
        private readonly IAuthniticationService<UberUser,UberAuthDatabase> _authService;
        private readonly IDriverRepository _driverRepository;
        private readonly IPassengerRepository _passengerRepository;

        public AuthniticationController(IAuthniticationService<UberUser, UberAuthDatabase> authService, IDriverRepository driverRepository, IPassengerRepository passengerRepository)
        {
            _authService = authService;
            _driverRepository = driverRepository;
            _passengerRepository = passengerRepository;
        }

        [HttpPost]
        [Route("SignUpDriver")]
        public async Task< IActionResult> SignUpDriver([FromBody] RegisterDriverRequestDto registerRequest)
        {
            // Here you would typically call your authentication service to create a user
            // For now, we will just return a success message
            Driver driver = new Driver
            {
                Id = Guid.NewGuid().ToString(),
                UserName = registerRequest.FirstName+"_"+registerRequest.LastName,
                Email = registerRequest.Email,
                PhoneNumber = registerRequest.PhoneNumber,
                LicensePlate = registerRequest.LicensePlate,
                SSN = registerRequest.SSN
            };
            
            var  result = await _authService.CreateUserAsync(
                driver, 
                registerRequest.password, "Driver");
            if (result.success)
            {
                await _driverRepository.MarkDriverIsActive(registerRequest.Email);
                return Ok(result);
            }
            else
            {
                return BadRequest(new { errors = result.Error });
            }
        }
        [HttpPost]
        [Route("SignUpPassenger")]
        public async Task<IActionResult> SignUpPassenger([FromBody] RegisterPassengerRequest registerRequest)
        {
            // Here you would typically call your authentication service to create a user
            // For now, we will just return a success message
            Passenger passenger = new Passenger
            {
                UserName = registerRequest.FirstName+"_"+ registerRequest.LastName,
                Email = registerRequest.Email,
                PhoneNumber = registerRequest.PhoneNumber, 
            };
            var result = await _authService.CreateUserAsync(passenger, registerRequest.Password, "Passenger");
            if (result.success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { errors = result.Error });
            }
        }
        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn([FromBody] LoginReqeustDto loginRequest)
        {
            // Here you would typically call your authentication service to sign in a user
            // For now, we will just return a success message
            var result = await _authService.signInUser(loginRequest.Email, loginRequest.Password);
            if (result.success)
            {
                await  _driverRepository.MarkDriverIsActive(loginRequest.Email);
                return Ok(result);
            }
            else
            {
                return BadRequest(new { errors = result.Error });
            }
        }

        

        [HttpPost]
        [Route("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequest)
        {
            // Here you would typically call your authentication service to refresh the token
            // For now, we will just return a success message
            var result = await _authService.getrefeshedtoken(refreshTokenRequest.Token, refreshTokenRequest.RefreshToken);
            if (result.success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { errors = result.Error });
            }
        }
    }
}
