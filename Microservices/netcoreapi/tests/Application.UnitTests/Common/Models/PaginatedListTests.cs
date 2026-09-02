using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.Common.Models
{
    public class PaginatedListTests
    {
        [Test]
        public void Constructor_SetsPropertiesCorrectly()
        {
            var items = new List<int> { 1, 2, 3 };
            var list = new PaginatedList<int>(items, count: 30, pageIndex: 2, pageSize: 10, totalRecord: 30);

            list.Items.Should().BeEquivalentTo(items);
            list.PageIndex.Should().Be(2);
            list.TotalPages.Should().Be(3);
            list.TotalCount.Should().Be(30);
            list.TotalRecord.Should().Be(30);
        }

        [Test]
        public void HasPreviousPage_IsFalse_OnFirstPage()
        {
            var list = new PaginatedList<int>(new List<int>(), count: 20, pageIndex: 1, pageSize: 10, totalRecord: 20);
            list.HasPreviousPage.Should().BeFalse();
        }

        [Test]
        public void HasPreviousPage_IsTrue_BeyondFirstPage()
        {
            var list = new PaginatedList<int>(new List<int>(), count: 20, pageIndex: 2, pageSize: 10, totalRecord: 20);
            list.HasPreviousPage.Should().BeTrue();
        }

        [Test]
        public void HasNextPage_IsFalse_OnLastPage()
        {
            var list = new PaginatedList<int>(new List<int>(), count: 10, pageIndex: 1, pageSize: 10, totalRecord: 10);
            list.HasNextPage.Should().BeFalse();
        }

        [Test]
        public void HasNextPage_IsTrue_WhenMorePagesExist()
        {
            var list = new PaginatedList<int>(new List<int>(), count: 20, pageIndex: 1, pageSize: 10, totalRecord: 20);
            list.HasNextPage.Should().BeTrue();
        }

        [Test]
        public async Task CreateAsync_ReturnsCorrectPage()
        {
            var source = Enumerable.Range(1, 50).AsQueryable();

            var result = await PaginatedList<int>.CreateAsync(source, pageIndex: 2, pageSize: 10, totalRecord: 50);

            result.Items.Should().HaveCount(10);
            result.Items.First().Should().Be(11);
            result.Items.Last().Should().Be(20);
            result.TotalPages.Should().Be(5);
            result.HasPreviousPage.Should().BeTrue();
            result.HasNextPage.Should().BeTrue();
        }

        [Test]
        public async Task CreateAsync_LastPage_HasNoNextPage()
        {
            var source = Enumerable.Range(1, 25).AsQueryable();

            var result = await PaginatedList<int>.CreateAsync(source, pageIndex: 3, pageSize: 10, totalRecord: 25);

            result.Items.Should().HaveCount(5);
            result.HasNextPage.Should().BeFalse();
        }

        [Test]
        public void TotalPages_RoundsUpForPartialPage()
        {
            var list = new PaginatedList<int>(new List<int>(), count: 21, pageIndex: 1, pageSize: 10, totalRecord: 21);
            list.TotalPages.Should().Be(3);
        }
    }
}
