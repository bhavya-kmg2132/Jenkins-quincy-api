//using Application.Common.Exceptions;
//using Application.Common.Interfaces;
//using AutoMapper;
//using Domain.Entities;
//using MediatR;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using Application.Common.Attributes;

//namespace Application.Users.Command.UpdateUserProfile
//{
//    /// <summary>
//    /// class UpdateUserProfileRequest extends the IRequest interface of MediatR
//    /// </summary>
//    public class UpdateUserProfileRequest : IRequest<Unit>
//    {
//        //public int Id { get; set; }
//        public string UserId { get; set; }
//        public string DOB { get; set; }
//        public string Gender { get; set; }
//        public string BloodGroup { get; set; }
//        public string PersonalEmail { get; set; }
//        public string DateOfJoining { get; set; }
//        public string PassportNumber { get; set; }
//        public string FatherName { get; set; }
//        public string MotherName { get; set; }
//        public string MaritalStatus { get; set; }
//        public string WeddingAnniversaryDate { get; set; }
//        public string SpouseName { get; set; }
//        public string SpouseDOB { get; set; }
//        public string HomeAddress1 { get; set; }
//        public string HomeAddress2 { get; set; }
//        public string City { get; set; }
//        public string State { get; set; }
//        public string HomeAddressCity { get; set; }
//        public string HomeAddressState { get; set; }
//        public string HomeAddressCountry { get; set; }
//        public string HomePhoneNumber { get; set; }
//        public string EmergencyContactName { get; set; }
//        public string EmergencyContactNumber { get; set; }
//        public string PrimarySkills { get; set; }
//        public string SecondarySkills { get; set; }
//        public string TertiarySkills { get; set; }
//        public string OtherSkills { get; set; }
//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//        public string DirectContact { get; set; }
//        public string PhoneNumber { get; set; }
//        public string Extension { get; set; }
//        public string Email { get; set; }
//        public int BranchId { get; set; }
//        public string BusinessUnit { get; set; }
//        public bool IsActive { get; set; }
//        public string UserRoleId { get; set; }
//        public string ManagerId { get; set; }
//        public string EPICLookupCode { get; set; }
//        public string AccessLevel { get; set; }
//        public string LinkedInUrl { get; set; }
//    }

//    /// <summary>
//    /// For Creating handler for the above request, created UpdateUserProfileRequest class
//    ///that implements the IRequestHandler interface as shown below.
//    /// </summary>
//    public class UpdateUserProfileRequestHandler : IRequestHandler<UpdateUserProfileRequest, Unit>
//    {
//        private readonly ILogger _logger;
//        private readonly IConfiguration _configuration;
//        private readonly IMapper _mapper;
//        private readonly ICurrentUserService _currentUserService;
//        private readonly IUserDataAccess _userDataAccess;

//        /// <summary>
//        ///  Instantiates UpdateUserProfileRequestHandler class
//        /// </summary>
//        /// <param name="configuration"></param>
//        /// <param name="logger"></param>
//        /// <param name="mapper"></param>
//        public UpdateUserProfileRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, ICurrentUserService currentUserService, IUserDataAccess userDataAccess)
//        {
//            this._configuration = configuration;
//            this._logger = logger;
//            this._currentUserService = currentUserService;
//            this._mapper = mapper;
//            this._userDataAccess = userDataAccess;
//        }

//        /// <summary>
//        /// Handler will recieve request ,process it and will return the response.
//        /// </summary>
//        /// <param name="request"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns>Unit</returns>
//        public async Task<Unit> Handle(UpdateUserProfileRequest request, CancellationToken cancellationToken)
//        {
//           NetAuth.Domain.Entities.CoreRequests.UpdateUserProfile userProfileRequest = new NetAuth.Domain.Entities.CoreRequests.UpdateUserProfile()
//            {
//                UserId = request.UserId,
//                DOB = request.DOB,
//                Gender = request.Gender,
//                BloodGroup = request.BloodGroup,
//                PersonalEmail = request.PersonalEmail,
//                DateOfJoining = request.DateOfJoining,
//                PassportNumber = request.PassportNumber,
//                FatherName = request.FatherName,
//                MotherName = request.MotherName,
//                MaritalStatus = request.MaritalStatus,
//                WeddingAnniversaryDate = request.WeddingAnniversaryDate,
//                SpouseName = request.SpouseName,
//                SpouseDOB = request.SpouseDOB,
//                HomeAddress1 = request.HomeAddress1,
//                HomeAddress2 = request.HomeAddress2,
//                City = request.City,
//                State = request.State,
//                HomeAddressCity = request.HomeAddressCity,
//                HomeAddressState = request.HomeAddressState,
//                HomeAddressCountry = request.HomeAddressCountry,
//                HomePhoneNumber = request.HomePhoneNumber,
//                EmergencyContactName = request.EmergencyContactName,
//                EmergencyContactNumber = request.EmergencyContactNumber,
//                PrimarySkills = request.PrimarySkills,
//                SecondarySkills = request.SecondarySkills,
//                TertiarySkills = request.TertiarySkills,
//                OtherSkills = request.OtherSkills,
//                FirstName = request.FirstName,
//                LastName = request.LastName,
//                DirectContact = request.DirectContact,
//                PhoneNumber = request.PhoneNumber,
//                Extension = request.Extension,
//                Email = request.Email,
//                BranchId = request.BranchId,
//                BusinessUnit = request.BusinessUnit,
//                IsActive = request.IsActive,
//                UserRoleId = request.UserRoleId,
//                ManagerId = request.ManagerId,
//                EPICLookupCode = request.EPICLookupCode,
//                AccessLevel = request.AccessLevel,
//                LinkedInUrl = request.LinkedInUrl
//            };

//            var resp = await _userDataAccess.UpdateUserProfile(userProfileRequest);
//            return Unit.Value;
//        }
//    }
//}