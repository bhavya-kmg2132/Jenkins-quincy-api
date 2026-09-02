namespace Application.IntegrationTests.EndPoints
{
    public class EndPointsSettings
    {
        public string Test_Environment { get; set; }
        public string ServerUrl { get; set; }
        public ApiServer ApiServer { get; set; }
        public ApiEndPoint ApiEndPoint { get; set; }
        public string AuthorizationToken { get; set; }
        public string AuthorizationToken_Level1User1 { get; set; }
        public string ServerUrl_Development_Staging { get; set; }
        public string ServerUrl_QA_Staging { get; set; }
        public string ServerUrl_UAT_Staging { get; set; }
        public string ServerUrl_Production_Staging { get; set; }


    }
    public class ApiServer
    {
        public string azure_dev_test_slot { get; set; }
        public string azure_dev_slot { get; set; }
        public string azure_qa_slot { get; set; }
        public string azure_uat_slot { get; set; }
        public string azure_staging_slot { get; set; }
        public string azure_production_slot { get; set; }
        public string kmg_dev_server { get; set; }
        public string kmg_qa_server { get; set; }
        public string local { get; set; }

    }

    public class ApiEndPoint
    {
        public string GetTokenServerUrl { get; set; }
        public string GetTokenForChrisGreen { get; set; }
        public string GetTokenForUserByParamKMG { get; set; }
        public string GetTokenForCaffDev { get; set; }
        public string GetTokenForJohnSmith { get; set; }
        public string GetTokenForLevel1User1 { get; set; }
        public string GetTokenForLevel2User1 { get; set; }
        public string GetTokenForLevel3User1 { get; set; }
        public string GetHeartbeatEndPoint { get; set; }
        public string GetHeartbeatNotFoundResultEndPoint { get; set; }
        public string GetHeartbeatNoContentResultEndPoint { get; set; }
        public string GetHeartbeatBadRequestResultEndPoint { get; set; }
        public string GetHeartbeatInternalServerErrorResultEndPoint { get; set; }
        public string GetHeartbeatUnprocessableEntityResultEndPoint { get; set; }
        public string GetHeartbeatUnauthorizedResultEndPoint { get; set; }
        public string GetHeartbeatOkResultEndPoint { get; set; }

        public string CreateToDoItemEndPoint { get; set; }
        public string CreateProspectEndPoint { get; set; }
        public string GetAllProspectsByAccessLevel { get; set; }
        public string UpdateProspectEndPoint { get; set; }
        public string CreateContactEndPoint { get; set; }
        public string InactivateProspectEndPoint { get; set; }
        public string ReactivateProspectEndPoint { get; set; }
        public string CreateWhiteBoardActivityEndPoint { get; set; }
        public string UpdateWhiteBoardActivityEndPoint { get; set; }
        public string UpdateContactEndPoint { get; set; }
        public string CreateNoteEndPoint { get; set; }
        public string UpdateNoteEndPoint { get; set; }
        public string DeleteNoteEndPoint { get; set; }
        public string CreateDialEndPoint { get; set; }
        public string DeleteDialEndPoint { get; set; }

        //Recall
        public string UpdateRecallEndPoint { get; set; }
        public string DeleteRecallEndPoint { get; set; }

        //Tags
        public string CreateTagFromMaintainence { get; set; }
        public string UpdateTagFromMaintainenceEndPoint { get; set; }
        public string DeleteTagFromMaintainenceEndPoint { get; set; }

        //Efile - Create 
        public string CreateEfileNodeEndPoint { get; set; }

        //Ring Out Call Request
        public string RingOutCallLogRequestEndPoint { get; set; }

        public string ProspectImportFromCsvEndPoint { get; set; }

        //Carrier
        public string UpdateCarrierEndPoint { get; set; }

        //Carrier

        public string CreateCarrierEndPoint { get; set; }
        public string DeleteCarrierEndPoint { get; set; }

        //Cerebro
        public string CerebroRequest { get; set; }


        public string CreateEpicClientEndPoint { get; set; }

        public string QuryListAccessLevel { get; set; }


        //Acme Product
        public string AcmeProductImportFromCsvEndPoint { get; set; }
        public string CreateAcmeProduct { get; set; }
        public string UpdateAcmeProduct { get; set; }
        public string DeleteAcmeProduct { get; set; }

        public string AcmeProductDeletePermanent { get; set; }
        public string CreateAcmeProductDetail { get; set; }
        public string UpdateAcmeProductDetail { get; set; }
        public string DeleteAcmeProductDetail { get; set; }

        // Auth
        public string AuthLogin { get; set; }
        public string AuthRegister { get; set; }
        public string AuthRefreshToken { get; set; }
        public string AuthUpdatePasswordHash { get; set; }

        // NetAuth - Users
        public string NetAuthGetUsers { get; set; }
        public string NetAuthGetUsersAsync { get; set; }
        public string NetAuthGetUserVmByUserName { get; set; }
        public string NetAuthGetUserByRoleId { get; set; }
        public string NetAuthGetUsersByStatus { get; set; }
        public string NetAuthAddUser { get; set; }
        public string NetAuthUpdateUser { get; set; }
        public string NetAuthActivateOrInActivateUser { get; set; }
        public string NetAuthResetUserObjectCache { get; set; }

        // NetAuth - Permissions
        public string NetAuthGetPermissions { get; set; }
        public string NetAuthGetPermissionsAsync { get; set; }
        public string NetAuthGetPermissionsByRoleId { get; set; }
        public string NetAuthAddPermission { get; set; }
        public string NetAuthUpdatePermission { get; set; }
        public string NetAuthAddPermissionsGrantedForUser { get; set; }
        public string NetAuthAddPermissionsDeniedForUser { get; set; }
        public string NetAuthAddPermissionsForRole { get; set; }

        // NetAuth - Roles
        public string NetAuthGetRoles { get; set; }
        public string NetAuthAddRoles { get; set; }
        public string NetAuthAddRole { get; set; }
        public string NetAuthDeleteRole { get; set; }

        // NetAuth - UiPermissions
        public string NetAuthGetUiPermissions { get; set; }
        public string NetAuthGetUiPermissionsByUserId { get; set; }
        public string NetAuthGetUiPermissionsByRoleId { get; set; }
        public string NetAuthAddUiPermission { get; set; }
        public string NetAuthUpdateUiPermission { get; set; }
        public string NetAuthAddUiPermissionsForRole { get; set; }

        // NetAuth - Teams
        public string NetAuthGetTeams { get; set; }
        public string NetAuthGetTeamById { get; set; }
        public string NetAuthGetTeamsByUserId { get; set; }
        public string NetAuthAddTeam { get; set; }
        public string NetAuthAddTeamMembers { get; set; }
        public string NetAuthRemoveTeamMember { get; set; }
        public string NetAuthGetTeamMembersByTeamId { get; set; }

        // NetAuth - User Activities & Lookups
        public string NetAuthGetUserActivities { get; set; }
        public string NetAuthGetUserActivitiesByUserIds { get; set; }
        public string NetAuthAddUserActivity { get; set; }
        public string NetAuthGetAuthReferenceLookupsByTypeName { get; set; }

        // TodoItems
        public string TodoItemsGetWithPagination { get; set; }
        public string TodoItemsGetWithFilter { get; set; }
        public string TodoItemsCreate { get; set; }
        public string TodoItemsUpdate { get; set; }
        public string TodoItemsUpdateDetails { get; set; }
        public string TodoItemsDelete { get; set; }
        public string TodoItemsDeletePermanent { get; set; }

        // TodoLists
        public string TodoListsGetList { get; set; }
        public string TodoListsGetById { get; set; }
        public string TodoListsCreate { get; set; }
        public string TodoListsUpdate { get; set; }
        public string TodoListsDelete { get; set; }
        public string TodoListsDeletePermanent { get; set; }



        //Acme Order

        public string AcmeOrderDeletePermanent { get; set; }
    }
}
