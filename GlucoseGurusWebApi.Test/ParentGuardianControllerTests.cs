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

namespace GlucoseGurusWebApi.Test
{
    [TestClass]
    public class ParentGuardianControllerTests
    {
        private Mock<IParentGuardianRepository> _parentGuardianRepositoryMock;
        private Mock<IAuthenticationService> _authenticationServiceMock;
        private Mock<ILogger<ParentGuardianController>> _loggerMock;
        private ParentGuardianController _controller;

        [TestInitialize]
        public void Setup()
        {
            _parentGuardianRepositoryMock = new Mock<IParentGuardianRepository>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _loggerMock = new Mock<ILogger<ParentGuardianController>>();
            _controller = new ParentGuardianController(_parentGuardianRepositoryMock.Object, _authenticationServiceMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Add_ReturnsUnauthorized_WhenUserNotAutenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Add(new ParentGuardian());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Add_ReturnsBadRequest_WhenMaxNumberOfParentGuardiansReached()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAllByUserIdAsync("userId")).ReturnsAsync(Enumerable.Range(0, ParentGuardian.MaxNumberOfParentGuardians).Select(x => new ParentGuardian()));

            // Act
            var result = await _controller.Add(new ParentGuardian());

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Add_ReturnsCreated_WhenParentGuardianAdded()
        {
            // Arrange
            var userId = "userId";
            var parentGuardian = new ParentGuardian { FirstName = "John", LastName = "Doe" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);
            _parentGuardianRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<ParentGuardian>())).ReturnsAsync(new ParentGuardian());

            // Act
            var result = await _controller.Add(new ParentGuardian());

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNoContent_WhenParentGuardianDeleted()
        {
            // Arrange
            var userId = "userId";
            var parentGuardianId = Guid.NewGuid();
            var parentGuardian = new ParentGuardian { Id = parentGuardianId, UserId = userId, FirstName = "John", LastName = "Doe"  };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(parentGuardianId)).ReturnsAsync(parentGuardian);

            // Act
            var result = await _controller.Delete(parentGuardianId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenParentGuardianNotFound()
        {
            // Arrange
            var userId = "userId";
            var parentGuardianId = Guid.NewGuid();
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(parentGuardianId)).ReturnsAsync((ParentGuardian?)null);

            // Act
            var result = await _controller.Delete(parentGuardianId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

    }
}
