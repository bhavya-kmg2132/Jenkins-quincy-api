using Application.Common.Exceptions;
using Application.Common.Security;
using Application.IntegrationTests;
using Application.TodoLists.Commands.CreateTodoList;
using Application.TodoLists.Commands.PurgeTodoLists;
using Application.TodoLists.Queries.ExportTodos;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Application.IntegrationTests.TodoLists.Commands
{
    using static Testing;

    public class PurgeTodoListsTests : BaseTestFixture
    {
        [Test]
        public void ShouldDenyAnonymousUser()
        {
            var command = new PurgeTodoListsCommand();

            command.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

            _ = FluentActions.Invoking(() =>
                  SendAsync(command)).Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Test]
        public async Task ShouldDenyNonAdministrator()
        {
            var command = new PurgeTodoListsCommand();

            _ = FluentActions.Invoking(() =>
                  SendAsync(command)).Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Test]
        public async Task ShouldAllowAdministrator()
        {
            var command = new PurgeTodoListsCommand();

            _ = await FluentActions.Invoking(() => SendAsync(command))
                .Should().NotThrowAsync<ForbiddenAccessException>();
        }
    }
}
