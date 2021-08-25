using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MinecraftServerInfoPanel.BL.PlayTimeCalculator;
using MinecraftServerInfoPanel.DataLayer;
using MinecraftServerInfoPanel.Domain.Entities;
using System;
using System.Collections.Generic;
using Xunit;

namespace MinecraftServerInfoPanel.UnitTests
{
    public class CalculateUserAllPlayTimeTests
    {
        private DbContextOptions<MinecraftDbContext> dbContextOptions = new DbContextOptionsBuilder<MinecraftDbContext>()
            .UseInMemoryDatabase(databaseName: "MinecraftDb")
            .Options;

        private MinecraftDbContext dbContext;

        public CalculateUserAllPlayTimeTests()
        {
            if (dbContext != null)
                dbContext.Dispose();
            dbContext = new MinecraftDbContext(dbContextOptions);
            //SeedDb();
        }

        [Fact]
        public void no_data_in_db_return_zero()
        {
            // Arrange

            // Act
            var result = Execute();

            // Assert
            result.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void alltime()
        {
            // Arrange
            SeedDb();

            // Act
            var result = Execute();

            // Assert
            result.Should().Be(TimeSpan.FromMinutes(58));
        }

        private TimeSpan Execute()
        {
            var sut = new PlayTimeCalculator(dbContext);
            return sut.CalculateUserAllPlayTime("prasol");
        }

        private void SeedDb()
        {
            List<DbConsoleLog> logs = new List<DbConsoleLog>
            {
                new DbConsoleLog { Id = 1, Date = new DateTime(2021, 8, 25, 18, 57, 33), Information = "prasol connected" },
                new DbConsoleLog { Id = 2, Date = new DateTime(2021, 8, 25, 19, 55, 33), Information = "prasol disconnected" },
            };

            dbContext.AddRange(logs);
            dbContext.SaveChanges();
        }
    }
}
