using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Common;
using Domain.Events;

namespace Domain.Entities
{
    public class User : AuditableEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string EmpId { get; set; }
        public string EmpType { get; set; }
        public string UserRoleId { get; set; }
        public string UserName { get; set; }
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
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Organization { get; set; }
        public string AccessLevel { get; set; }

        [NonSerialized]
        [JsonIgnore]
        [NotMapped]
        private bool _done;

        [JsonIgnore]
        [NotMapped]
        public bool Done
        {
            get => _done;
            set
            {
                if (value == true && _done == false)
                {
                    DomainEvents.Add(new UserCompletedEvent(this));
                }

                _done = value;
            }
        }


        [NonSerialized]
        [NotMapped]
        [JsonIgnore]
        private List<DomainEvent> _domainEvents;

        [JsonIgnore]
        [NotMapped]
        public List<DomainEvent> DomainEvents
        {
            get
            {
                if (_domainEvents == null)
                {
                    _domainEvents = new List<DomainEvent>();
                }

                return _domainEvents;

            }
            set { _domainEvents = value; }
        }
    }
}
