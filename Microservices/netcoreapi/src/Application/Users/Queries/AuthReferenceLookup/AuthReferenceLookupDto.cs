using Application.Common.Mappings;

namespace Application.Users.Queries.AuthReferenceLookup
{
    public class AuthReferenceLookupDto : IMapFrom<NetAuth.Contract.DataContract.Entities.AuthReferenceLookup>
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        //Mapping AuthReferenceLookupDto with AuthReferenceLookup entity
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NetAuth.Contract.DataContract.Entities.AuthReferenceLookup, AuthReferenceLookupDto>();
        }
    }
}
