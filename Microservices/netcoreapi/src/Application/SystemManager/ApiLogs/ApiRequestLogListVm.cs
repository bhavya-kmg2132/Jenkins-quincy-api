using System.Collections.Generic;

namespace Application.ApiLog.Queries
{
    public class ApiRequestLogListVm
    {
        public List<ApiRequestLogDto> Items { get; set; }
        public int Total { get; set; }
    }
}
