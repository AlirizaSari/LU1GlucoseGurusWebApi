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
            _controller = new ParentGuardianController(_parentGuardianRepositoryMock.Object, _authenticationServiceMock.Object, _loggerMock.Object);
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
        public async Task Add_ReturnsBadRequest_WhenUserHasMaxNumberOfParentGuardians()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAllByUserIdAsync("userId")).ReturnsAsync(new List<ParentGuardian> { new ParentGuardian() });

            // Act
            var result = await _controller.Add(new ParentGuardian());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Add_ReturnsCreated_WhenParentGuardianCreated()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var parentGuardian = new ParentGuardian { UserId = userId, FirstName = "John", LastName = "Doe" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);
            _parentGuardianRepositoryMock.Setup(x => x.ReadAllByUserIdAsync(userId)).ReturnsAsync(new List<ParentGuardian>());
            _parentGuardianRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<ParentGuardian>())).ReturnsAsync(parentGuardian);

            // Act
            var result = await _controller.Add(parentGuardian);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtRouteResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNoContent_WhenParentGuardianDeleted()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var parentGuardianId = Guid.NewGuid();
            var parentGuardian = new ParentGuardian { UserId = userId, FirstName = "John", LastName = "Doe" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(parentGuardianId)).ReturnsAsync(parentGuardian);
            _parentGuardianRepositoryMock.Setup(x => x.DeleteAsync(parentGuardianId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(parentGuardianId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenParentGuardianNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var parentGuardianId = Guid.NewGuid();

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(parentGuardianId)).ReturnsAsync((ParentGuardian?)null);

            // Act
            var result = await _controller.Delete(parentGuardianId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("ParentGuardian does not belong to the current user.", notFoundResult.Value);
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
            var userId = Guid.NewGuid().ToString();
            var parentGuardianId = Guid.NewGuid();

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(parentGuardianId)).ReturnsAsync((ParentGuardian?)null);

            // Act
            var result = await _controller.Update(parentGuardianId, new ParentGuardian());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("ParentGuardian does not belong to the current user.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task Update_ReturnsOk_WhenParentGuardianUpdated()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var parentGuardianId = Guid.NewGuid();
            var parentGuardian = new ParentGuardian { UserId = userId, FirstName = "John", LastName = "Doe" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(parentGuardianId)).ReturnsAsync(parentGuardian);
            _parentGuardianRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<ParentGuardian>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(parentGuardianId, new ParentGuardian());

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

    }
}
