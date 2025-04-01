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
    public class ParentGuardianControllerTests
    {
        private Mock<IParentGuardianRepository> _parentGuardianRepositoryMock = null!;
        private Mock<IAuthenticationService> _authenticationServiceMock = null!;
        private Mock<ILogger<ParentGuardianController>> _loggerMock = null!;
        private ParentGuardianController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _parentGuardianRepositoryMock = new Mock<IParentGuardianRepository>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _loggerMock = new Mock<ILogger<ParentGuardianController>>();
            _controller = new ParentGuardianController(
                _parentGuardianRepositoryMock.Object,
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
        public async Task Get_ReturnsEmptyList_WhenUserHasNoParentGuardians()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAllByUserIdAsync("userId")).ReturnsAsync(new List<ParentGuardian>());

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var parentGuardians = okResult.Value as IEnumerable<ParentGuardian>;
            Assert.IsNotNull(parentGuardians);
            Assert.AreEqual(0, parentGuardians.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsParentGuardians_WhenUserHasParentGuardians()
        {
            // Arrange
            var parentGuardians = new List<ParentGuardian>
            {
                new ParentGuardian { Id = Guid.NewGuid(), UserId = "userId", FirstName = "John", LastName = "Doe" },
                new ParentGuardian { Id = Guid.NewGuid(), UserId = "userId", FirstName = "Jane", LastName = "Doe" }
            };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAllByUserIdAsync("userId")).ReturnsAsync(parentGuardians);

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedParentGuardians = okResult.Value as IEnumerable<ParentGuardian>;
            Assert.IsNotNull(returnedParentGuardians);
            Assert.AreEqual(2, returnedParentGuardians.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenParentGuardianNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((ParentGuardian?)null);

            // Act
            var result = await _controller.Get(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenParentGuardianDoesNotBelongToUser()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "differentUserId" });

            // Act
            var result = await _controller.Get(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Get_ReturnsOk_WhenParentGuardianRetrievedSuccessfully()
        {
            // Arrange
            var parentGuardianId = Guid.NewGuid();
            var parentGuardian = new ParentGuardian { Id = parentGuardianId, UserId = "userId", FirstName = "John", LastName = "Doe" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(parentGuardianId)).ReturnsAsync(parentGuardian);

            // Act
            var result = await _controller.Get(parentGuardianId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedParentGuardian = okResult.Value as ParentGuardian;
            Assert.IsNotNull(returnedParentGuardian);
            Assert.AreEqual(parentGuardianId, returnedParentGuardian.Id);
        }

        [TestMethod]
        public async Task Add_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Add(new ParentGuardian());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Add_ReturnsBadRequest_WhenMaxNumberOfParentGuardiansReached()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAllByUserIdAsync("userId")).ReturnsAsync(new List<ParentGuardian> { new ParentGuardian() });

            // Act
            var result = await _controller.Add(new ParentGuardian());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Maximum number of parent guardians reached.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task Add_ReturnsCreatedAtRoute_WhenParentGuardianCreatedSuccessfully()
        {
            // Arrange
            var newParentGuardian = new ParentGuardian { FirstName = "John", LastName = "Doe" };
            var createdParentGuardian = new ParentGuardian { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", UserId = "userId" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAllByUserIdAsync("userId")).ReturnsAsync(new List<ParentGuardian>());
            _parentGuardianRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<ParentGuardian>())).ReturnsAsync(createdParentGuardian);

            // Act
            var result = await _controller.Add(newParentGuardian);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtRouteResult));
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRouteResult);
            Assert.AreEqual("readParentGuardian", createdAtRouteResult.RouteName);
            Assert.AreEqual(createdParentGuardian.Id, createdAtRouteResult.RouteValues["parentGuardianId"]);
            Assert.AreEqual(createdParentGuardian, createdAtRouteResult.Value);
        }

        [TestMethod]
        public async Task Update_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new ParentGuardian());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenParentGuardianNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((ParentGuardian?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new ParentGuardian());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenParentGuardianDoesNotBelongToUser()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "differentUserId" });

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new ParentGuardian());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsOk_WhenParentGuardianUpdatedSuccessfully()
        {
            // Arrange
            var parentGuardianId = Guid.NewGuid();
            var updatedParentGuardian = new ParentGuardian { FirstName = "John", LastName = "Doe" };
            var existingParentGuardian = new ParentGuardian { Id = parentGuardianId, UserId = "userId", FirstName = "Jane", LastName = "Doe" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(parentGuardianId)).ReturnsAsync(existingParentGuardian);
            _parentGuardianRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<ParentGuardian>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(parentGuardianId, updatedParentGuardian);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedParentGuardian = okResult.Value as ParentGuardian;
            Assert.IsNotNull(returnedParentGuardian);
            Assert.AreEqual(parentGuardianId, returnedParentGuardian.Id);
            Assert.AreEqual("userId", returnedParentGuardian.UserId);
            Assert.AreEqual("John", returnedParentGuardian.FirstName);
            Assert.AreEqual("Doe", returnedParentGuardian.LastName);
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
        public async Task Delete_ReturnsNotFound_WhenParentGuardianNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((ParentGuardian?)null);

            // Act
            var result = await _controller.Delete(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenParentGuardianDoesNotBelongToUser()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "differentUserId" });

            // Act
            var result = await _controller.Delete(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNoContent_WhenParentGuardianDeletedSuccessfully()
        {
            // Arrange
            var parentGuardianId = Guid.NewGuid();
            var existingParentGuardian = new ParentGuardian { Id = parentGuardianId, UserId = "userId" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(parentGuardianId)).ReturnsAsync(existingParentGuardian);
            _parentGuardianRepositoryMock.Setup(x => x.DeleteAsync(parentGuardianId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(parentGuardianId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}
