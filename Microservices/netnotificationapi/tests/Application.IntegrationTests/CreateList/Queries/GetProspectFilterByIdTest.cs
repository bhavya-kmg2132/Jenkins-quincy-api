using Application.Contacts.Queries.GetContactListQuery;
using Application.CreateList.Commands.CreateList;
using Application.CreateList.Queries;
using Application.CreateList.Queries.GetFilterList;

using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IntegrationTests.CreateList.Queries
{
    using static Testing;

    public class GetProspectFilterByIdTest : TestBase
    {
        /// <summary>
        /// Test to get Manage list.
        /// </summary>
        /// <returns>void</returns>
        [Test] 
        public async Task ShouldReturnProspectList()
        {

            // 1.Create List.  
            var request = new CreateListRequest
            {
                CreateListFilters = new CreateListVm
                {
                    TypeOfList = "Dynamic",
                    IsNewList = true,
                    ListName = "Dynamic First Integration Test_" + DateTime.UtcNow,
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
                            OperatorType ="or" }
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
                    Secondary = new List<PrimaryFieldVm>()
                }
            };

            // Get Id of created list
            var insertId = await SendAsync(request);

            var query = new GetProspectFilterByIdQuery();
             query.GetProspectFilterById = insertId;
                       
            var result = await SendAsync(query);
            result.ProspectList.Should().HaveCountGreaterThan(0);
        }
    }
}
