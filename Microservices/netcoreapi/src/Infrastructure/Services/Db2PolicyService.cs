using Application.Common.Interfaces;
using Application.ExternalPolicy.Driver.Commands.AddDriver;
using Application.ExternalPolicy.Driver.Commands.DeleteDriver;
using Application.ExternalPolicy.Driver.Commands.PatchDriver;
using Application.ExternalPolicy.Driver.Queries.GetDriverDetail;
using Application.ExternalPolicy.Endorsement.Queries.GetPolicyEndorsement;
using Application.ExternalPolicy.Models;
using Application.ExternalPolicy.Notepad.Commands.CreateNotepad;
using Application.ExternalPolicy.Notepad.Commands.UpdateNotepad;
using Application.ExternalPolicy.Notepad.Queries.GetNotepadDetail;
using Application.ExternalPolicy.Notepad.Queries.GetNotepads;
using Application.ExternalPolicy.Policy.Commands.ChangePolicy;
using Application.ExternalPolicy.Policy.Commands.ChangeTransaction;
using Application.ExternalPolicy.Policy.Commands.PatchPolicy;
using Application.ExternalPolicy.Policy.Commands.SavePolicyInfo;
using Application.ExternalPolicy.Policy.Commands.UpdatePolicyInfo;
using Application.ExternalPolicy.Policy.Commands.UpdateUnderwriterQuestions;
using Application.ExternalPolicy.Policy.Queries.GetPolicyDetail;
using Application.ExternalPolicy.Policy.Queries.GetPolicyHistory;
using Application.ExternalPolicy.Policy.Queries.GetQuoteNumber;
using Application.ExternalPolicy.Policy.Queries.GetQuotes;
using Application.ExternalPolicy.PolicyCancellation.Commands.CancelPolicy;
using Application.ExternalPolicy.PolicyCancellation.Commands.DeleteTransPolicy;
using Application.ExternalPolicy.PolicyCancellation.Commands.HoldTransaction;
using Application.ExternalPolicy.PolicyCancellation.Queries.GetPolicyCancellation;
using Application.ExternalPolicy.PolicyCancellation.Queries.GetPolicyCancellationDetail;
using Application.ExternalPolicy.TaskManager.Commands.CloseTask;
using Application.ExternalPolicy.TaskManager.Commands.CreateTask;
using Application.ExternalPolicy.TaskManager.Commands.ReferAllTasks;
using Application.ExternalPolicy.TaskManager.Commands.ReferTask;
using Application.ExternalPolicy.TaskManager.Commands.ReopenTask;
using Application.ExternalPolicy.TaskManager.Commands.UpdateTask;
using Application.ExternalPolicy.TaskManager.Queries.GetTaskDetail;
using Application.ExternalPolicy.TaskManager.Queries.GetTasks;
using Application.ExternalPolicy.TaskManager.Queries.GetTaskUsers;
using Application.ExternalPolicy.Vehicle.Commands.AddCoverages;
using Application.ExternalPolicy.Vehicle.Commands.AddVehicle;
using Application.ExternalPolicy.Vehicle.Commands.DeleteCoverages;
using Application.ExternalPolicy.Vehicle.Commands.DeleteVehicle;
using Application.ExternalPolicy.Vehicle.Commands.PatchVehicle;
using Application.ExternalPolicy.Vehicle.Queries.GetVehicleDetail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class Db2PolicyService : ExternalPolicyServiceBase, IDb2PolicyService
    {
        public Db2PolicyService(System.Net.Http.HttpClient httpClient, IConfiguration configuration, ICurrentUserService currentUserService, ILogger<Db2PolicyService> logger)
            : base(httpClient, configuration, currentUserService, logger)
        {
        }

        public Task<ExternalPolicyResponse> GetQuoteNumberAsync(GetQuoteNumberQuery request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/quote-number", request, cancellationToken);

        public Task<ExternalPolicyResponse> GetQuotesAsync(GetQuotesQuery request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/quotes", request, cancellationToken);

        public Task<ExternalPolicyResponse> ChangeTransactionAsync(ChangeTransactionRequest request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/change-transaction", request, cancellationToken);

        public Task<ExternalPolicyResponse> AddVehicleAsync(AddVehicleRequest request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/vehicles", request, cancellationToken);

        public Task<ExternalPolicyResponse> DeleteVehicleAsync(DeleteVehicleRequest request, CancellationToken cancellationToken)
            => DeleteAsync("api/v1/policy/vehicles", request, cancellationToken);

        public Task<ExternalPolicyResponse> GetVehicleDetailAsync(GetVehicleDetailQuery request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/vehicles/query", request, cancellationToken);

        public Task<ExternalPolicyResponse> GetVehicleLocationMasterAsync(CancellationToken cancellationToken)
            => GetAsync("api/v1/policy/vehicles/location-master", cancellationToken);

        public Task<ExternalPolicyResponse> AddCoveragesAsync(AddCoveragesRequest request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/vehicles/coverages", request, cancellationToken);

        public Task<ExternalPolicyResponse> DeleteCoveragesAsync(DeleteCoveragesRequest request, CancellationToken cancellationToken)
            => DeleteAsync("api/v1/policy/vehicles/coverages", request, cancellationToken);

        public Task<ExternalPolicyResponse> PatchVehicleAsync(PatchVehicleRequest request, CancellationToken cancellationToken)
            => PatchAsync("api/v1/policy/vehicles", request, cancellationToken);

        public Task<ExternalPolicyResponse> AddDriverAsync(AddDriverRequest request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/drivers", request, cancellationToken);

        public Task<ExternalPolicyResponse> DeleteDriverAsync(DeleteDriverRequest request, CancellationToken cancellationToken)
            => DeleteAsync("api/v1/policy/drivers", request, cancellationToken);

        public Task<ExternalPolicyResponse> PatchDriverAsync(PatchDriverRequest request, CancellationToken cancellationToken)
            => PatchAsync("api/v1/policy/drivers", request, cancellationToken);

        public Task<ExternalPolicyResponse> GetDriverDetailAsync(GetDriverDetailQuery request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/drivers/detail", request, cancellationToken);

        // Endorsement downstream path mirrors DB2's own Swagger ("Endorsement" tag) exactly.
        public Task<ExternalPolicyResponse> GetPolicyEndorsementAsync(GetPolicyEndorsementQuery request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/endorse/get-policy", request, cancellationToken);

        public Task<ExternalPolicyResponse> ChangePolicyAsync(ChangePolicyRequest request, CancellationToken cancellationToken)
            => GetAsync($"api/v1/policy/change-policy/{Uri.EscapeDataString(request.PolicyNumber)}", cancellationToken);

        public Task<ExternalPolicyResponse> UpdatePolicyInfoAsync(UpdatePolicyInfoRequest request, CancellationToken cancellationToken)
            => PutAsync("api/v1/policy/update-policy-info", request, cancellationToken);

        public Task<ExternalPolicyResponse> RateMcaDataAsync(List<PolicyDataTable> policyData, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/RateMCAData", policyData, cancellationToken);

        public Task<ExternalPolicyResponse> SavePolicyInfoAsync(SavePolicyInfoRequest request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/save-policy-info", request, cancellationToken);

        public Task<ExternalPolicyResponse> UpdateUnderwriterQuestionsAsync(UpdateUnderwriterQuestionsRequest request, CancellationToken cancellationToken)
            => PutAsync("api/v1/policy/underwriter-questions", request, cancellationToken);

        public Task<ExternalPolicyResponse> GetPolicyDetailAsync(GetPolicyDetailQuery request, CancellationToken cancellationToken)
            => GetAsync($"api/v1/policy/{Uri.EscapeDataString(request.PolicyNumber)}", cancellationToken);

        public Task<ExternalPolicyResponse> GetPolicyHistoryAsync(GetPolicyHistoryQuery request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/history", request, cancellationToken);

        public Task<ExternalPolicyResponse> PatchPolicyAsync(PatchPolicyRequest request, CancellationToken cancellationToken)
            => PatchAsync("api/v1/policy/policy-update", request, cancellationToken);

        // Policy Cancellation downstream paths mirror DB2's own Swagger ("PolicyCancellation" tag) exactly.
        public Task<ExternalPolicyResponse> GetPolicyCancellationAsync(GetPolicyCancellationQuery request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/cancellation/get-policy", request, cancellationToken);

        public Task<ExternalPolicyResponse> GetPolicyCancellationDetailAsync(GetPolicyCancellationDetailQuery request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/cancellation/details", request, cancellationToken);

        public Task<ExternalPolicyResponse> CancelPolicyAsync(CancelPolicyRequest request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/cancellation/cancel-policy", request, cancellationToken);

        public Task<ExternalPolicyResponse> DeleteTransPolicyAsync(DeleteTransPolicyRequest request, CancellationToken cancellationToken)
            => DeleteAsync("api/v1/policy/cancellation/delete-policy", request, cancellationToken);

        public Task<ExternalPolicyResponse> HoldTransactionAsync(HoldTransactionRequest request, CancellationToken cancellationToken)
            => PatchAsync("api/v1/policy/cancellation/hold-transaction", request, cancellationToken);

        // Notepad and TaskManager downstream paths are DB2's actual wire contract (from its
        // own Swagger) and must not be renamed to match our public route names.
        public Task<ExternalPolicyResponse> GetNotepadsAsync(GetNotepadsQuery request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/notepads/query", request, cancellationToken);

        public Task<ExternalPolicyResponse> GetNotepadDetailAsync(GetNotepadDetailQuery request, CancellationToken cancellationToken)
            => GetAsync($"api/v1/policy/notepads/{Uri.EscapeDataString(request.NotepadId)}", cancellationToken);

        public Task<ExternalPolicyResponse> CreateNotepadAsync(CreateNotepadRequest request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/notepads", request, cancellationToken);

        public Task<ExternalPolicyResponse> UpdateNotepadAsync(UpdateNotepadRequest request, CancellationToken cancellationToken)
            => PutAsync("api/v1/policy/notepads", request, cancellationToken);

        public Task<ExternalPolicyResponse> GetTasksAsync(GetTasksQuery request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/tasks/query", request, cancellationToken);

        public Task<ExternalPolicyResponse> GetTaskDetailAsync(GetTaskDetailQuery request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/tasks/detail", request, cancellationToken);

        public Task<ExternalPolicyResponse> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/tasks", request, cancellationToken);

        public Task<ExternalPolicyResponse> UpdateTaskAsync(UpdateTaskRequest request, CancellationToken cancellationToken)
            => PutAsync("api/v1/policy/tasks", request, cancellationToken);

        public Task<ExternalPolicyResponse> GetTaskUsersAsync(GetTaskUsersQuery request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/tasks/users-query", request, cancellationToken);

        public Task<ExternalPolicyResponse> ReferTaskAsync(ReferTaskRequest request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/tasks/refer", request, cancellationToken);

        public Task<ExternalPolicyResponse> ReferAllTasksAsync(ReferAllTasksRequest request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/tasks/refer-all", request, cancellationToken);

        public Task<ExternalPolicyResponse> CloseTaskAsync(CloseTaskRequest request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/tasks/close", request, cancellationToken);

        public Task<ExternalPolicyResponse> ReopenTaskAsync(ReopenTaskRequest request, CancellationToken cancellationToken)
            => PostAsync("api/v1/policy/tasks/reopen", request, cancellationToken);
    }
}
