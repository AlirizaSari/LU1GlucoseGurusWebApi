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
    public class CareMomentControllerTests
    {
        private Mock<ICareMomentRepository> _careMomentRepositoryMock = null!;
        private Mock<IAuthenticationService> _authenticationServiceMock = null!;
        private Mock<ILogger<CareMomentController>> _loggerMock = null!;
        private CareMomentController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _careMomentRepositoryMock = new Mock<ICareMomentRepository>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _loggerMock = new Mock<ILogger<CareMomentController>>();
            _controller = new CareMomentController(
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
        public async Task Get_ReturnsEmptyList_WhenUserHasNoCareMoments()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _careMomentRepositoryMock.Setup(x => x.ReadAllAsync()).ReturnsAsync(new List<CareMoment>());

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var careMoments = okResult.Value as IEnumerable<CareMoment>;
            Assert.IsNotNull(careMoments);
            Assert.AreEqual(0, careMoments.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsCareMoments_WhenUserHasCareMoments()
        {
            // Arrange
            var careMoments = new List<CareMoment>
            {
                new CareMoment { Id = Guid.NewGuid(), Name = "CareMoment 1" },
                new CareMoment { Id = Guid.NewGuid(), Name = "CareMoment 2" }
            };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _careMomentRepositoryMock.Setup(x => x.ReadAllAsync()).ReturnsAsync(careMoments);

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedCareMoments = okResult.Value as IEnumerable<CareMoment>;
            Assert.IsNotNull(returnedCareMoments);
            Assert.AreEqual(2, returnedCareMoments.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenCareMomentNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _careMomentRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((CareMoment?)null);

            // Act
            var result = await _controller.Get(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Get_ReturnsOk_WhenCareMomentRetrievedSuccessfully()
        {
            // Arrange
            var careMomentId = Guid.NewGuid();
            var careMoment = new CareMoment { Id = careMomentId, Name = "CareMoment 1" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _careMomentRepositoryMock.Setup(x => x.ReadAsync(careMomentId)).ReturnsAsync(careMoment);

            // Act
            var result = await _controller.Get(careMomentId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedCareMoment = okResult.Value as CareMoment;
            Assert.IsNotNull(returnedCareMoment);
            Assert.AreEqual(careMomentId, returnedCareMoment.Id);
        }

        [TestMethod]
        public async Task Add_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Add(new CareMoment());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Add_ReturnsCreatedAtRoute_WhenCareMomentCreatedSuccessfully()
        {
            // Arrange
            var newCareMoment = new CareMoment { Name = "New CareMoment" };
            var createdCareMoment = new CareMoment { Id = Guid.NewGuid(), Name = "New CareMoment" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _careMomentRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<CareMoment>())).ReturnsAsync(createdCareMoment);

            // Act
            var result = await _controller.Add(newCareMoment);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtRouteResult));
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRouteResult);
            Assert.AreEqual("readCareMoment", createdAtRouteResult.RouteName);
            Assert.AreEqual(createdCareMoment.Id, createdAtRouteResult.RouteValues["careMomentId"]);
            Assert.AreEqual(createdCareMoment, createdAtRouteResult.Value);
        }

        [TestMethod]
        public async Task Update_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new CareMoment());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenCareMomentNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _careMomentRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((CareMoment?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new CareMoment());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsOk_WhenCareMomentUpdatedSuccessfully()
        {
            // Arrange
            var careMomentId = Guid.NewGuid();
            var updatedCareMoment = new CareMoment { Name = "Updated CareMoment" };
            var existingCareMoment = new CareMoment { Id = careMomentId, Name = "Existing CareMoment" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _careMomentRepositoryMock.Setup(x => x.ReadAsync(careMomentId)).ReturnsAsync(existingCareMoment);
            _careMomentRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<CareMoment>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(careMomentId, updatedCareMoment);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedCareMoment = okResult.Value as CareMoment;
            Assert.IsNotNull(returnedCareMoment);
            Assert.AreEqual(careMomentId, returnedCareMoment.Id);
            Assert.AreEqual("Updated CareMoment", returnedCareMoment.Name);
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
        public async Task Delete_ReturnsNotFound_WhenCareMomentNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _careMomentRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((CareMoment?)null);

            // Act
            var result = await _controller.Delete(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNoContent_WhenCareMomentDeletedSuccessfully()
        {
            // Arrange
            var careMomentId = Guid.NewGuid();
            var existingCareMoment = new CareMoment { Id = careMomentId, Name = "Existing CareMoment" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _careMomentRepositoryMock.Setup(x => x.ReadAsync(careMomentId)).ReturnsAsync(existingCareMoment);
            _careMomentRepositoryMock.Setup(x => x.DeleteAsync(careMomentId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(careMomentId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}
