using GlucoseGurusWebApi.WebApi.Models;
using GlucoseGurusWebApi.WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlucoseGurusWebApi.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("doctors")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(IDoctorRepository doctorRepository, IAuthenticationService authenticationService, ILogger<DoctorController> logger)
        {
            _doctorRepository = doctorRepository;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet(Name = "readDoctors")]
        public async Task<ActionResult<IEnumerable<Doctor>>> Get()
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var doctors = await _doctorRepository.ReadAllAsync();
            return Ok(doctors);
        }

        [HttpGet("{doctorId}", Name = "readDoctor")]
        public async Task<ActionResult<Doctor>> Get(Guid doctorId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var doctor = await _doctorRepository.ReadAsync(doctorId);
            if (doctor == null)
                return NotFound($"Doctor does not exist.");

            return Ok(doctor);
        }

        [HttpPost(Name = "createDoctor")]
        public async Task<ActionResult<Doctor>> Add(Doctor newDoctor)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var doctor = await _doctorRepository.InsertAsync(newDoctor);
            return CreatedAtRoute("readDoctor", new { doctorId = doctor.Id }, doctor);
        }

        [HttpPut("{doctorId}", Name = "updateDoctor")]
        public async Task<ActionResult> Update(Guid doctorId, Doctor updatedDoctor)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var doctor = await _doctorRepository.ReadAsync(doctorId);
            if (doctor == null)
                return NotFound($"Doctor does not exist.");

            updatedDoctor.Id = doctorId;
            await _doctorRepository.UpdateAsync(updatedDoctor);

            return Ok(updatedDoctor);
        }

        [HttpDelete("{doctorId}", Name = "deleteDoctor")]
        public async Task<ActionResult> Delete(Guid doctorId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var doctor = await _doctorRepository.ReadAsync(doctorId);
            if (doctor == null)
                return NotFound($"Doctor does not exist.");

            await _doctorRepository.DeleteAsync(doctorId);
            return Ok();
        }
    }
}
