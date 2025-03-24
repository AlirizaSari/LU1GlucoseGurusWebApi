using GlucoseGurusWebApi.WebApi.Models;
using GlucoseGurusWebApi.WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace GlucoseGurusWebApi.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("parentGuardians")]
    public class ParentGuardianController : ControllerBase
    {
        private readonly IParentGuardianRepository _parentGuardianRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<ParentGuardianController> _logger;

        public ParentGuardianController(IParentGuardianRepository parentGuardianRepository, IAuthenticationService authenticationService, ILogger<ParentGuardianController> logger)
        {
            _parentGuardianRepository = parentGuardianRepository;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet(Name = "readParentGuardians")]
        public async Task<ActionResult<IEnumerable<ParentGuardian>>> Get()
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var parentGuardians = await _parentGuardianRepository.ReadAllByUserIdAsync(userId);

            return Ok(parentGuardians);
        }

        [HttpGet("{parentGuardianId}", Name = "readParentGuardian")]
        public async Task<ActionResult<ParentGuardian>> Get(Guid parentGuardianId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var parentGuardian = await _parentGuardianRepository.ReadAsync(parentGuardianId);
            if (parentGuardian == null || parentGuardian.UserId != userId)
                return NotFound($"ParentGuardian does not belong to the current user.");

            return Ok(parentGuardian);
        }

        [HttpPost(Name = "createParentGuardian")]
        public async Task<ActionResult<ParentGuardian>> Add(ParentGuardian newParentGuardian)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var userParentGuardians = await _parentGuardianRepository.ReadAllByUserIdAsync(userId);
            if (userParentGuardians.Count() >= ParentGuardian.MaxNumberOfParentGuardians)
                return BadRequest($"Maximum number of parent guardians reached.");

            newParentGuardian.UserId = userId;

            var createdParentGuardian = await _parentGuardianRepository.InsertAsync(newParentGuardian);
            return CreatedAtRoute("readParentGuardian", new { parentGuardianId = createdParentGuardian.Id }, createdParentGuardian);
        }

        [HttpPut("{parentGuardianId}", Name = "updateParentGuardian")]
        public async Task<ActionResult> Update(Guid parentGuardianId, ParentGuardian newParentGuardian)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var existingParentGuardian = await _parentGuardianRepository.ReadAsync(parentGuardianId);

            if (existingParentGuardian == null || existingParentGuardian.UserId != userId)
                return NotFound($"ParentGuardian does not belong to the current user.");

            newParentGuardian.Id = parentGuardianId;
            newParentGuardian.UserId = userId;
            await _parentGuardianRepository.UpdateAsync(newParentGuardian);

            return Ok(newParentGuardian);
        }

        [HttpDelete("{parentGuardianId}", Name = "deleteParentGuardian")]
        public async Task<ActionResult> Delete(Guid parentGuardianId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var existingParentGuardian = await _parentGuardianRepository.ReadAsync(parentGuardianId);
            if (existingParentGuardian == null || existingParentGuardian.UserId != userId)
                return NotFound($"ParentGuardian does not belong to the current user.");

            await _parentGuardianRepository.DeleteAsync(parentGuardianId);

            return Ok();
        }
    }
}
