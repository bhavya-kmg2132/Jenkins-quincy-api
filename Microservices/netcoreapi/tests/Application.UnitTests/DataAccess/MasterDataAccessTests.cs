using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using FluentAssertions;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Application.UnitTests.DataAccess
{
    public class MasterDataAccessTests
    {
        private Mock<ILogger<MasterDataAccess>> _logger;
        private Mock<IConfiguration> _configuration;
        private Mock<IDomainEventService> _domainEventService;
        private Mock<IConnectionHelper> _connectionHelper;
        private MasterDataAccess _sut;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<MasterDataAccess>>();
            _configuration = new Mock<IConfiguration>();
            _domainEventService = new Mock<IDomainEventService>();
            _connectionHelper = new Mock<IConnectionHelper>();

            _connectionHelper
                .Setup(x => x.LoadSqlQueriesXml("Master"))
                .Returns(new Dictionary<string, string>());

            _sut = new MasterDataAccess(
                _configuration.Object,
                _logger.Object,
                _domainEventService.Object,
                _connectionHelper.Object);
        }

        [Test]
        public void Constructor_LoadsSqlQueriesForMaster()
        {
            _connectionHelper.Verify(x => x.LoadSqlQueriesXml("Master"), Times.Once);
        }

        // --- BuildWhereClause pure-logic tests ---

        [Test]
        public async Task BuildWhereClause_NullFiltersAndNullSearch_ReturnsEmpty()
        {
            var result = await _sut.BuildWhereClause(null, "TodoItem", null);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task BuildWhereClause_NullFiltersAndEmptySearch_ReturnsEmpty()
        {
            var result = await _sut.BuildWhereClause(null, "TodoItem", string.Empty);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task BuildWhereClause_SimpleStringFilter_ReturnsLikeClause()
        {
            var filters = JObject.Parse("{\"title\": \"hello\"}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("LIKE '%hello%'");
            result.Should().StartWith("AND ");
        }

        [Test]
        public async Task BuildWhereClause_UnknownFilterColumn_IsIgnored()
        {
            var filters = JObject.Parse("{\"unknownColumn\": \"value\"}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task BuildWhereClause_NumberFilter_GreaterThan_ReturnsCorrectSql()
        {
            var filters = JObject.Parse("{\"listId\": {\"filterType\": \"number\", \"type\": \"greaterThan\", \"filter\": \"5\"}}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("> 5");
        }

        [Test]
        public async Task BuildWhereClause_NumberFilter_Equals_ReturnsCorrectSql()
        {
            var filters = JObject.Parse("{\"listId\": {\"filterType\": \"number\", \"type\": \"equals\", \"filter\": \"10\"}}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("=10");
        }

        [Test]
        public async Task BuildWhereClause_NumberFilter_InRange_ReturnsCorrectSql()
        {
            var filters = JObject.Parse("{\"listId\": {\"filterType\": \"number\", \"type\": \"inRange\", \"filter\": \"1\", \"filterTo\": \"10\"}}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("BETWEEN 1 AND 10");
        }

        [Test]
        public async Task BuildWhereClause_NumberFilter_Blank_ReturnsIsNull()
        {
            var filters = JObject.Parse("{\"listId\": {\"filterType\": \"number\", \"type\": \"blank\", \"filter\": \"\"}}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("IS NULL");
        }

        [Test]
        public async Task BuildWhereClause_TextFilter_Contains_ReturnsLikeClause()
        {
            var filters = JObject.Parse("{\"title\": {\"filterType\": \"text\", \"type\": \"contains\", \"filter\": \"test\"}}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("LIKE '%test%'");
        }

        [Test]
        public async Task BuildWhereClause_TextFilter_StartsWith_ReturnsStartsWithClause()
        {
            var filters = JObject.Parse("{\"title\": {\"filterType\": \"text\", \"type\": \"startsWith\", \"filter\": \"abc\"}}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("LIKE 'abc%'");
        }

        [Test]
        public async Task BuildWhereClause_TextFilter_Equals_ReturnsEqualsClause()
        {
            var filters = JObject.Parse("{\"title\": {\"filterType\": \"text\", \"type\": \"equals\", \"filter\": \"exact\"}}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("= 'exact'");
        }

        [Test]
        public async Task BuildWhereClause_TextFilter_NotBlank_ReturnsIsNotNull()
        {
            var filters = JObject.Parse("{\"title\": {\"filterType\": \"text\", \"type\": \"notBlank\", \"filter\": \"\"}}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("IS NOT NULL");
        }

        [Test]
        public async Task BuildWhereClause_DateFilter_GreaterThan_ReturnsCorrectSql()
        {
            var filters = JObject.Parse("{\"createdDateTime\": {\"filterType\": \"date\", \"type\": \"greaterThan\", \"dateFrom\": \"2024-01-01\", \"dateTo\": null}}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("> '2024-01-01'");
        }

        [Test]
        public async Task BuildWhereClause_DateFilter_InRange_ReturnsCorrectSql()
        {
            var filters = JObject.Parse("{\"createdDateTime\": {\"filterType\": \"date\", \"type\": \"inRange\", \"dateFrom\": \"2024-01-01\", \"dateTo\": \"2024-12-31\"}}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("BETWEEN '2024-01-01' AND '2024-12-31'");
        }

        [Test]
        public async Task BuildWhereClause_SetFilter_WithArray_ReturnsInClause()
        {
            var filters = JObject.Parse("{\"isActive\": {\"filterType\": \"set\", \"values\": [\"Active\", \"Pending\"]}}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("IN (");
        }

        [Test]
        public async Task BuildWhereClause_SetFilter_WithString_ReturnsInClause()
        {
            var filters = JObject.Parse("{\"isActive\": {\"filterType\": \"set\", \"values\": \"Active,Pending\"}}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("IN (");
        }

        [Test]
        public async Task BuildWhereClause_MultipleFilters_CombinesWithAnd()
        {
            var filters = JObject.Parse("{\"title\": \"hello\", \"note\": \"world\"}");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("AND ");
        }

        [Test]
        public async Task BuildWhereClause_MultipleConditionsObject_CombinesWithOperator()
        {
            var filters = JObject.Parse(@"{
                ""title"": {
                    ""filterType"": ""text"",
                    ""operator"": ""OR"",
                    ""conditions"": [
                        {""filterType"": ""text"", ""type"": ""contains"", ""filter"": ""foo""},
                        {""filterType"": ""text"", ""type"": ""startsWith"", ""filter"": ""bar""}
                    ]
                }
            }");

            var result = await _sut.BuildWhereClause(filters, "TodoItem", null);

            result.Should().Contain("OR");
        }

        [Test]
        public async Task BuildWhereClause_NumberFilter_UnsupportedType_ThrowsArgumentException()
        {
            var filters = JObject.Parse("{\"listId\": {\"filterType\": \"number\", \"type\": \"invalidOp\", \"filter\": \"5\"}}");

            var act = async () => await _sut.BuildWhereClause(filters, "TodoItem", null);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        // --- ColumnMapping tests ---

        [Test]
        public void ColumnMapping_GetColumnList_TodoItem_ReturnsMapping()
        {
            var mapping = MasterDataAccess.ColumnMapping.GetColumnList("TodoItem", false);

            mapping.Should().NotBeNull();
            mapping.Should().ContainKey("title");
            mapping.Should().ContainKey("id");
        }

        [Test]
        public void ColumnMapping_GetColumnList_UnknownModule_ThrowsArgumentException()
        {
            var act = () => MasterDataAccess.ColumnMapping.GetColumnList("Unknown", false);

            act.Should().Throw<ArgumentException>().WithMessage("*Unknown*");
        }

        [Test]
        public void ColumnMapping_TodoItemColumnList_ContainsExpectedKeys()
        {
            var list = MasterDataAccess.ColumnMapping.TodoItemColumnList;

            list.Should().ContainKey("id");
            list.Should().ContainKey("listId");
            list.Should().ContainKey("title");
            list.Should().ContainKey("createdBy");
            list.Should().ContainKey("updatedBy");
        }
    }
}
