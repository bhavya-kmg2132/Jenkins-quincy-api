//using Application.Common.Models;
//using Microsoft.AspNetCore.Identity;
//using System.Linq;

//namespace Infrastructure.Identity
//{
//    public static class IdentityResultExtensions
//    {
//        public static Result ToApplicationResult(this IdentityResult result)
//        {
//            return result.Succeeded
//                ? Result.IsSuccess()
//                : Result.Failure(result.Errors.Select(e => e.Description));
//        }
//    }
//}