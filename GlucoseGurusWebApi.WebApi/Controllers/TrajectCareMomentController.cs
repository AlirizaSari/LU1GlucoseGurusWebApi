using GlucoseGurusWebApi.WebApi.Models;
using GlucoseGurusWebApi.WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlucoseGurusWebApi.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("trajectCareMoments")]
    public class TrajectCareMomentController : ControllerBase
    {
        private readonly ITrajectCareMomentRepository _trajectCareMomentRepository;
        private readonly ITrajectRepository _trajectRepository;
        private readonly ICareMomentRepository _careMomentRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<TrajectCareMomentController> _logger;

        public TrajectCareMomentController(ITrajectCareMomentRepository trajectCareMomentRepository, ITrajectRepository trajectRepository, ICareMomentRepository careMomentRepository, IAuthenticationService authenticationService, ILogger<TrajectCareMomentController> logger)
        {
            _trajectCareMomentRepository = trajectCareMomentRepository;
            _trajectRepository = trajectRepository;
            _careMomentRepository = careMomentRepository;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet(Name = "readTrajectCareMoments")]
        public async Task<ActionResult<IEnumerable<TrajectCareMoment>>> Get()
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var trajectCareMoments = await _trajectCareMomentRepository.ReadAllAsync();
            return Ok(trajectCareMoments);
        }

        [HttpGet("{trajectId}/{careMomentId}", Name = "readTrajectCareMoment")]
        public ActionResult<TrajectCareMoment> Get(Guid trajectId, Guid careMomentId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var trajectCareMoment = _trajectCareMomentRepository.ReadAsync(trajectId, careMomentId);
            if (trajectCareMoment == null)
                return NotFound($"TrajectCareMoment does not exist.");

            return Ok(trajectCareMoment);
        }

        [HttpPost(Name = "createTrajectCareMoment")]
        public async Task<ActionResult<TrajectCareMoment>> Add(TrajectCareMoment newTrajectCareMoment)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var traject = await _trajectRepository.ReadAsync(newTrajectCareMoment.TrajectId);
            if (traject == null)
                return NotFound($"Traject does not exist.");

            var careMoment = await _careMomentRepository.ReadAsync(newTrajectCareMoment.CareMomentId);
            if (careMoment == null)
                return NotFound($"CareMoment does not exist.");

            newTrajectCareMoment.TrajectId = traject.Id;
            newTrajectCareMoment.CareMomentId = careMoment.Id;

            var trajectCareMoment = await _trajectCareMomentRepository.InsertAsync(newTrajectCareMoment);
            return CreatedAtRoute("readTrajectCareMoment", new { trajectId = trajectCareMoment.TrajectId, careMomentId = trajectCareMoment.CareMomentId }, trajectCareMoment);
        }

        [HttpPut("{trajectId}/{careMomentId}", Name = "updateTrajectCareMoment")]
        public async Task<ActionResult> Update(Guid trajectId, Guid careMomentId, TrajectCareMoment updatedTrajectCareMoment)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var trajectCareMoment = await _trajectCareMomentRepository.ReadAsync(trajectId, careMomentId);
            if (trajectCareMoment == null)
                return NotFound($"TrajectCareMoment does not exist.");

            updatedTrajectCareMoment.TrajectId = trajectId;
            updatedTrajectCareMoment.CareMomentId = careMomentId;

            await _trajectCareMomentRepository.UpdateAsync(updatedTrajectCareMoment);

            return Ok(updatedTrajectCareMoment);
        }

        [HttpDelete("{trajectId}/{careMomentId}", Name = "deleteTrajectCareMoment")]
        public async Task<ActionResult> Delete(Guid trajectId, Guid careMomentId)
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
                return Unauthorized();

            var trajectCareMoment = await _trajectCareMomentRepository.ReadAsync(trajectId, careMomentId);
            if (trajectCareMoment == null)
                return NotFound($"TrajectCareMoment does not exist.");

            await _trajectCareMomentRepository.DeleteAsync(trajectId, careMomentId);
            return NoContent();
        }

    }
}
