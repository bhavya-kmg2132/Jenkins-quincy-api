using Application.Common.Exceptions;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Infrastructure.DataAccess;
using Application.List.Commands.CreateList;
using System.Collections.Generic;
using Application.List.Queries;
using Application.Prospects.Commands.CreateProspect;

namespace Application.IntegrationTests.CreateList.Commands
{
    using static Testing;
    public class EndPointCreateListTestsTests : TestBase
    {

        [Test]
       // [Ignore("Ignore until endpoints are properly setup")]
        public async Task ShouldDynamicCreateListTest()
        {
            //1. Create Prospect to generate Id 
            //1.1. Create Prospect
            var createProspect = new CreateProspectRequest
            {
                ProspectName = "Unity Earth",
                City = "Chicago",
                State = "IL",
                ZipCode = "60616",
                EmailAddress = "Chris.Green@Caffeine09.onmicrosoft.com",
                PhoneNumber = "1234567890",
                PhoneExtension = "12300",
                IsActive = true,
                Latitude = "37.0902° N",
                Longitude = "95.7129° W",
                Info = "abc",
                Website = "XYZ",
                ProspectInfo = new ProspectInformationVm() { IndustryId = 1 }
            };

            //1.2. Get Id of created prospect
            var Id = await SendAsync(createProspect);


            // 1.Create List.  
            var request = new CreateListRequest
            {
                ListFilter = new ListDto
                {
                    TypeOfList = "Dynamic",
                    ProspectId = new List<int> { Id },
                    SaveAsType = "Publish",// "Template" ,"Draft"
                    ListName = "Dynamic List 1_" + DateTime.UtcNow,
                    FilterJson = "{\"logic\":\"or\",\"filters\":[{\"operator\":\"eq\",\"value\":\"raj\",\"field\":\"prospectName\"},{\"operator\":\"eq\",\"value\":\"NY\",\"field\":\"state\"},{\"logic\":\"and\",\"filters\":[{\"operator\":\"contains\",\"value\":\"3435\",\"field\":\"phoneNumber\"},{\"operator\":\"contains\",\"value\":\"down\",\"field\":\"prospectName\"}]}]}",
                }
            };

            // Get Id of created list
            var insertId = await SendAsync(request);

            // Find created list in the data access.
            var dataAccess = ListDataAccess();
            var createList = await dataAccess.Find(insertId);

            // Assertion
            // Test that created list should not be null.
            createList.Should().NotBeNull();

            // Test that the create list  in data access is same as the created one.
            createList.TypeOfList.Should().Be(request.ListFilter.TypeOfList);
            createList.ListName.Should().Be(request.ListFilter.ListName);
            createList.SaveAsType.Should().Be(request.ListFilter.SaveAsType);
            // createList.Primary.Should().Equals(request.ListFilter.Primary);
        }

       // [Test]
       //// [Ignore("Ignore until endpoints are properly setup")]
       // public async Task ShouldDynamicCreateListWithGroupTest()
       // {
       //     // 1.Create List.  
       //     var request = new CreateListRequest
       //     {
       //         ListFilter = new ListDto
       //         {
       //             TypeOfList = "Dynamic",
       //             ProspectId = new List<int> { 2, 3 },
       //             ListName = "Dynamic List 1 with Group_" + DateTime.UtcNow,
       //             SaveAsType = "Publish",// "Template" ,"Draft"
       //             FilterJson = "{\"logic\":\"or\",\"filters\":[{\"operator\":\"eq\",\"value\":\"raj\",\"field\":\"prospectName\"},{\"operator\":\"eq\",\"value\":\"NY\",\"field\":\"state\"},{\"logic\":\"and\",\"filters\":[{\"operator\":\"contains\",\"value\":\"3435\",\"field\":\"phoneNumber\"},{\"operator\":\"contains\",\"value\":\"down\",\"field\":\"prospectName\"}]}]}",
       //             //Primary = new List<PrimaryFieldVm>()
       //             //{
       //             //    new PrimaryFieldVm {
       //             //    FieldId = 24,
       //             //    FieldName = "State",
       //             //    IsSelected = true,
       //             //    CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
       //             //    {
       //             //        new CriteriaSelectedValueDto {
       //             //        SelectedOperatorLabel =  "Equal",
       //             //        SelectedOperatorValue = "eq",
       //             //        Value = "AL",
       //             //        OperatorType ="or" },

