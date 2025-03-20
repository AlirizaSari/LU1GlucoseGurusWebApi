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
        private readonly IParentGuardianRepository _parentGuardianRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IAuthenticationService _authenticationService;

        public NoteController(ILogger<NoteController> logger, INoteRepository NoteRepository, IParentGuardianRepository parentGuardianRepository, IPatientRepository patientRepository, IAuthenticationService authenticationService)
        {
            _logger = logger;
            _NoteRepository = NoteRepository;
            _parentGuardianRepository = parentGuardianRepository;
            _patientRepository = patientRepository;
            _authenticationService = authenticationService;
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

        [HttpPost(Name = "createNote")]
        public async Task<ActionResult<Note>> Add(Note newNote)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var parentGuardian = await _parentGuardianRepository.ReadAsync(newNote.ParentGuardianId);
            if (parentGuardian == null || parentGuardian.UserId != userId)
                return BadRequest($"ParentGuardian does not belong to the current user.");

            var patient = await _patientRepository.ReadAsync(newNote.PatientId);
            if (patient == null || patient.ParentGuardianId != parentGuardian.Id)
                return BadRequest($"Patient does not belong to the parent guardian.");

            newNote.ParentGuardianId = parentGuardian.Id;
            newNote.PatientId = patient.Id;

            var Note = await _NoteRepository.InsertAsync(newNote);
            return CreatedAtRoute("ReadNote", new { NoteId = Note.Id }, Note);
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

            updatedNote.Id = NoteId;
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
            return Ok();
        }
    }
}
