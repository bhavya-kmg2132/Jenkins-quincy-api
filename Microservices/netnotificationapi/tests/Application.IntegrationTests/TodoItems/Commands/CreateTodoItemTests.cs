using Application.Common.Exceptions;
using Application.TodoItems.Commands.CreateTodoItem;
using Application.TodoLists.Commands.CreateTodoList;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Application.IntegrationTests.TodoItems.Commands
{
    using static Testing;

    [TestFixture]
    public class EndPointCreateTodoItemTests : BaseTestFixture
    {
        [SetUp]
        public void DerivedSetUp()
        {
            RunAsDefaultUserAsync();
        }

        [TearDown]
        public void DerivedTearDown() { }


        [Test]
        public void ShouldRequireMinimumFields()
        {
            var command = new CreateTodoItemCommand();

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task ShouldCreateTodoItem()
        {
            
            var listId = await SendAsync(new CreateTodoListCommand
            {
                Title = "New List"
            });

            var command = new CreateTodoItemCommand
            {
                ListId = listId,
                Title = "Tasks"
                
            };

            var itemId = await SendAsync(command);


            //var item = await FindAsync<TodoItem>(itemId);
            var dataAccess = GetTodoItemDataAccess();
            var item = await dataAccess.Get(itemId);

            item.Should().NotBeNull();
            item.ListId.Should().Be(command.ListId);
            item.Title.Should().Be(command.Title);
            //item.LastModifiedBy.Should().BeNull();
            //item.LastModified.Should().BeNull();
        }
    }
}