       //             //        new CriteriaSelectedValueDto {
       //             //        SelectedOperatorLabel =  "Equal",
       //             //        SelectedOperatorValue = "eq",
       //             //        Value = "WA",
       //             //        OperatorType ="or" }
       //             //    }
       //             //    },

       //             //    new PrimaryFieldVm {
       //             //    FieldId = 10,
       //             //    FieldName = "Company Name",
       //             //    IsSelected = true,
       //             //    CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
       //             //    {
       //             //        new CriteriaSelectedValueDto {
       //             //        SelectedOperatorLabel =  "Equal",
       //             //        SelectedOperatorValue = "eq",
       //             //        Value = "ABC",
       //             //        OperatorType ="Or" }
       //             //    }
       //             //    },

       //             //    new PrimaryFieldVm {
       //             //    FieldId = 8,
       //             //    FieldName = "Budget/Revenue",
       //             //    IsSelected =true,
       //             //    CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
       //             //    {
       //             //        new CriteriaSelectedValueDto {
       //             //        SelectedOperatorLabel =  "Equal",
       //             //        SelectedOperatorValue = "eq",
       //             //        Value = "500",
       //             //        OperatorType ="AND" }
       //             //    }
       //             //    },
       //             //},
       //             //Group = new List<FilterGroupVm>()
       //             //{
       //             //       new FilterGroupVm{
       //             //       GroupType ="or",
       //             //       Criteria = new List<PrimaryFieldVm>
       //             //       {
       //             //           new PrimaryFieldVm
       //             //           {
       //             //               FieldId = 24,
       //             //               FieldName = "State",
       //             //               IsSelected = true,
       //             //               CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
       //             //               {
       //             //                   new CriteriaSelectedValueDto {
       //             //                   SelectedOperatorLabel =  "Equal",
       //             //                   SelectedOperatorValue = "eq",
       //             //                   Value = "AL",
       //             //                   OperatorType ="or" },

       //             //                   new CriteriaSelectedValueDto {
       //             //                   SelectedOperatorLabel =  "Equal",
       //             //                   SelectedOperatorValue = "eq",
       //             //                   Value = "WA",
       //             //                   OperatorType ="or" }
       //             //               }
       //             //            },
       //             //           new PrimaryFieldVm 
       //             //            {
       //             //                FieldId = 10,
       //             //                FieldName = "Company Name",
       //             //                IsSelected = true,
       //             //                CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
       //             //                {
       //             //                    new CriteriaSelectedValueDto {
       //             //                    SelectedOperatorLabel =  "Equal",
       //             //                    SelectedOperatorValue = "eq",
       //             //                    Value = "ABC",
       //             //                    OperatorType ="Or" }
       //             //                }
       //             //            },
       //             //           new PrimaryFieldVm {
       //             //           FieldId = 8,
       //             //           FieldName = "Budget/Revenue",
       //             //           IsSelected =true,
       //             //           CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
       //             //           {
       //             //               new CriteriaSelectedValueDto {
       //             //               SelectedOperatorLabel =  "Equal",
       //             //               SelectedOperatorValue = "eq",
       //             //               Value = "500",
       //             //               OperatorType ="AND" }
       //             //           }
       //             //           },
       //             //       }
       //             //       }
       //             //}
       //         }
       //     };

       //     // Get Id of created list
       //     var insertId = await SendAsync(request);

       //     // Find created list in the data access.
       //     var dataAccess = ListDataAccess();
       //     var createList = await dataAccess.Find(insertId);

       //     // Assertion
       //     // Test that created list should not be null.
       //     createList.Should().NotBeNull();

       //     // Test that the create list  in data access is same as the created one.
       //     createList.TypeOfList.Should().Be(request.ListFilter.TypeOfList);
       //     createList.ListName.Should().Be(request.ListFilter.ListName);
       //     createList.SaveAsType.Should().Be(request.ListFilter.SaveAsType);
       //     // createList.Primary.Should().Equals(request.ListFilter.Primary);
       // }

