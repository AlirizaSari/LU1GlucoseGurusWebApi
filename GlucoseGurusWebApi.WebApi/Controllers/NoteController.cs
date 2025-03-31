using GlucoseGurusWebApi.WebApi.Models;
using GlucoseGurusWebApi.WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlucoseGurusWebApi.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("notes")]
    public class NoteController : ControllerBase
    {
        private readonly ILogger<NoteController> _logger;
        private readonly INoteRepository _NoteRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IParentGuardianRepository _parentGuardianRepository;
        private readonly IAuthenticationService _authenticationService;

        public NoteController(INoteRepository NoteRepository, IPatientRepository patientRepository, IParentGuardianRepository parentGuardianRepository, IAuthenticationService authenticationService, ILogger<NoteController> logger)
        {
            _NoteRepository = NoteRepository;
            _patientRepository = patientRepository;
            _parentGuardianRepository = parentGuardianRepository;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet(Name = "readNotes")]
        public async Task<ActionResult<IEnumerable<Note>>> Get()
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var Notes = await _NoteRepository.ReadAllAsync();
            return Ok(Notes);
        }

        [HttpGet("{NoteId}", Name = "readNote")]
        public async Task<ActionResult<Note>> Get(Guid NoteId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var Note = await _NoteRepository.ReadAsync(NoteId);
            if (Note == null)
                return NotFound($"Note does not exist.");

            return Ok(Note);
        }

        [HttpGet("patient/{patientId}", Name = "readNotesByPatient")]
        public async Task<ActionResult<IEnumerable<Note>>> GetByPatient(Guid patientId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var Notes = await _NoteRepository.ReadByPatientAsync(patientId);
            return Ok(Notes);
        }

        [HttpGet("parentGuardian/{parentGuardianId}", Name = "readNotesByParentGuardian")]
        public async Task<ActionResult<IEnumerable<Note>>> GetByParentGuardian(Guid parentGuardianId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var Notes = await _NoteRepository.ReadByParentGuardianAsync(parentGuardianId);
            return Ok(Notes);
        }

        [HttpPost(Name = "createNote")]
        public async Task<ActionResult<Note>> Add(Note newNote)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var parentGuardian = await _parentGuardianRepository.ReadAsync(newNote.ParentGuardianId);
            if (parentGuardian == null || parentGuardian.UserId != userId)
                return NotFound($"ParentGuardian does not belong to the current user.");

            var patient = await _patientRepository.ReadAsync(newNote.PatientId);
            if (patient == null)
                return NotFound($"Patient does not exist.");

            newNote.ParentGuardianId = parentGuardian.Id;
            newNote.PatientId = patient.Id;

            var Note = await _NoteRepository.InsertAsync(newNote);
            return CreatedAtRoute("readNote", new { NoteId = Note.Id }, Note);
        }

        [HttpPut("{NoteId}", Name = "updateNote")]
        public async Task<ActionResult> Update(Guid NoteId, Note updatedNote)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var Note = await _NoteRepository.ReadAsync(NoteId);
            if (Note == null)
                return NotFound($"Note does not exist.");

            var parentGuardian = await _parentGuardianRepository.ReadAsync(updatedNote.ParentGuardianId);
            if (parentGuardian == null || parentGuardian.UserId != userId)
                return NotFound($"ParentGuardian does not belong to the current user.");

            var patient = await _patientRepository.ReadAsync(updatedNote.PatientId);
            if (patient == null)
                return NotFound($"Patient does not exist.");

            updatedNote.Id = NoteId;
            updatedNote.ParentGuardianId = parentGuardian.Id;
            updatedNote.PatientId = patient.Id;
            await _NoteRepository.UpdateAsync(updatedNote);

            return Ok(updatedNote);
        }

        [HttpDelete("{NoteId}", Name = "deleteNote")]
        public async Task<ActionResult> Delete(Guid NoteId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var Note = await _NoteRepository.ReadAsync(NoteId);
            if (Note == null)
                return NotFound($"Note does not exist.");

            await _NoteRepository.DeleteAsync(NoteId);

            return NoContent();
        }
    }
}
