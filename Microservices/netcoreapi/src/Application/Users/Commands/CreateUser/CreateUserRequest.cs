using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.CreateUser
{
    /// <summary>
    /// class CreateUserRequest extends the IRequest interface of MediatR
    /// </summary>
    public class CreateUserRequest : IRequest<string>
    {
        public string Id { get; set; }
        public string EmpId { get; set; }
        public string EmpType { get; set; }
        public string UserName { get; set; }
        public string auth_type { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string BusinessUnit { get; set; }
        //public bool IsDeleted { get; set; }

        public string oid { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string preferred_username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondaryEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string Extension { get; set; }
        public string display_name { get; set; }
        public string ManagerId { get; set; }
        public string AccessLevel { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Organization { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request , created CreateUserRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class CreateUserRequestHandler : IRequestHandler<CreateUserRequest, string>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates the class CreateUserRequestHandler
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public CreateUserRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess userDataAccess, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._userDataAccess = userDataAccess;
            this._mapper = mapper;
            this._currentUserService = currentUserService;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>UserId of created user</returns>
        public async Task<string> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("CreateUserRequest.Handle - In process");

            //2. Assign requested User values 
            var entity = new NetAuth.Contract.DataContract.Requests.CreateUserRequest();

            entity.EmpId = request.EmpId;
            entity.EmpType = request.EmpType;
            entity.UserName = request.UserName;
            entity.auth_type = request.auth_type;
            entity.Mobile = request.Mobile;
            entity.Email = request.Email;
            entity.Position = request.Position;
            entity.BusinessUnit = request.BusinessUnit;
            entity.oid = request.oid;
            entity.given_name = request.given_name;
            entity.family_name = request.family_name;
            entity.preferred_username = request.preferred_username;
            entity.FirstName = request.FirstName;
            entity.LastName = request.LastName;
            entity.SecondaryEmail = request.SecondaryEmail;
            entity.PhoneNumber = request.PhoneNumber;
            entity.Extension = request.Extension;
            entity.display_name = request.display_name;
            entity.ManagerId = request.ManagerId;
            entity.AccessLevel = request.AccessLevel;
            entity.Designation = request.Designation;
            entity.Department = request.Department;
            entity.Location = request.Location;
            entity.Organization = request.Organization;

            //4.Add the User
            entity.CreatedBy = _currentUserService.oid;
            string id = await _userDataAccess.AddUser(entity);

            //5. Logging Information : Completed
            _logger.LogInformation("CreateUserRequest.Handle - Completed");

            //6. Return id
            return id;
        }
    }
}



