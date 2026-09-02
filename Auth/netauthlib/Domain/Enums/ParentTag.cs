using System.ComponentModel.DataAnnotations;

namespace NetAuth.Domain.Enums
{
    internal enum ParentTag
    {
        Cerebro = 1,

        [Display(Name = "EPIC Classifications")]
        EPICClassifications = 2,

        [Display(Name = "User Defined")]
        UserDefined = 3
    }
}
