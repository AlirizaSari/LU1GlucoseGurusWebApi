using GlucoseGurusWebApi.WebApi.Models;
using GlucoseGurusWebApi.WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlucoseGurusWebApi.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("careMoments")]
    public class CareMomentController : ControllerBase
    {
        private readonly ICareMomentRepository _careMomentRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<CareMomentController> _logger;

        public CareMomentController(ICareMomentRepository careMomentRepository, IAuthenticationService authenticationService, ILogger<CareMomentController> logger)
        {
            _careMomentRepository = careMomentRepository;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet(Name = "readCareMoments")]
        public async Task<ActionResult<IEnumerable<CareMoment>>> Get()
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var careMoments = await _careMomentRepository.ReadAllAsync();
            return Ok(careMoments);
        }

        [HttpGet("{careMomentId}", Name = "readCareMoment")]
        public async Task<ActionResult<CareMoment>> Get(Guid careMomentId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var careMoment = await _careMomentRepository.ReadAsync(careMomentId);
            if (careMoment == null)
                return NotFound($"CareMoment does not exist.");

            return Ok(careMoment);
        }

        [HttpPost(Name = "createCareMoment")]
        public async Task<ActionResult<CareMoment>> Add(CareMoment newCareMoment)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var careMoment = await _careMomentRepository.InsertAsync(newCareMoment);
            return CreatedAtRoute("readCareMoment", new { careMomentId = careMoment.Id }, careMoment);
        }

        [HttpPut("{careMomentId}", Name = "updateCareMoment")]
        public async Task<ActionResult<CareMoment>> Update(Guid careMomentId, CareMoment updatedCareMoment)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var careMoment = await _careMomentRepository.ReadAsync(careMomentId);
            if (careMoment == null)
                return NotFound($"CareMoment does not exist.");

            updatedCareMoment.Id = careMomentId;
            await _careMomentRepository.UpdateAsync(updatedCareMoment);
            return Ok(updatedCareMoment);
        }

        [HttpDelete("{careMomentId}", Name = "deleteCareMoment")]
        public async Task<ActionResult> Delete(Guid careMomentId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var careMoment = await _careMomentRepository.ReadAsync(careMomentId);
            if (careMoment == null)
                return NotFound($"CareMoment does not exist.");

            await _careMomentRepository.DeleteAsync(careMomentId);
            return NoContent();
        }

    }
}
