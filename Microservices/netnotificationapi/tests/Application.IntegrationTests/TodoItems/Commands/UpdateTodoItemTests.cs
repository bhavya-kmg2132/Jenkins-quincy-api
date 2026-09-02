using Application.Common.Exceptions;
using Application.TodoItems.Commands.CreateTodoItem;
using Application.TodoItems.Commands.UpdateTodoItem;
using Application.TodoLists.Commands.CreateTodoList;
using Domain.Entities;
using FluentAssertions;
using System.Threading.Tasks;
using NUnit.Framework;
using System;
using Application.IntegrationTests;

namespace Application.IntegrationTests.TodoItems.Commands
{
    using static Testing;

    public class UpdateTodoItemTests : BaseTestFixture
    {
        [Test]
        public void ShouldRequireValidTodoItemId()
        {
            var command = new UpdateTodoItemCommand
            {
                Id =    "99",
                Title = "New Title"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().ThrowAsync<NotFoundException>();
        }

        [Test]
        public async Task ShouldUpdateTodoItem()
        {
            
            var listId = await SendAsync(new CreateTodoListCommand
            {
                Title = "New List"
            });

            var itemId = await SendAsync(new CreateTodoItemCommand
            {
                ListId = listId,
                Title = "New Item"
            });

            var command = new UpdateTodoItemCommand
            {
                Id = itemId,
                Title = "Updated Item Title"
            };

            await SendAsync(command);

            //var item = await FindAsync<TodoItem>(itemId);
            var dataAccess = GetTodoItemDataAccess();
            TodoItem item = await dataAccess.Get(itemId);

            item.Title.Should().Be(command.Title);
            
        }
    }
}
