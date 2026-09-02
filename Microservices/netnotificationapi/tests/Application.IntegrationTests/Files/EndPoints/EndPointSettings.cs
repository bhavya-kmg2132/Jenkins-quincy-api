using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string GetTokenForUserByParamCaffeineLamb { get; set; }
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
        public string CreateAcmeProductDetail { get; set; }
        public string UpdateAcmeProductDetail { get; set; }
        public string DeleteAcmeProductDetail { get; set; }

    }
}
