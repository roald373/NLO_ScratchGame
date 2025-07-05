using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.AutoMock;
using NLO_ScratchGame_Contracts;
using NLO_ScratchGame_Database;
using NLOScratchGame_Worker;
using Xunit;

namespace NLOScratchGame_WorkerTests
{
    public class WorkerTests
    {
        private readonly AutoMocker autoMocker = new AutoMocker();

        [Fact]
        public async Task Worker_Should_SaveScratchAttemptAndUpdateScratchCell()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ScratchGameContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            var context = new ScratchGameContext(options);

            await SetupTestContextAsync(context);
            var _sut = autoMocker.CreateInstance<Worker>();

            var command = new ScratchCommand
            {
                UserId = 1,
                Row = 1,
                Column = 1,
                ScratchedAt = DateTimeOffset.UtcNow
            };

            // Act
            await _sut.ProcessScratchCommand(command, CancellationToken.None);

            // Assert
            context.ScratchAttempts.Count().Should().Be(1);
            var attempt = context.ScratchAttempts.First();
            attempt.UserId.Should().Be(command.UserId);
            attempt.Row.Should().Be(command.Row);
            attempt.Column.Should().Be(command.Column);
            attempt.IsSuccessful.Should().BeTrue();

            var scratchedCell = context.ScratchCells.Single(x => x.ScratchedByUserId != null);
            scratchedCell.ScratchedByUserId.Should().Be(command.UserId);
        }

        [Fact]
        public async Task Worker_Should_EnsureAUserCanOnlyScratchOnce()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ScratchGameContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            var context = new ScratchGameContext(options);

            await SetupTestContextAsync(context);
            var _sut = autoMocker.CreateInstance<Worker>();

            var command = new ScratchCommand
            {
                UserId = 1,
                Row = 1,
                Column = 1,
                ScratchedAt = DateTimeOffset.UtcNow
            };

            var command2 = new ScratchCommand
            {
                UserId = 1,
                Row = 1,
                Column = 2,
                ScratchedAt = DateTimeOffset.UtcNow
            };

            // Act
            await _sut.ProcessScratchCommand(command, CancellationToken.None);
            await _sut.ProcessScratchCommand(command2, CancellationToken.None);

            // Assert
            context.ScratchAttempts.Count().Should().Be(2);
            var attempt1 = context.ScratchAttempts.First();
            attempt1.UserId.Should().Be(command.UserId);
            attempt1.Row.Should().Be(command.Row);
            attempt1.Column.Should().Be(command.Column);
            attempt1.IsSuccessful.Should().BeTrue();

            var attempt2 = context.ScratchAttempts.Skip(1).First();
            attempt2.UserId.Should().Be(command2.UserId);
            attempt2.Row.Should().Be(command2.Row);
            attempt2.Column.Should().Be(command2.Column);
            attempt2.IsSuccessful.Should().BeFalse();

            var scratchedCell = context.ScratchCells.Single(x => x.ScratchedByUserId != null);
            scratchedCell.ScratchedByUserId.Should().Be(command.UserId);
        }

        [Fact]
        public async Task Worker_Should_EnsureACellCanOnlyBeScratchedOnce()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ScratchGameContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            var context = new ScratchGameContext(options);

            await SetupTestContextAsync(context);
            var _sut = autoMocker.CreateInstance<Worker>();

            var command = new ScratchCommand
            {
                UserId = 1,
                Row = 1,
                Column = 1,
                ScratchedAt = DateTimeOffset.UtcNow
            };

            var command2 = new ScratchCommand
            {
                UserId = 2,
                Row = 1,
                Column = 1,
                ScratchedAt = DateTimeOffset.UtcNow
            };

            // Act
            await _sut.ProcessScratchCommand(command, CancellationToken.None);
            await _sut.ProcessScratchCommand(command2, CancellationToken.None);

            // Assert
            context.ScratchAttempts.Count().Should().Be(2);
            var attempt1 = context.ScratchAttempts.First();
            attempt1.UserId.Should().Be(command.UserId);
            attempt1.Row.Should().Be(command.Row);
            attempt1.Column.Should().Be(command.Column);
            attempt1.IsSuccessful.Should().BeTrue();

            var attempt2 = context.ScratchAttempts.Skip(1).First();
            attempt2.UserId.Should().Be(command2.UserId);
            attempt2.Row.Should().Be(command2.Row);
            attempt2.Column.Should().Be(command2.Column);
            attempt2.IsSuccessful.Should().BeFalse();

            var scratchedCell = context.ScratchCells.Single(x => x.ScratchedByUserId != null);
            scratchedCell.ScratchedByUserId.Should().Be(command.UserId);
        }

        [Fact]
        public async Task Worker_Should_NotifyClientsOfScratchAttempt()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ScratchGameContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            var context = new ScratchGameContext(options);

            await SetupTestContextAsync(context);
            var _sut = autoMocker.CreateInstance<Worker>();

            var command = new ScratchCommand
            {
                UserId = 1,
                Row = 1,
                Column = 1,
                ScratchedAt = DateTimeOffset.UtcNow
            };

            // Act
            await _sut.ProcessScratchCommand(command, CancellationToken.None);

            // Assert
            autoMocker.GetMock<IScratchEventPublisher>()
                .Verify(x => x.PublishCellScratchedAsync(It.Is<ScratchResult>(r => r.UserId == command.UserId && r.Row == command.Row && r.Column == command.Column && r.SuccessFullyScratched), CancellationToken.None), Times.Once);
        }


        private async Task SetupTestContextAsync(ScratchGameContext context)
        {
            var mock = new Mock<IServiceProvider>();
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            var serviceScopeMock = new Mock<IServiceScope>();
            serviceScopeFactory.Setup(x => x.CreateScope())
                .Returns(serviceScopeMock.Object);
            serviceScopeMock.Setup(x => x.ServiceProvider)
                .Returns(mock.Object);

            mock.Setup(x => x.GetService(typeof(ScratchGameContext)))
                .Returns(context);
            mock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);

            autoMocker.Use(mock);
            await context.SeedData(CancellationToken.None);
        }
    }
}
