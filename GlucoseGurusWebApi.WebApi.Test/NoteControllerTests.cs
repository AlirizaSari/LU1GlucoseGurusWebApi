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
    public class NoteControllerTests
    {
        private Mock<INoteRepository> _noteRepositoryMock = null!;
        private Mock<IPatientRepository> _patientRepositoryMock = null!;
        private Mock<IParentGuardianRepository> _parentGuardianRepositoryMock = null!;
        private Mock<IAuthenticationService> _authenticationServiceMock = null!;
        private Mock<ILogger<NoteController>> _loggerMock = null!;
        private NoteController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _noteRepositoryMock = new Mock<INoteRepository>();
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _parentGuardianRepositoryMock = new Mock<IParentGuardianRepository>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _loggerMock = new Mock<ILogger<NoteController>>();
            _controller = new NoteController(
                _noteRepositoryMock.Object,
                _patientRepositoryMock.Object,
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
        public async Task Get_ReturnsEmptyList_WhenUserHasNoNotes()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadAllAsync()).ReturnsAsync(new List<Note>());

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var notes = okResult.Value as IEnumerable<Note>;
            Assert.IsNotNull(notes);
            Assert.AreEqual(0, notes.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsNotes_WhenUserHasNotes()
        {
            // Arrange
            var notes = new List<Note>
            {
                new Note { Id = Guid.NewGuid(), Text = "Note 1", Date = DateTime.Now, UserMood = 3 },
                new Note { Id = Guid.NewGuid(), Text = "Note 2", Date = DateTime.Now, UserMood = 4 }
            };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadAllAsync()).ReturnsAsync(notes);

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedNotes = okResult.Value as IEnumerable<Note>;
            Assert.IsNotNull(returnedNotes);
            Assert.AreEqual(2, returnedNotes.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenNoteNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Note?)null);

            // Act
            var result = await _controller.Get(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Get_ReturnsOk_WhenNoteRetrievedSuccessfully()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var note = new Note { Id = noteId, Text = "Note 1", Date = DateTime.Now, UserMood = 3 };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadAsync(noteId)).ReturnsAsync(note);

            // Act
            var result = await _controller.Get(noteId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedNote = okResult.Value as Note;
            Assert.IsNotNull(returnedNote);
            Assert.AreEqual(noteId, returnedNote.Id);
        }

        [TestMethod]
        public async Task GetByPatient_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.GetByPatient(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task GetByPatient_ReturnsEmptyList_WhenUserHasNoNotesForPatient()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadByPatientAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Note>());

            // Act
            var result = await _controller.GetByPatient(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var notes = okResult.Value as IEnumerable<Note>;
            Assert.IsNotNull(notes);
            Assert.AreEqual(0, notes.Count());
        }

        [TestMethod]
        public async Task GetByPatient_ReturnsNotes_WhenUserHasNotesForPatient()
        {
            // Arrange
            var notes = new List<Note>
            {
                new Note { Id = Guid.NewGuid(), Text = "Note 1", Date = DateTime.Now, UserMood = 3 },
                new Note { Id = Guid.NewGuid(), Text = "Note 2", Date = DateTime.Now, UserMood = 4 }
            };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadByPatientAsync(It.IsAny<Guid>())).ReturnsAsync(notes);

            // Act
            var result = await _controller.GetByPatient(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedNotes = okResult.Value as IEnumerable<Note>;
            Assert.IsNotNull(returnedNotes);
            Assert.AreEqual(2, returnedNotes.Count());
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
        public async Task GetByParentGuardian_ReturnsEmptyList_WhenUserHasNoNotesForParentGuardian()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadByParentGuardianAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Note>());

            // Act
            var result = await _controller.GetByParentGuardian(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var notes = okResult.Value as IEnumerable<Note>;
            Assert.IsNotNull(notes);
            Assert.AreEqual(0, notes.Count());
        }

        [TestMethod]
        public async Task GetByParentGuardian_ReturnsNotes_WhenUserHasNotesForParentGuardian()
        {
            // Arrange
            var notes = new List<Note>
            {
                new Note { Id = Guid.NewGuid(), Text = "Note 1", Date = DateTime.Now, UserMood = 3 },
                new Note { Id = Guid.NewGuid(), Text = "Note 2", Date = DateTime.Now, UserMood = 4 }
            };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadByParentGuardianAsync(It.IsAny<Guid>())).ReturnsAsync(notes);

            // Act
            var result = await _controller.GetByParentGuardian(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedNotes = okResult.Value as IEnumerable<Note>;
            Assert.IsNotNull(returnedNotes);
            Assert.AreEqual(2, returnedNotes.Count());
        }

        [TestMethod]
        public async Task Add_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Add(new Note());

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
            var result = await _controller.Add(new Note { ParentGuardianId = Guid.NewGuid() });

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
            var result = await _controller.Add(new Note { ParentGuardianId = Guid.NewGuid() });

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Add_ReturnsNotFound_WhenPatientNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "userId" });
            _patientRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Patient?)null);

            // Act
            var result = await _controller.Add(new Note { ParentGuardianId = Guid.NewGuid(), PatientId = Guid.NewGuid() });

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Add_ReturnsCreatedAtRoute_WhenNoteCreatedSuccessfully()
        {
            // Arrange
            var parentGuardianId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var newNote = new Note { ParentGuardianId = parentGuardianId, PatientId = patientId, Text = "New Note", Date = DateTime.Now, UserMood = 3 };
            var createdNote = new Note { Id = Guid.NewGuid(), ParentGuardianId = parentGuardianId, PatientId = patientId, Text = "New Note", Date = DateTime.Now, UserMood = 3 };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(parentGuardianId)).ReturnsAsync(new ParentGuardian { Id = parentGuardianId, UserId = "userId" });
            _patientRepositoryMock.Setup(x => x.ReadAsync(patientId)).ReturnsAsync(new Patient { Id = patientId });
            _noteRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<Note>())).ReturnsAsync(createdNote);

            // Act
            var result = await _controller.Add(newNote);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtRouteResult));
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRouteResult);
            Assert.AreEqual("readNote", createdAtRouteResult.RouteName);
            Assert.AreEqual(createdNote.Id, createdAtRouteResult.RouteValues["NoteId"]);
            Assert.AreEqual(createdNote, createdAtRouteResult.Value);
        }

        [TestMethod]
        public async Task Update_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new Note());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenNoteNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Note?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new Note());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenParentGuardianNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new Note());
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((ParentGuardian?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new Note { ParentGuardianId = Guid.NewGuid() });

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenParentGuardianDoesNotBelongToUser()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new Note());
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "differentUserId" });

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new Note { ParentGuardianId = Guid.NewGuid() });

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenPatientNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new Note());
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(new ParentGuardian { UserId = "userId" });
            _patientRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Patient?)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new Note { ParentGuardianId = Guid.NewGuid(), PatientId = Guid.NewGuid() });

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsOk_WhenNoteUpdatedSuccessfully()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var parentGuardianId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var updatedNote = new Note { ParentGuardianId = parentGuardianId, PatientId = patientId, Text = "Updated Note", Date = DateTime.Now, UserMood = 4 };
            var existingNote = new Note { Id = noteId, ParentGuardianId = parentGuardianId, PatientId = patientId, Text = "Existing Note", Date = DateTime.Now, UserMood = 3 };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadAsync(noteId)).ReturnsAsync(existingNote);
            _parentGuardianRepositoryMock.Setup(x => x.ReadAsync(parentGuardianId)).ReturnsAsync(new ParentGuardian { Id = parentGuardianId, UserId = "userId" });
            _patientRepositoryMock.Setup(x => x.ReadAsync(patientId)).ReturnsAsync(new Patient { Id = patientId });
            _noteRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Note>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(noteId, updatedNote);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedNote = okResult.Value as Note;
            Assert.IsNotNull(returnedNote);
            Assert.AreEqual(noteId, returnedNote.Id);
            Assert.AreEqual("Updated Note", returnedNote.Text);
            Assert.AreEqual(4, returnedNote.UserMood);
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
        public async Task Delete_ReturnsNotFound_WhenNoteNotFound()
        {
            // Arrange
            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Note?)null);

            // Act
            var result = await _controller.Delete(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNoContent_WhenNoteDeletedSuccessfully()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var existingNote = new Note { Id = noteId, Text = "Existing Note", Date = DateTime.Now, UserMood = 3 };

            _authenticationServiceMock.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns("userId");
            _noteRepositoryMock.Setup(x => x.ReadAsync(noteId)).ReturnsAsync(existingNote);
            _noteRepositoryMock.Setup(x => x.DeleteAsync(noteId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(noteId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}
