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
using Application.ExternalPolicy.Vehicle.Queries.GetVehicleLocationMaster;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IDb2PolicyService
    {
        Task<ExternalPolicyResponse> GetQuoteNumberAsync(GetQuoteNumberQuery request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> GetQuotesAsync(GetQuotesQuery request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> ChangeTransactionAsync(ChangeTransactionRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> AddVehicleAsync(AddVehicleRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> DeleteVehicleAsync(DeleteVehicleRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> GetVehicleDetailAsync(GetVehicleDetailQuery request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> GetVehicleLocationMasterAsync(CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> AddCoveragesAsync(AddCoveragesRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> DeleteCoveragesAsync(DeleteCoveragesRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> PatchVehicleAsync(PatchVehicleRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> AddDriverAsync(AddDriverRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> DeleteDriverAsync(DeleteDriverRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> PatchDriverAsync(PatchDriverRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> GetDriverDetailAsync(GetDriverDetailQuery request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> GetPolicyEndorsementAsync(GetPolicyEndorsementQuery request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> ChangePolicyAsync(ChangePolicyRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> UpdatePolicyInfoAsync(UpdatePolicyInfoRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> RateMcaDataAsync(List<PolicyDataTable> policyData, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> SavePolicyInfoAsync(SavePolicyInfoRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> UpdateUnderwriterQuestionsAsync(UpdateUnderwriterQuestionsRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> GetPolicyDetailAsync(GetPolicyDetailQuery request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> GetPolicyHistoryAsync(GetPolicyHistoryQuery request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> PatchPolicyAsync(PatchPolicyRequest request, CancellationToken cancellationToken);

        Task<ExternalPolicyResponse> GetPolicyCancellationAsync(GetPolicyCancellationQuery request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> GetPolicyCancellationDetailAsync(GetPolicyCancellationDetailQuery request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> CancelPolicyAsync(CancelPolicyRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> DeleteTransPolicyAsync(DeleteTransPolicyRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> HoldTransactionAsync(HoldTransactionRequest request, CancellationToken cancellationToken);

        Task<ExternalPolicyResponse> GetNotepadsAsync(GetNotepadsQuery request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> GetNotepadDetailAsync(GetNotepadDetailQuery request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> CreateNotepadAsync(CreateNotepadRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> UpdateNotepadAsync(UpdateNotepadRequest request, CancellationToken cancellationToken);

        Task<ExternalPolicyResponse> GetTasksAsync(GetTasksQuery request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> GetTaskDetailAsync(GetTaskDetailQuery request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> UpdateTaskAsync(UpdateTaskRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> GetTaskUsersAsync(GetTaskUsersQuery request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> ReferTaskAsync(ReferTaskRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> ReferAllTasksAsync(ReferAllTasksRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> CloseTaskAsync(CloseTaskRequest request, CancellationToken cancellationToken);
        Task<ExternalPolicyResponse> ReopenTaskAsync(ReopenTaskRequest request, CancellationToken cancellationToken);
    }
}
