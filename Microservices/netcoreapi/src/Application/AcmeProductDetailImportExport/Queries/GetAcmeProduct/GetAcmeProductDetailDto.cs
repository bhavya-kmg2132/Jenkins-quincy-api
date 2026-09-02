using Application.Common.Mappings;

namespace Application.AcmeProductDetailExport.Queries
{
    public class GetAcmeProductDetailDto : IMapFrom<Domain.Entities.AcmeProduct>
    {
        public GetAcmeProductDetailDto()
        {

        }

        public int AcmeProductId { get; set; }
        public int AcmeProductDetailId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string City { get; set; }
        public string Price { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.AcmeProduct, GetAcmeProductDetailDto>()
                .ForMember(d => d.AcmeProductId, opt => opt.Ignore())
                .ForMember(d => d.AcmeProductDetailId, opt => opt.Ignore())
                .ForMember(d => d.Desc, opt => opt.Ignore())
                .ForMember(d => d.City, opt => opt.Ignore())
                .ForMember(d => d.Price, opt => opt.Ignore());
        }
    }
}
