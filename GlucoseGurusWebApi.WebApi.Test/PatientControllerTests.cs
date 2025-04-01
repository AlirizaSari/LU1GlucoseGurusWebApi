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
    public class PatientControllerTests
    {
        private Mock<IPatientRepository> _patientRepositoryMock = null!;
        private Mock<IParentGuardianRepository> _parentGuardianRepositoryMock = null!;
        private Mock<IDoctorRepository> _doctorRepositoryMock = null!;
        private Mock<ITrajectRepository> _trajectRepositoryMock = null!;
        private Mock<IAuthenticationService> _authenticationServiceMock = null!;
        private Mock<ILogger<PatientController>> _loggerMock = null!;
        private PatientController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _parentGuardianRepositoryMock = new Mock<IParentGuardianRepository>();
            _doctorRepositoryMock = new Mock<IDoctorRepository>();
            _trajectRepositoryMock = new Mock<ITrajectRepository>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _loggerMock = new Mock<ILogger<PatientController>>();
            _controller = new PatientController(
                _patientRepositoryMock.Object,
                _parentGuardianRepositoryMock.Object,
                _doctorRepositoryMock.Object,
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
        public async Task Get_ReturnsPatients_WhenUserAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _patientRepositoryMock.Setup(x => x.ReadAllAsync()).ReturnsAsync(new List<Patient> { new Patient() });

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Get_ReturnsPatient_WhenUserAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _patientRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new Patient { ParentGuardianId = Guid.NewGuid() });
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "userId" });

            // Act
            var result = await _controller.Get(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetByParentGuardian_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.GetByParentGuardian(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task GetByParentGuardian_ReturnsPatients_WhenUserAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "userId" });
            _patientRepositoryMock.Setup(x => x.ReadByParentGuardianAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Patient> { new Patient() });

            // Act
            var result = await _controller.GetByParentGuardian(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetByParentGuardian_ReturnsNotFound_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.GetByParentGuardian(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task GetByParentGuardian_ReturnsPatients_WhenParentGuardianBelongsToUser()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "userId" });
            _patientRepositoryMock.Setup(x => x.ReadByParentGuardianAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Patient> { new Patient() });

            // Act
            var result = await _controller.GetByParentGuardian(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Add_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Add(Guid.NewGuid(), new Patient());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Add_ReturnsNotFound_WhenParentGuardianNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((ParentGuardian?)null);

            // Act
            var result = await _controller.Add(Guid.NewGuid(), new Patient());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Add_ReturnsNotFound_WhenParentGuardianDoesNotBelongToUser()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "differentUserId" });

            // Act
            var result = await _controller.Add(Guid.NewGuid(), new Patient());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Add_ReturnsNotFound_WhenTrajectNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "userId" });
            _trajectRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Traject?)null);

            // Act
            var result = await _controller.Add(Guid.NewGuid(), new Patient { TrajectId = Guid.NewGuid() });

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Add_ReturnsNotFound_WhenDoctorNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "userId" });
            _trajectRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new Traject());
            _doctorRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Doctor?)null);

            // Act
            var result = await _controller.Add(Guid.NewGuid(), new Patient { TrajectId = Guid.NewGuid(), DoctorId = Guid.NewGuid() });

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Add_ReturnsCreated_WhenPatientCreatedSuccessfully()
        {
            // Arrange
            var parentGuardianId = Guid.NewGuid();
            var trajectId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();
            var newPatient = new Patient { TrajectId = trajectId, DoctorId = doctorId };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(parentGuardianId)).ReturnsAsync(new ParentGuardian { UserId = "userId" });
            _trajectRepositoryMock.Setup(x => x.ReadAsync(trajectId)).ReturnsAsync(new Traject { Id = trajectId });
            _doctorRepositoryMock.Setup(x => x.ReadAsync(doctorId)).ReturnsAsync(new Doctor { Id = doctorId });
            _patientRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<Patient>())).ReturnsAsync(new Patient { Id = Guid.NewGuid() });

            // Act
            var result = await _controller.Add(parentGuardianId, newPatient);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtRouteResult));
        }

        [TestMethod]
        public async Task Update_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), Guid.NewGuid(), new Patient());

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
            var result = await _controller.Update(Guid.NewGuid(), Guid.NewGuid(), new Patient());

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
            var result = await _controller.Update(Guid.NewGuid(), Guid.NewGuid(), new Patient());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenPatientNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "userId" });
            _patientRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Patient?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), Guid.NewGuid(), new Patient());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenTrajectNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "userId" });
            _patientRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new Patient());
            _trajectRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Traject?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), Guid.NewGuid(), new Patient { TrajectId = Guid.NewGuid() });

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenDoctorNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "userId" });
            _patientRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new Patient());
            _trajectRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new Traject());
            _doctorRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Doctor?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), Guid.NewGuid(), new Patient { TrajectId = Guid.NewGuid(), DoctorId = Guid.NewGuid() });

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsOk_WhenPatientUpdatedSuccessfully()
        {
            // Arrange
            var parentGuardianId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var trajectId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();
            var updatedPatient = new Patient { TrajectId = trajectId, DoctorId = doctorId };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(parentGuardianId)).ReturnsAsync(new ParentGuardian { UserId = "userId" });
            _patientRepositoryMock.Setup(x => x.ReadAsync(patientId)).ReturnsAsync(new Patient { Id = patientId });
            _trajectRepositoryMock.Setup(x => x.ReadAsync(trajectId)).ReturnsAsync(new Traject { Id = trajectId });
            _doctorRepositoryMock.Setup(x => x.ReadAsync(doctorId)).ReturnsAsync(new Doctor { Id = doctorId });
            _patientRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(parentGuardianId, patientId, updatedPatient);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
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
        public async Task Delete_ReturnsNotFound_WhenParentGuardianNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((ParentGuardian?)null);

            // Act
            var result = await _controller.Delete(Guid.NewGuid(), Guid.NewGuid());

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
            var result = await _controller.Delete(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenPatientNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "userId" });
            _patientRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Patient?)null);

            // Act
            var result = await _controller.Delete(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNoContent_WhenPatientDeletedSuccessfully()
        {
            // Arrange
            var parentGuardianId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(parentGuardianId)).ReturnsAsync(new ParentGuardian { UserId = "userId" });
            _patientRepositoryMock.Setup(x => x.ReadAsync(patientId)).ReturnsAsync(new Patient { Id = patientId });
            _patientRepositoryMock.Setup(x => x.DeleteAsync(patientId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(parentGuardianId, patientId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}
