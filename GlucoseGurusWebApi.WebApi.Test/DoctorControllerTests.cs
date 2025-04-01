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
    public class DoctorControllerTests
    {
        private Mock<IDoctorRepository> _doctorRepositoryMock = null!;
        private Mock<IAuthenticationService> _authenticationServiceMock = null!;
        private Mock<ILogger<DoctorController>> _loggerMock = null!;
        private DoctorController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _doctorRepositoryMock = new Mock<IDoctorRepository>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _loggerMock = new Mock<ILogger<DoctorController>>();
            _controller = new DoctorController(
                _doctorRepositoryMock.Object,
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
        public async Task Get_ReturnsEmptyList_WhenUserHasNoDoctors()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _doctorRepositoryMock.Setup(x => x.ReadAllAsync()).ReturnsAsync(new List<Doctor>());

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var doctors = okResult.Value as IEnumerable<Doctor>;
            Assert.IsNotNull(doctors);
            Assert.AreEqual(0, doctors.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsDoctors_WhenUserHasDoctors()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { Id = Guid.NewGuid(), Name = "Doctor 1", Specialization = "Specialization 1" },
                new Doctor { Id = Guid.NewGuid(), Name = "Doctor 2", Specialization = "Specialization 2" }
            };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _doctorRepositoryMock.Setup(x => x.ReadAllAsync()).ReturnsAsync(doctors);

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedDoctors = okResult.Value as IEnumerable<Doctor>;
            Assert.IsNotNull(returnedDoctors);
            Assert.AreEqual(2, returnedDoctors.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenDoctorNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _doctorRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Doctor?)null);

            // Act
            var result = await _controller.Get(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Get_ReturnsOk_WhenDoctorRetrievedSuccessfully()
        {
            // Arrange
            var doctorId = Guid.NewGuid();
            var doctor = new Doctor { Id = doctorId, Name = "Doctor 1", Specialization = "Specialization 1" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _doctorRepositoryMock.Setup(x => x.ReadAsync(doctorId)).ReturnsAsync(doctor);

            // Act
            var result = await _controller.Get(doctorId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedDoctor = okResult.Value as Doctor;
            Assert.IsNotNull(returnedDoctor);
            Assert.AreEqual(doctorId, returnedDoctor.Id);
        }

        [TestMethod]
        public async Task Add_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Add(new Doctor());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Add_ReturnsCreatedAtRoute_WhenDoctorCreatedSuccessfully()
        {
            // Arrange
            var newDoctor = new Doctor { Name = "New Doctor", Specialization = "Specialization" };
            var createdDoctor = new Doctor { Id = Guid.NewGuid(), Name = "New Doctor", Specialization = "Specialization" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _doctorRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<Doctor>())).ReturnsAsync(createdDoctor);

            // Act
            var result = await _controller.Add(newDoctor);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtRouteResult));
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRouteResult);
            Assert.AreEqual("readDoctor", createdAtRouteResult.RouteName);
            Assert.AreEqual(createdDoctor.Id, createdAtRouteResult.RouteValues["doctorId"]);
            Assert.AreEqual(createdDoctor, createdAtRouteResult.Value);
        }

        [TestMethod]
        public async Task Update_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new Doctor());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenDoctorNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _doctorRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Doctor?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new Doctor());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsOk_WhenDoctorUpdatedSuccessfully()
        {
            // Arrange
            var doctorId = Guid.NewGuid();
            var updatedDoctor = new Doctor { Name = "Updated Doctor", Specialization = "Updated Specialization" };
            var existingDoctor = new Doctor { Id = doctorId, Name = "Existing Doctor", Specialization = "Existing Specialization" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _doctorRepositoryMock.Setup(x => x.ReadAsync(doctorId)).ReturnsAsync(existingDoctor);
            _doctorRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Doctor>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(doctorId, updatedDoctor);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedDoctor = okResult.Value as Doctor;
            Assert.IsNotNull(returnedDoctor);
            Assert.AreEqual(doctorId, returnedDoctor.Id);
            Assert.AreEqual("Updated Doctor", returnedDoctor.Name);
            Assert.AreEqual("Updated Specialization", returnedDoctor.Specialization);
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
        public async Task Delete_ReturnsNotFound_WhenDoctorNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _doctorRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Doctor?)null);

            // Act
            var result = await _controller.Delete(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNoContent_WhenDoctorDeletedSuccessfully()
        {
            // Arrange
            var doctorId = Guid.NewGuid();
            var existingDoctor = new Doctor { Id = doctorId, Name = "Existing Doctor", Specialization = "Existing Specialization" };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _doctorRepositoryMock.Setup(x => x.ReadAsync(doctorId)).ReturnsAsync(existingDoctor);
            _doctorRepositoryMock.Setup(x => x.DeleteAsync(doctorId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(doctorId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}
