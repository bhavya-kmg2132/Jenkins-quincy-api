using Application.Contacts.Queries.GetContactListQuery;
using Application.CreateList.Commands.CreateList;
using Application.CreateList.Queries;
using Application.CreateList.Queries.GetPreviewList;
using Application.CreateList.Queries.GetPreviewListWithPagination;
using Application.List.Queries;
using Application.List.Queries.GetPreviewListWithPagination;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IntegrationTests.CreateList.Queries
{
    using static Testing;

    public class GetProspectPreviewListWithPaginationTest : TestBase
    {
        /// <summary>
        /// Test to get Prospect Preview List with pagination.
        /// </summary>
        /// <returns>void</returns>
        [Test]
        public async Task ShouldReturnProspectPreviewListWithPagination()
        {
            // 1.Create List.  
            var ListFilter = new ListDto
            {
                TypeOfList = "Dynamic",
                ProspectId = new List<int> { 2, 3 },
                ListName = "Dynamic First Integration Test",
                FilterJson = "{\"logic\":\"or\",\"filters\":[{\"operator\":\"eq\",\"value\":\"raj\",\"field\":\"prospectName\"},{\"operator\":\"eq\",\"value\":\"NY\",\"field\":\"state\"},{\"logic\":\"and\",\"filters\":[{\"operator\":\"contains\",\"value\":\"3435\",\"field\":\"phoneNumber\"},{\"operator\":\"contains\",\"value\":\"down\",\"field\":\"prospectName\"}]}]}",
                //Primary = new List<PrimaryFieldVm>()
                //    {
                //        new PrimaryFieldVm {
                //        FieldId = 24,
                //        FieldName = "State",
                //        IsSelected = true,
                //        CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                //        {
                //            new CriteriaSelectedValueDto {
                //            SelectedOperatorLabel =  "Equal",
                //            SelectedOperatorValue = "eq",
                //            Value = "AL",
                //            OperatorType ="or" },

                //            new CriteriaSelectedValueDto {
                //            SelectedOperatorLabel =  "Equal",
                //            SelectedOperatorValue = "eq",
                //            Value = "WA",
                //            OperatorType ="AND" }
                //        }
                //        },

                //        new PrimaryFieldVm {
                //        FieldId = 10,
                //        FieldName = "Company Name",
                //        IsSelected = true,
                //        CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                //        {
                //            new CriteriaSelectedValueDto {
                //            SelectedOperatorLabel =  "Equal",
                //            SelectedOperatorValue = "eq",
                //            Value = "ABC",
                //            OperatorType ="Or" }
                //        }
                //        },

                //        new PrimaryFieldVm {
                //        FieldId = 8,
                //        FieldName = "Budget/Revenue",
                //        IsSelected =true,
                //        CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                //        {
                //            new CriteriaSelectedValueDto {
                //            SelectedOperatorLabel =  "Equal",
                //            SelectedOperatorValue = "eq",
                //            Value = "500",
                //            OperatorType ="AND" }
                //        }
                //        },
                //    },

            };

            var query = new GetPreviewListWithPaginationQuery()
            {
                ListFilter = ListFilter,
                PageNumber = 1,
                PageSize = 1
            };

            var result = await SendAsync(query);
            result.Items.Should().HaveCountGreaterThan(0);
        }
    }
}

