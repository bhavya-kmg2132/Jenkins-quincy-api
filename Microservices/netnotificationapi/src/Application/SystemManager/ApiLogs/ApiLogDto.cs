namespace Application.ApiLog.Queries
{
    /// <summary>
    /// Dto class is used to pass data from domain to ViewModel layer.
    /// It helps in:
    /// 1. Abstraction of Domain layer
    /// 2. Data Hiding
    /// 3. Serialization and Lazy load problems
    /// </summary>
    public class ApiLogDto //: IMapFrom<Domain.Entities.ApiLog>
    {



        public string Discription { get; set; }


        ////Mapping BusinessLineDto with MasterBusinessLine entity
        //public void Mapping(Profile profile)
        //{
        //    profile.CreateMap<Domain.Entities.ApiLog, ApiLogDto>();
        //}
    }
}
