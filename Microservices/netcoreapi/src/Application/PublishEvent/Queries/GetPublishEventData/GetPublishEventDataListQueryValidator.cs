namespace Application.PublishEvent.Queries.GetPublishEventDataList
{
    public class GetPublishEventDataListQueryValidator : AbstractValidator<GetPublishEventDataListQuery>
    {
        public GetPublishEventDataListQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("PageNumber must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1).WithMessage("PageSize must be greater than or equal to 1.");
        }
    }
}