        [Test]
        // [Ignore("Ignore until endpoints are properly setup")]
        public async Task ShouldStaticCreateListTest()
        {
            // 1.Create List.  
            var request = new CreateListRequest
            {
                ListFilter = new ListDto
                {
                    TypeOfList = "Static",
                    ProspectId = new List<int> { 2,3},
                    SaveAsType = "Publish",// "Template" ,"Draft"
                    ListName = "Static List 1_" + DateTime.UtcNow,
                    FilterJson = "{\"logic\":\"or\",\"filters\":[{\"operator\":\"eq\",\"value\":\"raj\",\"field\":\"prospectName\"},{\"operator\":\"eq\",\"value\":\"NY\",\"field\":\"state\"},{\"logic\":\"and\",\"filters\":[{\"operator\":\"contains\",\"value\":\"3435\",\"field\":\"phoneNumber\"},{\"operator\":\"contains\",\"value\":\"down\",\"field\":\"prospectName\"}]}]}",
                    //Primary = new List<PrimaryFieldVm>()
                    //{
                    //    new PrimaryFieldVm {
                    //    FieldId = 24,
                    //    FieldName = "State",
                    //    IsSelected = true,
                    //    CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                    //    {
                    //        new CriteriaSelectedValueDto {
                    //        SelectedOperatorLabel =  "Equal",
                    //        SelectedOperatorValue = "eq",
                    //        Value = "AL",
                    //        OperatorType ="or" },

                    //        new CriteriaSelectedValueDto {
                    //        SelectedOperatorLabel =  "Equal",
                    //        SelectedOperatorValue = "eq",
                    //        Value = "WA",
                    //        OperatorType ="or" }
                    //    }
                    //    },

                    //    new PrimaryFieldVm {
                    //    FieldId = 10,
                    //    FieldName = "Company Name",
                    //    IsSelected = true,
                    //    CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                    //    {
                    //        new CriteriaSelectedValueDto {
                    //        SelectedOperatorLabel =  "Equal",
                    //        SelectedOperatorValue = "eq",
                    //        Value = "ABC",
                    //        OperatorType ="Or" }
                    //    }
                    //    },

                    //    new PrimaryFieldVm {
                    //    FieldId = 8,
                    //    FieldName = "Budget/Revenue",
                    //    IsSelected =true,
                    //    CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                    //    {
                    //        new CriteriaSelectedValueDto {
                    //        SelectedOperatorLabel =  "Equal",
                    //        SelectedOperatorValue = "eq",
                    //        Value = "500",
                    //        OperatorType ="AND" }
                    //    }
                    //    },
                    //},
                    //Secondary = new List<PrimaryFieldVm>()
                }
            };

            // Get Id of created list
            var insertId = await SendAsync(request);

            // Find created list in the data access.
            var dataAccess = ListDataAccess();
            var createList = await dataAccess.Find(insertId);

            // Assertion
            // Test that created list should not be null.
            createList.Should().NotBeNull();

            // Test that the create list  in data access is same as the created one.
            createList.TypeOfList.Should().Be(request.ListFilter.TypeOfList);
            createList.ListName.Should().Be(request.ListFilter.ListName);
            createList.SaveAsType.Should().Be(request.ListFilter.SaveAsType);
            // createList.Primary.Should().Equals(request.ListFilter.Primary);
        }

