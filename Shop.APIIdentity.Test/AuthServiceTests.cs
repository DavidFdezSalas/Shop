using MassTransit;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shop.APIIdentity.Dto.Auth;
using Shop.APIIdentity.Services.Auth;
using Shop.Shared.Events;
using System.Security.Claims;

namespace Shop.APIIdentity.Test
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<IPublishEndpoint> _mockPublishEndpoint;
        private AuthService _authservice;
        private Mock<ITokenService> _mockTokenService;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _mockTokenService = new Mock<ITokenService>();

            _mockPublishEndpoint = new Mock<IPublishEndpoint>();

            _authservice = new AuthService(
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockPublishEndpoint.Object);
        }

        #region Register Tests

        [Test]
        public async Task Register_WithValidCredentials_ShouldReturnTrue()
        {
            //Arrange
            var username = "testuser";
            var email = "test@example.com";
            var password = "Test@123";

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "Customer"))
                .ReturnsAsync(IdentityResult.Success);

            _mockPublishEndpoint.Setup(x => x.Publish(
                It.IsAny<UserCreatedEvent>(), default))
                .Returns(Task.CompletedTask);

            //Act
            var result = await _authservice.Register(username, email, password);

            //Assert
            Assert.That(result, Is.True);
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "Customer"), Times.Once);
            _mockPublishEndpoint.Verify(x => x.Publish(
                It.IsAny<UserCreatedEvent>(),
                default), Times.Once);
        }

        [Test]
        public async Task Register_WhenCreateAsyncFails_ShouldReturnFalse()
        {
            //Arrange
            var username = "testuser";
            var email = "test@example.com";
            var password = "Test@123";

            var errors = new IdentityError[]
            {
                new IdentityError { Code = "DuplicateUserName", Description = "Username already exists" }
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            //Act
            var result = await _authservice.Register(username, email, password);

            //Assert
            Assert.That(result, Is.False);
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
            _mockPublishEndpoint.Verify(x => x.Publish(It.IsAny<UserCreatedEvent>(), default), Times.Never);
        }

        [Test]
        public async Task Register_WhenAddToRoleAsyncFails_ShouldStillReturnTrue()
        {
            //Arrange
            var username = "testuser";
            var email = "test@example.com";
            var password = "Test@123";

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "Customer"))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Role not found" }));

            _mockPublishEndpoint.Setup(x => x.Publish(It.IsAny<UserCreatedEvent>(), default))
                .Returns(Task.CompletedTask);

            //Act
            var result = await _authservice.Register(username, email, password);

            //Assert
            Assert.That(result, Is.True);
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "Customer"), Times.Once);
            _mockPublishEndpoint.Verify(x => x.Publish(It.IsAny<UserCreatedEvent>(), default), Times.Once);
        }

        #endregion

        #region Login Tests

        [Test]
        public async Task Login_WithValidCredentials_ShouldReturnSuccessResponseWithToken()
        {
            //Arrange
            var email = "test@example.com";
            var password = "Test@123";
            var expectedToken = "jwt-token-12345";
            var expectedExpiration = DateTime.UtcNow.AddMinutes(60);

            var user = new IdentityUser
            {
                Id = "test-user-id",
                UserName = "testuser",
                Email = email
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, password))
                .ReturnsAsync(true);

            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Customer" });

            _mockTokenService.Setup(x => x.GenerateToken(It.IsAny<IEnumerable<Claim>>()))
                .Returns(new TokenResult(expectedToken, expectedExpiration));

            //Act
            var result = await _authservice.Login(email, password);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Token, Is.EqualTo(expectedToken));
            Assert.That(result.ExpirationAt, Is.EqualTo(expectedExpiration));
            Assert.That(result.ErrorMessage, Is.Null);

            _mockUserManager.Verify(x => x.FindByEmailAsync(email), Times.Once);
            _mockUserManager.Verify(x => x.CheckPasswordAsync(user, password), Times.Once);
            _mockUserManager.Verify(x => x.GetRolesAsync(user), Times.Once);
            _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<IEnumerable<Claim>>()), Times.Once);
        }

        [Test]
        public async Task Login_WithNonExistentUser_ShouldReturnFailureResponse()
        {
            //Arrange
            var email = "nonexistent@example.com";
            var password = "Test@123";

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync((IdentityUser)null);

            //Act
            var result = await _authservice.Login(email, password);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("User not found."));
            Assert.That(result.Token, Is.Null);

            _mockUserManager.Verify(x => x.FindByEmailAsync(email), Times.Once);
            _mockUserManager.Verify(x => x.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
            _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
        }

        [Test]
        public async Task Login_WithInvalidPassword_ShouldReturnFailureResponse()
        {
            //Arrange
            var email = "test@example.com";
            var password = "WrongPassword";

            var user = new IdentityUser
            {
                Id = "test-user-id",
                UserName = "testuser",
                Email = email
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, password))
                .ReturnsAsync(false);

            //Act
            var result = await _authservice.Login(email, password);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid credentials."));
            Assert.That(result.Token, Is.Null);

            _mockUserManager.Verify(x => x.FindByEmailAsync(email), Times.Once);
            _mockUserManager.Verify(x => x.CheckPasswordAsync(user, password), Times.Once);
            _mockUserManager.Verify(x => x.GetRolesAsync(It.IsAny<IdentityUser>()), Times.Never);
            _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
        }

        #endregion
    }
}
