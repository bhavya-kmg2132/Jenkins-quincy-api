using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Common;

namespace Application.Common.Interfaces
{
    public interface IAcmeDataAccess
    {
        #region IAcmeProductDataAccess
        Task<string> Add(Domain.Entities.AcmeProduct acmeProduct);
        Task<List<Domain.Entities.AcmeProduct>> GetAcmeProductList();
        Task<Domain.Entities.AcmeProduct> GetAcmeProductById(string Id);
        Task<int> Update(Domain.Entities.AcmeProduct acme);
        Task<int> Delete(Domain.Entities.AcmeProduct entiey);
        Task<int> DeletePermanentAcmeProduct(string id);
        Task<bool> FindAcmeProductByName(string name);
        Task<ReferenceCustomFields> GetReferenceCustomFields(string tableName);
        #endregion
    }
}
