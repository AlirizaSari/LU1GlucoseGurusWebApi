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
    public class TrajectControllerTests
    {
        private Mock<ITrajectRepository> _trajectRepositoryMock = null!;
        private Mock<IAuthenticationService> _authenticationServiceMock = null!;
        private Mock<ILogger<TrajectController>> _loggerMock = null!;
        private TrajectController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _trajectRepositoryMock = new Mock<ITrajectRepository>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _loggerMock = new Mock<ILogger<TrajectController>>();
            _controller = new TrajectController(
                _trajectRepositoryMock.Object,
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
        public async Task Get_ReturnsEmptyList_WhenUserHasNoTrajects()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectRepositoryMock.Setup(x => x.ReadAllAsync()).ReturnsAsync(new List<Traject>());

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var trajects = okResult.Value as IEnumerable<Traject>;
            Assert.IsNotNull(trajects);
            Assert.AreEqual(0, trajects.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsTrajects_WhenUserHasTrajects()
        {
            // Arrange
            var trajects = new List<Traject>
            {
                new Traject { Id = Guid.NewGuid(), Name = "Traject 1" },
                new Traject { Id = Guid.NewGuid(), Name = "Traject 2" }
            };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectRepositoryMock.Setup(x => x.ReadAllAsync()).ReturnsAsync(trajects);

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedTrajects = okResult.Value as IEnumerable<Traject>;
            Assert.IsNotNull(returnedTrajects);
            Assert.AreEqual(2, returnedTrajects.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenTrajectNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Traject?)null);

            // Act
            var result = await _controller.Get(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Get_ReturnsOk_WhenTrajectRetrievedSuccessfully()
        {
            // Arrange
            var trajectId = Guid.NewGuid();
            var traject = new Traject { Id = trajectId, Name = "Traject 1" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectRepositoryMock.Setup(x => x.ReadAsync(trajectId)).ReturnsAsync(traject);

            // Act
            var result = await _controller.Get(trajectId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedTraject = okResult.Value as Traject;
            Assert.IsNotNull(returnedTraject);
            Assert.AreEqual(trajectId, returnedTraject.Id);
        }

        [TestMethod]
        public async Task Add_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Add(new Traject());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Add_ReturnsCreatedAtRoute_WhenTrajectCreatedSuccessfully()
        {
            // Arrange
            var newTraject = new Traject { Name = "New Traject" };
            var createdTraject = new Traject { Id = Guid.NewGuid(), Name = "New Traject" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<Traject>())).ReturnsAsync(createdTraject);

            // Act
            var result = await _controller.Add(newTraject);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtRouteResult));
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRouteResult);
            Assert.AreEqual("readTraject", createdAtRouteResult.RouteName);
            Assert.AreEqual(createdTraject.Id, createdAtRouteResult.RouteValues["trajectId"]);
            Assert.AreEqual(createdTraject, createdAtRouteResult.Value);
        }

        [TestMethod]
        public async Task Update_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new Traject());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenTrajectNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Traject?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new Traject());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsOk_WhenTrajectUpdatedSuccessfully()
        {
            // Arrange
            var trajectId = Guid.NewGuid();
            var updatedTraject = new Traject { Name = "Updated Traject" };
            var existingTraject = new Traject { Id = trajectId, Name = "Existing Traject" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectRepositoryMock.Setup(x => x.ReadAsync(trajectId)).ReturnsAsync(existingTraject);
            _trajectRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Traject>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(trajectId, updatedTraject);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedTraject = okResult.Value as Traject;
            Assert.IsNotNull(returnedTraject);
            Assert.AreEqual(trajectId, returnedTraject.Id);
            Assert.AreEqual("Updated Traject", returnedTraject.Name);
        }

        [TestMethod]
        public async Task Delete_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Delete(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenTrajectNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Traject?)null);

            // Act
            var result = await _controller.Delete(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNoContent_WhenTrajectDeletedSuccessfully()
        {
            // Arrange
            var trajectId = Guid.NewGuid();
            var existingTraject = new Traject { Id = trajectId, Name = "Existing Traject" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _trajectRepositoryMock.Setup(x => x.ReadAsync(trajectId)).ReturnsAsync(existingTraject);
            _trajectRepositoryMock.Setup(x => x.DeleteAsync(trajectId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(trajectId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}