        [Test]
        // [Ignore("Ignore until endpoints are properly setup")]
        public async Task ShouldListWithGroupTest()
        {
            // 1.Create List.  
            var request = new CreateListRequest
            {
                ListFilter = new ListDto
                {
                    TypeOfList = "Static",
                    ProspectId = new List<int> { 2,3},
                    ListName = "Static List 1 with Group_"  + DateTime.UtcNow,
                    SaveAsType = "Publish",// "Template" ,"Draft"
                    FilterJson = "{\"logic\":\"or\",\"filters\":[{\"operator\":\"eq\",\"value\":\"raj\",\"field\":\"prospectName\"},{\"operator\":\"eq\",\"value\":\"NY\",\"field\":\"state\"},{\"logic\":\"and\",\"filters\":[{\"operator\":\"contains\",\"value\":\"3435\",\"field\":\"phoneNumber\"},{\"operator\":\"contains\",\"value\":\"down\",\"field\":\"prospectName\"}]}]}",
                    //Primary = new List<PrimaryFieldVm>()
                    //{
                    //    new PrimaryFieldVm {
                    //    FieldId = 24,
                    //    FieldName = "State",
                    //    IsSelected = true,
                    //    CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                    //    {
                    //        new CriteriaSelectedValueDto {
                    //        SelectedOperatorLabel =  "Equal",
                    //        SelectedOperatorValue = "eq",
                    //        Value = "AL",
                    //        OperatorType ="or" },

                    //        new CriteriaSelectedValueDto {
                    //        SelectedOperatorLabel =  "Equal",
                    //        SelectedOperatorValue = "eq",
                    //        Value = "WA",
                    //        OperatorType ="or" }
                    //    }
                    //    },

                    //    new PrimaryFieldVm {
                    //    FieldId = 10,
                    //    FieldName = "Company Name",
                    //    IsSelected = true,
                    //    CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                    //    {
                    //        new CriteriaSelectedValueDto {
                    //        SelectedOperatorLabel =  "Equal",
                    //        SelectedOperatorValue = "eq",
                    //        Value = "ABC",
                    //        OperatorType ="Or" }
                    //    }
                    //    },

                    //    new PrimaryFieldVm {
                    //    FieldId = 8,
                    //    FieldName = "Budget/Revenue",
                    //    IsSelected =true,
                    //    CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                    //    {
                    //        new CriteriaSelectedValueDto {
                    //        SelectedOperatorLabel =  "Equal",
                    //        SelectedOperatorValue = "eq",
                    //        Value = "500",
                    //        OperatorType ="AND" }
                    //    }
                    //    },
                    //},
                    //Group = new List<FilterGroupVm>()
                    //{
                    //       new FilterGroupVm{
                    //       GroupType ="or",
                    //       Criteria = new List<PrimaryFieldVm>
                    //       {
                    //           new PrimaryFieldVm
                    //           {
                    //               FieldId = 24,
                    //               FieldName = "State",
                    //               IsSelected = true,
                    //               CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                    //               {
                    //                   new CriteriaSelectedValueDto {
                    //                   SelectedOperatorLabel =  "Equal",
                    //                   SelectedOperatorValue = "eq",
                    //                   Value = "AL",
                    //                   OperatorType ="or" },

                    //                   new CriteriaSelectedValueDto {
                    //                   SelectedOperatorLabel =  "Equal",
                    //                   SelectedOperatorValue = "eq",
                    //                   Value = "WA",
                    //                   OperatorType ="or" }
                    //               }
                    //            },
                    //           new PrimaryFieldVm
                    //            {
                    //                FieldId = 10,
                    //                FieldName = "Company Name",
                    //                IsSelected = true,
                    //                CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                    //                {
                    //                    new CriteriaSelectedValueDto {
                    //                    SelectedOperatorLabel =  "Equal",
                    //                    SelectedOperatorValue = "eq",
                    //                    Value = "ABC",
                    //                    OperatorType ="Or" }
                    //                }
                    //            },
                    //           new PrimaryFieldVm {
                    //           FieldId = 8,
                    //           FieldName = "Budget/Revenue",
                    //           IsSelected =true,
                    //           CriteriaSelectedValue = new List<CriteriaSelectedValueDto>()
                    //           {
                    //               new CriteriaSelectedValueDto {
                    //               SelectedOperatorLabel =  "Equal",
                    //               SelectedOperatorValue = "eq",
                    //               Value = "500",
                    //               OperatorType ="AND" }
                    //           }
                    //           },
                    //       }
                    //       }
                    //}
                }
            };

            // Get Id of created list
            var insertId = await SendAsync(request);

            // Find created list in the data access.
            var dataAccess = ListDataAccess();
            var createList = await dataAccess.Find(insertId);

            // Assertion
            // Test that created list should not be null.
            createList.Should().NotBeNull();

            // Test that the create list  in data access is same as the created one.
            createList.TypeOfList.Should().Be(request.ListFilter.TypeOfList);
            createList.ListName.Should().Be(request.ListFilter.ListName);
            createList.SaveAsType.Should().Be(request.ListFilter.SaveAsType);
            // createList.Primary.Should().Equals(request.ListFilter.Primary);
        }
    }
}
