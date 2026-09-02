using Application.Common.Exceptions;
using Application.IntegrationTests;
using Application.TodoLists.Commands.CreateTodoList;
using Application.TodoLists.Commands.UpdateTodoList;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Application.IntegrationTests.TodoLists.Commands
{
    using static Testing;

    public class UpdateTodoListTests : BaseTestFixture
    {
        [Test]
        public void ShouldRequireValidTodoListId()
        {
            var command = new UpdateTodoListCommand
            {
                Id = "99",
                Title = "New Title"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().ThrowAsync<NotFoundException>();
        }

        [Test]
        public async Task ShouldRequireUniqueTitle()
        {
            var listId = await SendAsync(new CreateTodoListCommand
            {
                Title = "New List"
            });

            await SendAsync(new CreateTodoListCommand
            {
                Title = "CustomField List"
            });

            var command = new UpdateTodoListCommand
            {
                Id = listId,
                Title = "CustomField List"
            };

            _ = FluentActions.Invoking(() =>
                  SendAsync(command))
                    .Should().ThrowAsync<ValidationException>().Where(ex => ex.Errors.ContainsKey("Title"))
                    .WithMessage("The specified title already exists.*");
        }

        [Test]
        public async Task ShouldUpdateTodoList()
        {
            var listId = await SendAsync(new CreateTodoListCommand
            {
                Title = "New List"
            });

            var command = new UpdateTodoListCommand
            {
                Id = listId,
                Title = "Updated List Title"
            };

            await SendAsync(command);

            //var list = await FindAsync<TodoList>(listId);
            var dataAccess = GetTodoListDataAccess();
            var list = await dataAccess.Get(listId);

            list.Title.Should().Be(command.Title);
            
        }
    }
}
