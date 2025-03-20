using GlucoseGurusWebApi.WebApi.Models;
using GlucoseGurusWebApi.WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlucoseGurusWebApi.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("ParentGuardians")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IParentGuardianRepository _parentGuardianRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ITrajectRepository _trajectRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IPatientRepository patientRepository, IParentGuardianRepository parentGuardianRepository, IDoctorRepository doctorRepository, ITrajectRepository trajectRepository, IAuthenticationService authenticationService, ILogger<PatientController> logger)
        {
            _patientRepository = patientRepository;
            _parentGuardianRepository = parentGuardianRepository;
            _doctorRepository = doctorRepository;
            _trajectRepository = trajectRepository;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet("Patients", Name = "ReadPatients")]
        public async Task<ActionResult<IEnumerable<Patient>>> Get()
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var patients = await _patientRepository.ReadAllAsync();
            return Ok(patients);
        }

        [HttpGet("Patients/{patientId}", Name = "ReadPatient")]
        public async Task<ActionResult<Patient>> Get(Guid patientId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var patient = await _patientRepository.ReadAsync(patientId);
            if (patient == null)
                return NotFound($"Patient does not exist.");

            var parentGuardian = await _parentGuardianRepository.ReadAsync(patient.ParentGuardianId);
            if (parentGuardian == null || parentGuardian.UserId != userId)
                return NotFound($"Patient does not belong to the current user.");

            return Ok(patient);
        }

        [HttpGet("{parentGuardianId}/Patients", Name = "ReadPatientsByParentGuardian")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetByParentGuardian(Guid parentGuardianId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var parentGuardian = await _parentGuardianRepository.ReadAsync(parentGuardianId);
            if (parentGuardian == null || parentGuardian.UserId != userId)
                return NotFound($"ParentGuardian does not belong to the current user.");

            var patients = await _patientRepository.ReadByParentGuardianAsync(parentGuardianId);
            return Ok(patients);
        }

        [HttpPost("{parentGuardianId}/Patients", Name = "CreatePatient")]
        public async Task<ActionResult<Patient>> Add(Guid parentGuardianId, Guid trajectId, Guid doctorId, Patient newPatient)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var parentGuardian = await _parentGuardianRepository.ReadAsync(parentGuardianId);
            if (parentGuardian == null || parentGuardian.UserId != userId)
                return NotFound($"ParentGuardian does not belong to the current user.");

            var traject = await _trajectRepository.ReadAsync(trajectId);
            if (traject == null)
                return NotFound($"Traject does not exist.");

            var doctor = await _doctorRepository.ReadAsync(doctorId);
            if (doctor == null)
                return NotFound($"Doctor does not exist.");

            newPatient.ParentGuardianId = parentGuardianId;
            newPatient.TrajectId = trajectId;
            newPatient.DoctorId = doctorId;
            var patient = await _patientRepository.InsertAsync(newPatient);
            return CreatedAtRoute("ReadPatient", new { patientId = patient.Id }, patient);
        }

        [HttpPut("{parentGuardianId}/Patients/{patientId}", Name = "UpdatePatient")]
        public async Task<ActionResult> Update(Guid parentGuardianId, Guid trajectId, Guid doctorId, Guid patientId, Patient updatedPatient)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var parentGuardian = await _parentGuardianRepository.ReadAsync(parentGuardianId);
            if (parentGuardian == null || parentGuardian.UserId != userId)
                return NotFound($"ParentGuardian does not belong to the current user.");

            var patient = await _patientRepository.ReadAsync(patientId);
            if (patient == null)
                return NotFound($"Patient does not exist.");

            var traject = await _trajectRepository.ReadAsync(trajectId);
            if (traject == null)
                return NotFound($"Traject does not exist.");

            var doctor = await _doctorRepository.ReadAsync(doctorId);
            if (doctor == null)
                return NotFound($"Doctor does not exist.");

            updatedPatient.Id = patientId;
            updatedPatient.ParentGuardianId = parentGuardianId;
            updatedPatient.TrajectId = trajectId;
            updatedPatient.DoctorId = doctorId;
            await _patientRepository.UpdateAsync(updatedPatient);
            return Ok(updatedPatient);
        }

        [HttpDelete("{parentGuardianId}/Patients/{patientId}", Name = "DeletePatient")]
        public async Task<ActionResult> Delete(Guid parentGuardianId, Guid patientId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var parentGuardian = await _parentGuardianRepository.ReadAsync(parentGuardianId);
            if (parentGuardian == null || parentGuardian.UserId != userId)
                return NotFound($"ParentGuardian does not belong to the current user.");

            var patient = await _patientRepository.ReadAsync(patientId);
            if (patient == null)
                return NotFound($"Patient does not exist.");

            await _patientRepository.DeleteAsync(patientId);
            return Ok();
        }
    }
}
