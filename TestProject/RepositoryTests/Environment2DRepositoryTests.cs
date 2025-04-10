using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MijnWebApi.WebApi.Classes.Repository;
using MijnWebApi.WebApi.Classes.Models;

namespace TestProject.Repositories
{
    [TestClass]
    public class Environment2DRepositoryTests
    {
        private Mock<IDbConnection> _mockConnection;
        private Environment2DRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _mockConnection = new Mock<IDbConnection>();
            _repository = new Environment2DRepository("FakeConnectionString");
        }

        [TestMethod]
        public async Task PostWorldAsync_ShouldInsertEnvironment()
        {
            // Arrange
            var environment2D = new Environment2D
            {
                Id = Guid.NewGuid(),
                Name = "Test Environment",
                MaxHeight = 100,
                MaxWidth = 200,
                OwnerUserID = Guid.NewGuid()
            };

            _mockConnection
                .Setup(conn => conn.ExecuteAsync(It.IsAny<string>(), environment2D, null, null, null))
                .ReturnsAsync(1);

            // Act
            var result = await _repository.PostWorldAsync(environment2D);

            // Assert
            Assert.AreEqual(environment2D, result);
        }

        [TestMethod]
        public async Task GetWorldAsync_ShouldReturnEnvironment()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedEnvironment = new Environment2D
            {
                Id = id,
                Name = "Test Environment",
                MaxHeight = 100,
                MaxWidth = 200,
                OwnerUserID = Guid.NewGuid()
            };

            _mockConnection
                .Setup(conn => conn.QuerySingleOrDefaultAsync<Environment2D>(
                    "SELECT * FROM [Environment2D] WHERE Id = @Id", new { Id = id }, null, null))
                .ReturnsAsync(expectedEnvironment);

            // Act
            var result = await _repository.GetWorldAsync(id);

            // Assert
            Assert.AreEqual(expectedEnvironment, result);
        }

        [TestMethod]
        public async Task GetWorldsForUserAsync_ShouldReturnEnvironmentsForUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedEnvironments = new List<Environment2D>
            {
                new Environment2D
                {
                    Id = Guid.NewGuid(),
                    Name = "Environment 1",
                    MaxHeight = 100,
                    MaxWidth = 200,
                    OwnerUserID = userId
                },
                new Environment2D
                {
                    Id = Guid.NewGuid(),
                    Name = "Environment 2",
                    MaxHeight = 150,
                    MaxWidth = 250,
                    OwnerUserID = userId
                }
            };

            _mockConnection
                .Setup(conn => conn.QueryAsync<Environment2D>(
                    "SELECT * FROM [Environment2D] WHERE OwnerUserID = @id", new { id = userId }, null, null, null))
                .ReturnsAsync(expectedEnvironments);

            // Act
            var result = await _repository.GetWorldsForUserAsync(userId);

            // Assert
            Assert.AreEqual(expectedEnvironments.Count, result.Count());
        }

        [TestMethod]
        public async Task GetWorldsAsync_ShouldReturnAllEnvironments()
        {
            // Arrange
            var expectedEnvironments = new List<Environment2D>
            {
                new Environment2D
                {
                    Id = Guid.NewGuid(),
                    Name = "Environment 1",
                    MaxHeight = 100,
                    MaxWidth = 200,
                    OwnerUserID = Guid.NewGuid()
                },
                new Environment2D
                {
                    Id = Guid.NewGuid(),
                    Name = "Environment 2",
                    MaxHeight = 150,
                    MaxWidth = 250,
                    OwnerUserID = Guid.NewGuid()
                }
            };

            _mockConnection
                .Setup(conn => conn.QueryAsync<Environment2D>(
                    "SELECT * FROM [Environment2D]", null, null, null, null))
                .ReturnsAsync(expectedEnvironments);

            // Act
            var result = await _repository.GetworldsAsync();

            // Assert
            Assert.AreEqual(expectedEnvironments.Count, result.Count());
        }

        [TestMethod]
        public async Task UpdateWorldAsync_ShouldUpdateEnvironment()
        {
            // Arrange
            var environment2D = new Environment2D
            {
                Id = Guid.NewGuid(),
                Name = "Updated Environment",
                MaxHeight = 120,
                MaxWidth = 220,
                OwnerUserID = Guid.NewGuid()
            };

            _mockConnection
                .Setup(conn => conn.ExecuteAsync(
                    "UPDATE [Environment2D] SET Name = @Name, MaxHeight = @MaxHeight, MaxWidth = @MaxWidth WHERE Id = @Id",
                    environment2D, null, null, null))
                .ReturnsAsync(1);

            // Act
            await _repository.UpdateWorldAsync(environment2D);

            // Assert
            _mockConnection.Verify(conn => conn.ExecuteAsync(It.IsAny<string>(), environment2D, null, null, null), Times.Once);
        }

        [TestMethod]
        public async Task DeleteWorldAsync_ShouldDeleteEnvironmentAndObjects()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockConnection
                .Setup(conn => conn.ExecuteAsync("DELETE FROM [Environment2D] WHERE Id = @Id", new { Id = id }, null, null, null))
                .ReturnsAsync(1);

            _mockConnection
                .Setup(conn => conn.ExecuteAsync("DELETE FROM [Object2D] WHERE Environment2DID = @Id", new { Id = id }, null, null, null))
                .ReturnsAsync(1);

            // Act
            await _repository.DeleteWorldAsync(id);

            // Assert
            _mockConnection.Verify(conn => conn.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null, null, null), Times.Exactly(2));
        }
    }
}
