using GlucoseGurusWebApi.WebApi.Models;
using GlucoseGurusWebApi.WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlucoseGurusWebApi.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("trajects")]
    public class TrajectController : ControllerBase
    {
        private readonly ITrajectRepository _trajectRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<TrajectController> _logger;

        public TrajectController(ITrajectRepository trajectRepository, IAuthenticationService authenticationService, ILogger<TrajectController> logger)
        {
            _trajectRepository = trajectRepository;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet(Name = "readTrajects")]
        public async Task<ActionResult<IEnumerable<Traject>>> Get()
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var trajects = await _trajectRepository.ReadAllAsync();
            return Ok(trajects);
        }

        [HttpGet("{trajectId}", Name = "readTraject")]
        public async Task<ActionResult<Traject>> Get(Guid trajectId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var traject = await _trajectRepository.ReadAsync(trajectId);
            if (traject == null)
                return NotFound($"Traject does not exist.");

            return Ok(traject);
        }

        [HttpPost(Name = "createTraject")]
        public async Task<ActionResult<Traject>> Add(Traject newTraject)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var traject = await _trajectRepository.InsertAsync(newTraject);
            return CreatedAtRoute("readTraject", new { trajectId = traject.Id }, traject);
        }

        [HttpPut("{trajectId}", Name = "updateTraject")]
        public async Task<ActionResult> Update(Guid trajectId, Traject updatedTraject)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var traject = await _trajectRepository.ReadAsync(trajectId);
            if (traject == null)
                return NotFound($"Traject does not exist.");

            updatedTraject.Id = trajectId;
            await _trajectRepository.UpdateAsync(updatedTraject);
            return Ok(updatedTraject);
        }

        [HttpDelete("{trajectId}", Name = "deleteTraject")]
        public async Task<ActionResult> Delete(Guid trajectId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var traject = await _trajectRepository.ReadAsync(trajectId);
            if (traject == null)
                return NotFound($"Traject does not exist.");

            await _trajectRepository.DeleteAsync(trajectId);
            return Ok();
        }
    }
}
