using GlucoseGurusWebApi.WebApi.Models;
using GlucoseGurusWebApi.WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlucoseGurusWebApi.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("Docters")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _docterRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(IDoctorRepository docterRepository, IAuthenticationService authenticationService, ILogger<DoctorController> logger)
        {
            _docterRepository = docterRepository;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet(Name = "ReadDocters")]
        public async Task<ActionResult<IEnumerable<Doctor>>> Get()
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var docters = await _docterRepository.ReadAllAsync();
            return Ok(docters);
        }

        [HttpGet("{docterId}", Name = "ReadDocter")]
        public async Task<ActionResult<Doctor>> Get(Guid docterId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var docter = await _docterRepository.ReadAsync(docterId);
            if (docter == null)
                return NotFound($"Docter does not exist.");

            return Ok(docter);
        }

        [HttpPost(Name = "CreateDocter")]
        public async Task<ActionResult<Doctor>> Add(Doctor newDocter)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var docter = await _docterRepository.InsertAsync(newDocter);
            return CreatedAtRoute("ReadDocter", new { docterId = docter.Id }, docter);
        }

        [HttpPut("{docterId}", Name = "UpdateDocter")]
        public async Task<ActionResult> Update(Guid docterId, Doctor updatedDocter)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var docter = await _docterRepository.ReadAsync(docterId);
            if (docter == null)
                return NotFound($"Docter does not exist.");

            updatedDocter.Id = docterId;
            await _docterRepository.UpdateAsync(updatedDocter);
            return Ok(updatedDocter);
        }

        [HttpDelete("{docterId}", Name = "DeleteDocter")]
        public async Task<ActionResult> Delete(Guid docterId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var docter = await _docterRepository.ReadAsync(docterId);
            if (docter == null)
                return NotFound($"Docter does not exist.");

            await _docterRepository.DeleteAsync(docterId);
            return Ok();
        }
    }
}
