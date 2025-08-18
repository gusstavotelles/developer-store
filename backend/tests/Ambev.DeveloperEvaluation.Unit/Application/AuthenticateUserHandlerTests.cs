using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using System;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Unit.Application
{
    public class AuthenticateUserHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnResult_WhenCredentialsAreValid()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var jwtGeneratorMock = new Mock<IJwtTokenGenerator>();

            var handler = new AuthenticateUserHandler(
                userRepositoryMock.Object,
                passwordHasherMock.Object,
                jwtGeneratorMock.Object
            );

            var command = new AuthenticateUserCommand
            {
                Email = "user@email.com",
                Password = "password"
            };

            var user = new User
            {
                Email = command.Email,
                Password = "hashed",
                Username = "Test User",
                Role = UserRole.Customer,
                Status = UserStatus.Active
            };

            userRepositoryMock.Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            passwordHasherMock.Setup(h => h.VerifyPassword(command.Password, user.Password)).Returns(true);
            jwtGeneratorMock.Setup(j => j.GenerateToken(user)).Returns("token");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("token", result.Token);
            Assert.Equal(command.Email, result.Email);
            Assert.Equal("Test User", result.Name);
            Assert.Equal("Customer", result.Role);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var jwtGeneratorMock = new Mock<IJwtTokenGenerator>();

            var handler = new AuthenticateUserHandler(
                userRepositoryMock.Object,
                passwordHasherMock.Object,
                jwtGeneratorMock.Object
            );

            var command = new AuthenticateUserCommand
            {
                Email = "user@email.com",
                Password = "wrongpassword"
            };

            var user = new User
            {
                Email = command.Email,
                Password = "hashed",
                Username = "Test User",
                Role = UserRole.Customer,
                Status = UserStatus.Active
            };

            userRepositoryMock.Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            passwordHasherMock.Setup(h => h.VerifyPassword(command.Password, user.Password)).Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                handler.Handle(command, CancellationToken.None));
        }
    }
}
