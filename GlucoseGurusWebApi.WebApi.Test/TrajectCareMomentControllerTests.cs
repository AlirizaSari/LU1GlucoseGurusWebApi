using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlucoseGurusWebApi.WebApi.Controllers;
using GlucoseGurusWebApi.WebApi.Models;
using GlucoseGurusWebApi.WebApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace GlucoseGurusWebApi.WebApi.Test
{
    [TestClass]
    public class TrajectCareMomentControllerTests
    {
        private Mock<ITrajectCareMomentRepository> _trajectCareMomentRepositoryMock = null!;
        private Mock<ITrajectRepository> _trajectRepositoryMock = null!;
        private Mock<ICareMomentRepository> _careMomentRepositoryMock = null!;
        private Mock<IAuthenticationService> _authenticationServiceMock = null!;
        private Mock<ILogger<TrajectCareMomentController>> _loggerMock = null!;
        private TrajectCareMomentController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _trajectCareMomentRepositoryMock = new Mock<ITrajectCareMomentRepository>();
            _trajectRepositoryMock = new Mock<ITrajectRepository>();
            _careMomentRepositoryMock = new Mock<ICareMomentRepository>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _loggerMock = new Mock<ILogger<TrajectCareMomentController>>();
            _controller = new TrajectCareMomentController(
                _trajectCareMomentRepositoryMock.Object,
                _trajectRepositoryMock.Object,
                _careMomentRepositoryMock.Object,
                _authenticationServiceMock.Object,
                _loggerMock.Object);
        }

        [TestMethod]
        public async Task Get_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Get_ReturnsEmptyList_WhenUserHasNoTrajectCareMoments()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectCareMomentRepositoryMock.Setup(x => x.ReadAllAsync()).ReturnsAsync(new List<TrajectCareMoment>());

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var trajectCareMoments = okResult.Value as IEnumerable<TrajectCareMoment>;
            Assert.IsNotNull(trajectCareMoments);
            Assert.AreEqual(0, trajectCareMoments.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsTrajectCareMoments_WhenUserHasTrajectCareMoments()
        {
            // Arrange
            var trajectCareMoments = new List<TrajectCareMoment>
            {
                new TrajectCareMoment { TrajectId = Guid.NewGuid(), CareMomentId = Guid.NewGuid(), Name = "TrajectCareMoment 1", Step = 1, IsCompleted = false },
                new TrajectCareMoment { TrajectId = Guid.NewGuid(), CareMomentId = Guid.NewGuid(), Name = "TrajectCareMoment 2", Step = 2, IsCompleted = true }
            };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectCareMomentRepositoryMock.Setup(x => x.ReadAllAsync()).ReturnsAsync(trajectCareMoments);

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedTrajectCareMoments = okResult.Value as IEnumerable<TrajectCareMoment>;
            Assert.IsNotNull(returnedTrajectCareMoments);
            Assert.AreEqual(2, returnedTrajectCareMoments.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenTrajectCareMomentNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectCareMomentRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((TrajectCareMoment?)null);

            // Act
            var result = await _controller.Get(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Get_ReturnsOk_WhenTrajectCareMomentRetrievedSuccessfully()
        {
            // Arrange
            var trajectId = Guid.NewGuid();
            var careMomentId = Guid.NewGuid();
            var trajectCareMoment = new TrajectCareMoment { TrajectId = trajectId, CareMomentId = careMomentId, Name = "TrajectCareMoment 1", Step = 1, IsCompleted = false };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectCareMomentRepositoryMock.Setup(x => x.ReadAsync(trajectId, careMomentId)).ReturnsAsync(trajectCareMoment);

            // Act
            var result = await _controller.Get(trajectId, careMomentId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedTrajectCareMoment = okResult.Value as TrajectCareMoment;
            Assert.IsNotNull(returnedTrajectCareMoment);
            Assert.AreEqual(trajectId, returnedTrajectCareMoment.TrajectId);
            Assert.AreEqual(careMomentId, returnedTrajectCareMoment.CareMomentId);
        }

        [TestMethod]
        public async Task Add_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Add(new TrajectCareMoment());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Add_ReturnsNotFound_WhenTrajectNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Traject?)null);

            // Act
            var result = await _controller.Add(new TrajectCareMoment { TrajectId = Guid.NewGuid() });

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Add_ReturnsNotFound_WhenCareMomentNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new Traject());
            _careMomentRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((CareMoment?)null);

            // Act
            var result = await _controller.Add(new TrajectCareMoment { TrajectId = Guid.NewGuid(), CareMomentId = Guid.NewGuid() });

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Add_ReturnsCreatedAtRoute_WhenTrajectCareMomentCreatedSuccessfully()
        {
            // Arrange
            var trajectId = Guid.NewGuid();
            var careMomentId = Guid.NewGuid();
            var newTrajectCareMoment = new TrajectCareMoment { TrajectId = trajectId, CareMomentId = careMomentId, Name = "New TrajectCareMoment", Step = 1, IsCompleted = false };
            var createdTrajectCareMoment = new TrajectCareMoment { TrajectId = trajectId, CareMomentId = careMomentId, Name = "New TrajectCareMoment", Step = 1, IsCompleted = false };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectRepositoryMock.Setup(x => x.ReadAsync(trajectId)).ReturnsAsync(new Traject { Id = trajectId });
            _careMomentRepositoryMock.Setup(x => x.ReadAsync(careMomentId)).ReturnsAsync(new CareMoment { Id = careMomentId });
            _trajectCareMomentRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<TrajectCareMoment>())).ReturnsAsync(createdTrajectCareMoment);

            // Act
            var result = await _controller.Add(newTrajectCareMoment);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtRouteResult));
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRouteResult);
            Assert.AreEqual("readTrajectCareMoment", createdAtRouteResult.RouteName);
            Assert.AreEqual(createdTrajectCareMoment.TrajectId, createdAtRouteResult.RouteValues["trajectId"]);
            Assert.AreEqual(createdTrajectCareMoment.CareMomentId, createdAtRouteResult.RouteValues["careMomentId"]);
            Assert.AreEqual(createdTrajectCareMoment, createdAtRouteResult.Value);
        }

        [TestMethod]
        public async Task Update_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), Guid.NewGuid(), new TrajectCareMoment());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenTrajectCareMomentNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectCareMomentRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((TrajectCareMoment?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), Guid.NewGuid(), new TrajectCareMoment());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsOk_WhenTrajectCareMomentUpdatedSuccessfully()
        {
            // Arrange
            var trajectId = Guid.NewGuid();
            var careMomentId = Guid.NewGuid();
            var updatedTrajectCareMoment = new TrajectCareMoment { TrajectId = trajectId, CareMomentId = careMomentId, Name = "Updated TrajectCareMoment", Step = 2, IsCompleted = true };
            var existingTrajectCareMoment = new TrajectCareMoment { TrajectId = trajectId, CareMomentId = careMomentId, Name = "Existing TrajectCareMoment", Step = 1, IsCompleted = false };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectCareMomentRepositoryMock.Setup(x => x.ReadAsync(trajectId, careMomentId)).ReturnsAsync(existingTrajectCareMoment);
            _trajectCareMomentRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TrajectCareMoment>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(trajectId, careMomentId, updatedTrajectCareMoment);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedTrajectCareMoment = okResult.Value as TrajectCareMoment;
            Assert.IsNotNull(returnedTrajectCareMoment);
            Assert.AreEqual(trajectId, returnedTrajectCareMoment.TrajectId);
            Assert.AreEqual(careMomentId, returnedTrajectCareMoment.CareMomentId);
            Assert.AreEqual("Updated TrajectCareMoment", returnedTrajectCareMoment.Name);
            Assert.AreEqual(2, returnedTrajectCareMoment.Step);
            Assert.IsTrue(returnedTrajectCareMoment.IsCompleted);
        }

        [TestMethod]
        public async Task Delete_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Delete(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenTrajectCareMomentNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectCareMomentRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((TrajectCareMoment?)null);

            // Act
            var result = await _controller.Delete(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNoContent_WhenTrajectCareMomentDeletedSuccessfully()
        {
            // Arrange
            var trajectId = Guid.NewGuid();
            var careMomentId = Guid.NewGuid();
            var existingTrajectCareMoment = new TrajectCareMoment { TrajectId = trajectId, CareMomentId = careMomentId, Name = "Existing TrajectCareMoment", Step = 1, IsCompleted = false };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectCareMomentRepositoryMock.Setup(x => x.ReadAsync(trajectId, careMomentId)).ReturnsAsync(existingTrajectCareMoment);
            _trajectCareMomentRepositoryMock.Setup(x => x.DeleteAsync(trajectId, careMomentId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(trajectId, careMomentId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}
