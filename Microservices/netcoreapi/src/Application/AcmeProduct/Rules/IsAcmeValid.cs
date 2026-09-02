using Application.Common.Interfaces;
using Application.Common.Rules.Engine.Execution;
using Application.Common.Rules.Execution;
using Application.Common.Rules.Generics;
using DomainRules.Acme;

namespace Application.Rules.AcmeProduct
{
    public class IsAcmeValid : Executor<Domain.Entities.AcmeProduct>
    {
        public IsAcmeValid(IAcmeDataAccess acmeDataAccess)
        {
            //Using standard
            Add("NameIsNotNullOrWhiteSpace", new Rule<Domain.Entities.AcmeProduct>(new NameIsNotNullOrWhiteSpace(), "Name field is missing"));
            //Using Generics - Use this
            Add("NameIsNotNullOrWhiteSpace_Generic", new Rule<Domain.Entities.AcmeProduct>(new IsNotNull<Domain.Entities.AcmeProduct>(x => x.Name), "Generic Name field is missing"));

            //Using standard
            //Add("IsDescriptionValid", new Rule<Domain.Entities.AcmeProduct>(new IsDescriptionLengthValid(), "Invalid Description length"));
            //Using Generics and Regex - Use This
            //Add("IsDescriptionValid_Generic", new Rule<Domain.Entities.AcmeProduct>(new IsExpressionValid<Domain.Entities.AcmeProduct>(x => Regex.IsMatch(x.Description, "\\d{11}")), "Invalid Document (regex)"));

            //Using standard
            //Add("DescriptionIsNotNullOrWhiteSpace", new Rule<Domain.Entities.AcmeProduct>(new DescriptionIsNotNullOrWhiteSpace(), "Description field is missing"));

            //Using Database call
            Add("NameIsNotDuplicate", new Rule<Domain.Entities.AcmeProduct>(new NameIsNotDuplicate(acmeDataAccess), "Name already exists in database"));
        }
    }
}
