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

namespace GlucoseGurusWebApi.Test
{
    [TestClass]
    public class DoctorControllerTests
    {
        private Mock<IDoctorRepository> _doctorRepositoryMock;
        private Mock<IAuthenticationService> _authenticationServiceMock;
        private Mock<ILogger<DoctorController>> _loggerMock;
        private DoctorController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _doctorRepositoryMock = new Mock<IDoctorRepository>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _loggerMock = new Mock<ILogger<DoctorController>>();
            _controller = new DoctorController(_doctorRepositoryMock.Object, _authenticationServiceMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Add_ReturnsUnauthorized_WhenUserNotAutenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Add(new Doctor());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }
    }
}
