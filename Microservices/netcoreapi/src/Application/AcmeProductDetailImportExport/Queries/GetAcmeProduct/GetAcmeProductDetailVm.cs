using System.Collections.Generic;

namespace Application.AcmeProductDetailExport.Queries.GetAcmeProduct
{
    public class GetAcmeProductDetailVm
    {
        public IList<GetAcmeProductDetailDto> AcmeProductList { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}
