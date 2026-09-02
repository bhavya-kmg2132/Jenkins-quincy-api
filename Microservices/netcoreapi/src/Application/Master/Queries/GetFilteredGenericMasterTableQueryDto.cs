using System.Collections.Generic;

namespace Application.Master.Master.Queries.GetFilteredGenericMasterTable
{
    public class GetFilteredGenericMasterTableQueryDto
    {

        public string Type { get; set; }
        public string Group { get; set; }
        public List<Domain.Entities.GenericMasterList> Data { get; set; }
    }
}
