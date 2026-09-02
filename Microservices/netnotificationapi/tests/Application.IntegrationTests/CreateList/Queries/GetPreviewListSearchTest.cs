using Application.Contacts.Queries.GetContactListQuery;
using Application.CreateList.Commands.CreateList;
using Application.CreateList.Queries;
using Application.CreateList.Queries.GetPreviewList;
using Application.CreateList.Queries.GetPreviewListWithPagination;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IntegrationTests.CreateList.Queries
{
    using static Testing;

    public class GetPreviewListWithSearchTest : TestBase
    {
        /// <summary>
        /// Test to get Prospect Preview List search.
        /// </summary>
        /// <returns>void</returns>
        [Test]
        public async Task ShouldReturnProspectPreviewListSearch()
        {
            // 1.Create List.  
            var createListFilters = new CreateListVm
            {
                TypeOfList = "Dynamic",
                IsNewList = true,
                ListName = "Dynamic First Integration Test",
                SaveAsType = "Publish",
                Primary = new List<PrimaryFieldVm>()
                    {
                        new PrimaryFieldVm {
                        FieldId = 24,
                        FieldName = "State",
                        IsSelected = true,
                        CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                        {
                            new CriteriaSelectedValueDto {
                            SelectedOperatorLabel =  "Equal",
                            SelectedOperatorValue = "eq",
                            Value = "AL",
                            OperatorType ="or" },

                            new CriteriaSelectedValueDto {
                            SelectedOperatorLabel =  "Equal",
                            SelectedOperatorValue = "eq",
                            Value = "WA",
                            OperatorType ="AND" }
                        }
                        },

                        new PrimaryFieldVm {
                        FieldId = 10,
                        FieldName = "Company Name",
                        IsSelected = true,
                        CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                        {
                            new CriteriaSelectedValueDto {
                            SelectedOperatorLabel =  "Equal",
                            SelectedOperatorValue = "eq",
                            Value = "ABC",
                            OperatorType ="Or" }
                        }
                        },

                        new PrimaryFieldVm {
                        FieldId = 8,
                        FieldName = "Budget/Revenue",
                        IsSelected =true,
                        CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                        {
                            new CriteriaSelectedValueDto {
                            SelectedOperatorLabel =  "Equal",
                            SelectedOperatorValue = "eq",
                            Value = "500",
                            OperatorType ="AND" }
                        }
                        },
                    },

            };

            var query = new GetPreviewListWithWildCardSearchQuery()
            {
                CreateListFilters = createListFilters,
                RowCount = 1,
                Search = "a"
            };

            var result = await SendAsync(query);
            result.ProspectList.Should().HaveCountGreaterThan(0);
        }
    }
}

