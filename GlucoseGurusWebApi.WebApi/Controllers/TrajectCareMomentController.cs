using GlucoseGurusWebApi.WebApi.Models;
using GlucoseGurusWebApi.WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlucoseGurusWebApi.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("tracjetCareMoments")]
    public class TrajectCareMomentController : Controller
    {
        private readonly ITrajectCareMomentRepository _tracjetCareMomentRepository;
        private readonly ITrajectRepository _trajectRepository;
        private readonly ICareMomentRepository _careMomentRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<TrajectCareMomentController> _logger;

        public TrajectCareMomentController(ITrajectCareMomentRepository tracjetCareMomentRepository, ITrajectRepository trajectRepository, ICareMomentRepository careMomentRepository, IAuthenticationService authenticationService, ILogger<TrajectCareMomentController> logger)
        {
            _tracjetCareMomentRepository = tracjetCareMomentRepository;
            _trajectRepository = trajectRepository;
            _careMomentRepository = careMomentRepository;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet(Name = "readTracjetCareMoments")]
        public async Task<ActionResult<IEnumerable<TracjetCareMoment>>> Get()
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var tracjetCareMoments = await _tracjetCareMomentRepository.ReadAllAsync();
            return Ok(tracjetCareMoments);
        }

        [HttpGet("{tracjetId}/{careMomentId}", Name = "readTracjetCareMoment")]
        public async Task<ActionResult<TracjetCareMoment>> Get(Guid tracjetId, Guid careMomentId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var tracjetCareMoment = await _tracjetCareMomentRepository.ReadAsync(tracjetId, careMomentId);
            if (tracjetCareMoment == null)
                return NotFound("TracjetCareMoment does not exist or one of the related entities is missing.");

            return Ok(tracjetCareMoment);
        }

        [HttpPost(Name = "createTracjetCareMoment")]
        public async Task<ActionResult<TracjetCareMoment>> Add(TracjetCareMoment newTracjetCareMoment)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var tracjet = await _trajectRepository.ReadAsync(newTracjetCareMoment.TrajectId);
            if (tracjet == null)
                return NotFound("Traject does not exist.");

            var careMoment = await _careMomentRepository.ReadAsync(newTracjetCareMoment.CareMomentId);
            if (careMoment == null)
                return NotFound("CareMoment does not exist.");

            var tracjetCareMoment = await _tracjetCareMomentRepository.InsertAsync(newTracjetCareMoment);
            return CreatedAtRoute("readTracjetCareMoment", new { tracjetId = tracjetCareMoment.TrajectId, careMomentId = tracjetCareMoment.CareMomentId }, tracjetCareMoment);
        }

        [HttpPut("{tracjetId}/{careMomentId}", Name = "updateTracjetCareMoment")]
        public async Task<ActionResult<TracjetCareMoment>> Update(Guid tracjetId, Guid careMomentId, TracjetCareMoment updatedTracjetCareMoment)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var tracjetCareMoment = await _tracjetCareMomentRepository.ReadAsync(tracjetId, careMomentId);
            if (tracjetCareMoment == null)
                return NotFound("TracjetCareMoment does not exist or one of the related entities is missing.");

            updatedTracjetCareMoment.TrajectId = tracjetId;
            updatedTracjetCareMoment.CareMomentId = careMomentId;

            await _tracjetCareMomentRepository.UpdateAsync(updatedTracjetCareMoment);
            return Ok(updatedTracjetCareMoment);
        }

        [HttpDelete("{tracjetId}/{careMomentId}", Name = "deleteTracjetCareMoment")]
        public async Task<ActionResult> Delete(Guid tracjetId, Guid careMomentId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var tracjetCareMoment = await _tracjetCareMomentRepository.ReadAsync(tracjetId, careMomentId);
            if (tracjetCareMoment == null)
                return NotFound("TracjetCareMoment does not exist or one of the related entities is missing.");

            await _tracjetCareMomentRepository.DeleteAsync(tracjetId, careMomentId);
            return Ok();
        }
    }
}
