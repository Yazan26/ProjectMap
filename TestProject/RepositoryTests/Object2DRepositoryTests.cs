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
    public class Object2DRepositoryTests
    {
        private Mock<IDbConnection> _mockConnection;
        private Object2DRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _mockConnection = new Mock<IDbConnection>();
            _repository = new Object2DRepository("FakeConnectionString");
        }

        [TestMethod]
        public async Task PostObjectAsync_ShouldInsertObject()
        {
            // Arrange
            var object2D = new Object2D
            {
                Id = Guid.NewGuid(),
                Environment2DID = Guid.NewGuid(),
                PrefabId = "Prefab123",
                PositionX = 10.5f,
                PositionY = 20.5f,
                ScaleX = 1.0f,
                ScaleY = 1.0f,
                RotationZ = 0.0f,
                SortingLayer = 1.0f,
                UserID = Guid.NewGuid()
            };

            _mockConnection
                .Setup(conn => conn.ExecuteAsync(It.IsAny<string>(), object2D, null, null, null))
                .ReturnsAsync(1);

            // Act
            var result = await _repository.PostObjectAsync(object2D);

            // Assert
            Assert.AreEqual(object2D, result);
        }

        [TestMethod]
        public async Task GetObjectAsync_ShouldReturnObjects()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedObjects = new List<Object2D>
            {
                new Object2D
                {
                    Id = Guid.NewGuid(),
                    Environment2DID = id,
                    PrefabId = "Prefab123",
                    PositionX = 10.5f,
                    PositionY = 20.5f,
                    ScaleX = 1.0f,
                    ScaleY = 1.0f,
                    RotationZ = 0.0f,
                    SortingLayer = 1.0f,
                    UserID = Guid.NewGuid()
                }
            };

            _mockConnection
                .Setup(conn => conn.QueryAsync<Object2D>("SELECT * FROM [Object2D] WHERE Environment2DID = @id", new { id }, null, null, null))
                .ReturnsAsync(expectedObjects);

            // Act
            var result = await _repository.GetObjectAsync(id);

            // Assert
            Assert.AreEqual(expectedObjects.Count, result.Count());
            Assert.AreEqual(expectedObjects[0].Id, result.First().Id);
        }

        [TestMethod]
        public async Task GetAllObjectsAsync_ShouldReturnAllObjects()
        {
            // Arrange
            var expectedObjects = new List<Object2D>
            {
                new Object2D
                {
                    Id = Guid.NewGuid(),
                    Environment2DID = Guid.NewGuid(),
                    PrefabId = "Prefab123",
                    PositionX = 10.5f,
                    PositionY = 20.5f,
                    ScaleX = 1.0f,
                    ScaleY = 1.0f,
                    RotationZ = 0.0f,
                    SortingLayer = 1.0f,
                    UserID = Guid.NewGuid()
                },
                new Object2D
                {
                    Id = Guid.NewGuid(),
                    Environment2DID = Guid.NewGuid(),
                    PrefabId = "Prefab456",
                    PositionX = 15.5f,
                    PositionY = 25.5f,
                    ScaleX = 2.0f,
                    ScaleY = 2.0f,
                    RotationZ = 45.0f,
                    SortingLayer = 2.0f,
                    UserID = Guid.NewGuid()
                }
            };

            _mockConnection
                .Setup(conn => conn.QueryAsync<Object2D>("SELECT * FROM [Object2D]", null, null, null, null))
                .ReturnsAsync(expectedObjects);

            // Act
            var result = await _repository.GetAllObjectsAsync();

            // Assert
            Assert.AreEqual(expectedObjects.Count, result.Count());
        }

        [TestMethod]
        public async Task UpdateObjectAsync_ShouldUpdateObject()
        {
            // Arrange
            var object2D = new Object2D
            {
                Id = Guid.NewGuid(),
                PositionX = 30.5f,
                PositionY = 40.5f
            };

            _mockConnection
                .Setup(conn => conn.ExecuteAsync("UPDATE [Object2D] SET PositionX = @PositionX, PositionY = @PositionY WHERE Id = @Id", object2D, null, null, null))
                .ReturnsAsync(1);

            // Act
            await _repository.UpdateObjectAsync(object2D);

            // Assert
            _mockConnection.Verify(conn => conn.ExecuteAsync(It.IsAny<string>(), object2D, null, null, null), Times.Once);
        }

        [TestMethod]
        public async Task DeleteObjectAsync_ShouldDeleteObject()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockConnection
                .Setup(conn => conn.ExecuteAsync("DELETE FROM [Object2D] WHERE Id = @Id", new { id }, null, null, null))
                .ReturnsAsync(1);

            // Act
            await _repository.DeleteObjectAsync(id);

            // Assert
            _mockConnection.Verify(conn => conn.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null, null, null), Times.Once);
        }
    }
}
