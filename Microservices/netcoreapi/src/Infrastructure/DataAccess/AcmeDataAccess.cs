using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Dapper;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DataAccess
{
    /// <summary>
    /// Data Access layer :where we write code to connect DB and fetch or manipulate records from DB.
    /// In the database layer, we'll find things like database, connection, table, SQL, and result set.
    /// </summary>
    public class AcmeDataAccess : IAcmeDataAccess
    {
        private ILogger<AcmeDataAccess> _logger;
        private IConfiguration _configuration;
        private readonly IDomainEventService _domainEventService;
        private readonly Dictionary<string, string> _sqlQueries;
        private readonly IConnectionHelper _connectionHelper;

        /// <summary>
        /// AcmeDataAccess : Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="domainEventService"></param>
        /// <param name="acmeProductReaderWriter"></param>
        /// <param name="currentUserService"></param>
        /// <param name="applicationDbContext"></param>
        /// <param name="dapper"></param>
        public AcmeDataAccess(IConfiguration configuration, ILogger<AcmeDataAccess> logger, IDomainEventService domainEventService, ICurrentUserService currentUserService,
            IConnectionHelper connectionHelper)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._configuration = configuration;
            this._domainEventService = domainEventService;

            this._connectionHelper = connectionHelper;
            this._sqlQueries = _connectionHelper.LoadSqlQueriesXml("AcmeProduct");

        }

        #region AcmeProductDataAccess

        /// <summary>
        /// Find Acme (This method is made to get all the remaining Feilds in getbyAcme)
        /// </summary>
        /// <param name="name"></param>
        /// <returns>bool</returns>
        public async Task<bool> FindAcmeProductByName(string name)
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("AcmeDataAccess.FindAcmeProductByName - In process");

                //Step 2: Retrieve SQL Query 
                var findAcmeProductByNameQuery = _sqlQueries["AcmeProduct.FindAcmeProductByName"];

                //Step 3: Declare queryPraram
                var queryParams = new { Name = name };


                using (var _dapperDbConnection = _connectionHelper.CreateConnection())
                {
                    //Step 4: Fetch data from DB using dapper
                    retval = await _dapperDbConnection.ExecuteScalarAsync<bool>(findAcmeProductByNameQuery, queryParams);
                }

                //Step 5: Logging Information
                _logger.LogInformation("AcmeDataAccess.FindAcmeProductByName - Completed");
            }
            catch (Exception ex)
            {
                //Step 6: Error Handling: Log & Rethrow Exception
                _logger.LogError("AcmeDataAccess.FindAcmeProductByName - " + ex.Message);
                throw;
            }

            //Step 7: Return result
            return retval;
        }

        /// <summary>
        /// GetReferenceCustomFields
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>GetReferenceCustomFields</returns>
        public async Task<ReferenceCustomFields> GetReferenceCustomFields(string tableName)
        {
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("AcmeDataAccess.GetReferenceCustomFields - In process");

                //Step 2: Retrieve SQL Query 
                var getReferenceCustomField = _sqlQueries["AcmeProduct.GetReferenceCustomFields"];

                //Step 3: Declare queryPraram
                var queryParams = new { TableName = tableName };

                using (var _dapperDbConnection = _connectionHelper.CreateConnection())
                {
                    //Step 3: Fetch data from DB using dapper
                    string referenceCustomFieldJson = await _dapperDbConnection.QueryFirstOrDefaultAsync<string>(getReferenceCustomField, queryParams);

                    var referenceCustomFields = new ReferenceCustomFields();
                    if (!string.IsNullOrWhiteSpace(referenceCustomFieldJson))
                    {
                        referenceCustomFields.CustomFields = JsonSerializer.Deserialize<List<CustomField>>(referenceCustomFieldJson);
                    }

                    //Step 4: Logging Information: Completed
                    _logger.LogInformation("AcmeDataAccess.GetReferenceCustomFields - Completed");

                    //Step 5: Return referenceCustomFields
                    return referenceCustomFields;
                }
            }
            catch (Exception ex)
            {
                //Step 6: Error Handling: Log & Rethrow Exception
                _logger.LogError("AcmeDataAccess.GetReferenceCustomFields - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Add Acme Product
        /// </summary>
        /// <param name="acmeProduct"></param>
        /// <returns>string</returns>
        public async Task<string> Add(Domain.Entities.AcmeProduct acmeProduct)
        {
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("AcmeDataAccess.Add - In process");

                //Step 2: Serialize Custom Fields (if present) 
                //If CustomFields exists,it is converted to a JSON string for database storage.
                string customFieldsJson = null;
                if (acmeProduct.CustomFields != null)
                {
                    customFieldsJson = JsonSerializer.Serialize(acmeProduct.CustomFields);
                }

                //Step 3: Retrieve Product Code 
                acmeProduct.Code = await GenerateAcmeProductCode();

                //Step 4: Declare Params to be Passed
                var acmeProductParams = new
                {
                    Code = acmeProduct.Code,
                    SKU = acmeProduct.SKU,
                    Name = acmeProduct.Name,
                    DisplayName = acmeProduct.DisplayName,
                    ShortName = acmeProduct.ShortName,
                    Description = acmeProduct.Description,
                    ProductType = acmeProduct.ProductType,
                    BaseCost = acmeProduct.BaseCost,
                    BasePrice = acmeProduct.BasePrice,
                    Currency = acmeProduct.Currency,
                    ProductOwnerId = acmeProduct.ProductOwnerId,
                    ProductOwner = acmeProduct.ProductOwner,
                    ProductCode = acmeProduct.ProductCode,
                    ProductActive = acmeProduct.ProductActive,
                    SalesStartDate = acmeProduct.SalesStartDate,
                    SalesEndDate = acmeProduct.SalesEndDate,
                    SupportStartDate = acmeProduct.SupportStartDate,
                    SupportEndDate = acmeProduct.SupportEndDate,
                    VendorId = acmeProduct.VendorId,
                    VendorName = acmeProduct.VendorName,
                    ProductCategoryId = acmeProduct.ProductCategoryId,
                    ProductCategory = acmeProduct.ProductCategory,
                    ManufacturerId = acmeProduct.ManufacturerId,
                    Manufacturer = acmeProduct.Manufacturer,
                    UnitPrice = acmeProduct.UnitPrice,
                    CommissionRate = acmeProduct.CommissionRate,
                    TaxId = acmeProduct.TaxId,
                    Tax = acmeProduct.Tax,
                    UsageUnitId = acmeProduct.UsageUnitId,
                    UsageUnit = acmeProduct.UsageUnit,
                    QuantityInStock = acmeProduct.QuantityInStock,
                    HandlerId = acmeProduct.HandlerId,
                    Handler = acmeProduct.Handler,
                    QuantityOrdered = acmeProduct.QuantityOrdered,
                    ReorderLevel = acmeProduct.ReorderLevel,
                    QuantityInDemand = acmeProduct.QuantityInDemand,
                    CreatedBy = acmeProduct.CreatedBy,
                    CreatedDateTime = acmeProduct.CreatedDateTime,
                    UpdatedDateTime = acmeProduct.UpdatedDateTime,

                    CustomFields = customFieldsJson
                };

                //Step 5: Retrieve SQL Query 
                var acmeProductQuery = _sqlQueries["AcmeProduct.Add"];

                using (var _dapperDbConnection = _connectionHelper.CreateConnection())
                {
                    //Step 6: Insert Data into the Database Using Dapper
                    var acmeProductResult = await _dapperDbConnection.ExecuteScalarAsync<string>(acmeProductQuery, acmeProductParams);
                    acmeProduct.Id = acmeProductResult;

                    //Step 7: Dispatch Events
                    await DispatchEvents(acmeProduct);

                    //Step 8: Logging Information
                    _logger.LogInformation("AcmeDataAccess.Add - Completed");

                    //Step 9: Return the Inserted ID
                    return acmeProductResult;
                }
            }
            catch (Exception ex)
            {
                // Step 10: Error Handling: Log & Rethrow Exception
                _logger.LogError("AcmeDataAccess.Add - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// UpdateAcmeProduct
        /// </summary>
        /// <param name="acme"></param>
        /// <returns>int</returns>
        public async Task<int> Update(Domain.Entities.AcmeProduct acmeProduct)
        {
            try
            {
                //Step 1: Logging Process
                _logger.LogInformation("AcmeProductDataAccess.Update - In process");

                //Step 2: Serialize Custom Fields (if present) 
                //If CustomFields exists,it is converted to a JSON string for database storage.
                string customFieldsJson = null;
                if (acmeProduct.CustomFields != null)
                {
                    customFieldsJson = JsonSerializer.Serialize(acmeProduct.CustomFields);
                }

                //Step 3: Declare Params to be Passed
                var acmeProductParams = new
                {
                    Id = acmeProduct.Id,
                    Code = acmeProduct.Code,
                    SKU = acmeProduct.SKU,
                    Name = acmeProduct.Name,
                    DisplayName = acmeProduct.DisplayName,
                    ShortName = acmeProduct.ShortName,
                    Description = acmeProduct.Description,
                    ProductType = acmeProduct.ProductType,
                    BaseCost = acmeProduct.BaseCost,
                    BasePrice = acmeProduct.BasePrice,
                    Currency = acmeProduct.Currency,
                    ProductOwnerId = acmeProduct.ProductOwnerId,
                    ProductOwner = acmeProduct.ProductOwner,
                    ProductCode = acmeProduct.ProductCode,
                    ProductActive = acmeProduct.ProductActive,
                    SalesStartDate = acmeProduct.SalesStartDate,
                    SalesEndDate = acmeProduct.SalesEndDate,
                    SupportStartDate = acmeProduct.SupportStartDate,
                    SupportEndDate = acmeProduct.SupportEndDate,
                    VendorId = acmeProduct.VendorId,
                    VendorName = acmeProduct.VendorName,
                    ProductCategoryId = acmeProduct.ProductCategoryId,
                    ProductCategory = acmeProduct.ProductCategory,
                    ManufacturerId = acmeProduct.ManufacturerId,
                    Manufacturer = acmeProduct.Manufacturer,
                    UnitPrice = acmeProduct.UnitPrice,
                    CommissionRate = acmeProduct.CommissionRate,
                    TaxId = acmeProduct.TaxId,
                    Tax = acmeProduct.Tax,
                    UsageUnitId = acmeProduct.UsageUnitId,
                    UsageUnit = acmeProduct.UsageUnit,
                    QuantityInStock = acmeProduct.QuantityInStock,
                    HandlerId = acmeProduct.HandlerId,
                    Handler = acmeProduct.Handler,
                    QuantityOrdered = acmeProduct.QuantityOrdered,
                    ReorderLevel = acmeProduct.ReorderLevel,
                    QuantityInDemand = acmeProduct.QuantityInDemand,
                    UpdatedBy = acmeProduct.UpdatedBy,
                    UpdatedDateTime = acmeProduct.UpdatedDateTime,

                    CustomFields = customFieldsJson
                };

                //Step 4: Retrieve SQL Query 
                var updateQuery = _sqlQueries["AcmeProduct.Update"];

                using (var _dapperDbConnection = _connectionHelper.CreateConnection())
                {
                    //Step 5: Update Data into the Database Using Dapper
                    int rowsAffected = await _dapperDbConnection.ExecuteAsync(updateQuery, acmeProductParams);

                    //Step 6: Dispatch Events
                    await DispatchEvents(acmeProduct);

                    //Step 7: Logging Information
                    _logger.LogInformation("TodoItemDataAccess.Update - Completed");

                    //Step 8: Return rows affected
                    return rowsAffected;
                }
            }
            catch (Exception ex)
            {
                //Step 9: Error Handling: Log & Rethrow Exception
                _logger.LogError("AcmeProductDataAccess.Update - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Delete Acme based on Id
        /// </summary>
        /// <param name="Acme"></param>
        /// <returns>int</returns>
        public async Task<int> Delete(Domain.Entities.AcmeProduct acme)
        {
            try
            {
                //Step 1: Logging Information In process
                this._logger.LogInformation("AcmeDataAccess.Delete - In process");

                //Step 2: Retrieve SQL Query 
                var acmeDeleteQuery = _sqlQueries["AcmeProduct.Delete"];

                using (var _dapperDbConnection = _connectionHelper.CreateConnection())
                {
                    //Step 3: Delete Data from the Database Using Dapper
                    int rowsAffected = await _dapperDbConnection.ExecuteAsync(acmeDeleteQuery, new { acme.Id, acme.UpdatedBy, acme.UpdatedDateTime });

                    //Step 4: Dispatch Events
                    await DispatchEvents(acme);

                    //Step 5: Logging Information
                    _logger.LogInformation("AcmeDataAccess.Delete - Completed");

                    //Step 6: Returns rowsAffected
                    return rowsAffected;
                }
            }
            catch (Exception ex)
            {
                //Step 7: Error Handling: Log & Rethrow Exception
                _logger.LogError("AcmeDataAccess.Delete - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// GetAcmeProductById
        /// </summary>
        /// <param name="id"></param>
        /// <returns>AcmeProduct</returns>
        public async Task<Domain.Entities.AcmeProduct> GetAcmeProductById(string id)
        {
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("AcmeDataAccess.GetAcmeProductById - In process");

                //Step 2: Retrieve SQL Query 
                var getAcmeProductByIdQuery = _sqlQueries["AcmeProduct.GetAcmeProductById"];

                //Step 3: Declare queryPraram
                var queryParams = new { Id = id };

                using (var _dapperDbConnection = _connectionHelper.CreateConnection())
                {
                    //Step 4: Fetch data from DB using dapper
                    AcmeProduct acmeProduct = await _dapperDbConnection.QueryFirstOrDefaultAsync<AcmeProduct>(getAcmeProductByIdQuery, queryParams);

                    if (acmeProduct != null && !string.IsNullOrWhiteSpace(acmeProduct.CustomFieldJson))
                    {
                        acmeProduct.CustomFields = JsonSerializer.Deserialize<List<CustomField>>(acmeProduct.CustomFieldJson);
                    }

                    //Step 5: Logging Information
                    _logger.LogInformation("AcmeDataAccess.GetAcmeProductById - Completed");

                    //Step 6: Return acme
                    return acmeProduct;
                }
            }
            catch (Exception ex)
            {
                //Step 7: Error Handling: Log & Rethrow Exception
                _logger.LogError("AcmeDataAccess.GetAcmeProductById - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// GetAcmeProductList
        /// </summary>
        /// <returns>List<Domain.Entities.AcmeProduct></returns>
        public async Task<List<Domain.Entities.AcmeProduct>> GetAcmeProductList()
        {
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("AcmeDataAccess.GetAcmeProductList - In process");

                //Step 2: Retrieve SQL Query 
                var getAcmeProductQuery = _sqlQueries["AcmeProduct.GetAcmeProductList"];

                using (var _dapperDbConnection = _connectionHelper.CreateConnection())
                {
                    //Step 4: Fetch data from DB using dapper
                    List<AcmeProduct> acmeProductList = (await _dapperDbConnection.QueryAsync<AcmeProduct>(getAcmeProductQuery))
                        .Select(p =>
                        {
                            p.CustomFields = string.IsNullOrWhiteSpace(p.CustomFieldJson)
                                ? null
                                : JsonSerializer.Deserialize<List<CustomField>>(p.CustomFieldJson);

                            return p;
                        })
                        .ToList();

                    //Step 4: Logging Information
                    _logger.LogInformation("AcmeDataAccess.GetAcmeProductList - Completed");

                    //Step 5: Return acme
                    return acmeProductList;
                }
            }
            catch (Exception ex)
            {
                //Step 6: Error Handling: Log & Rethrow Exception
                _logger.LogError("AcmeDataAccess.GetAcmeProductList - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Delete permanently from Acme
        /// </summary>
        /// <param name="id"></param>
        /// <returns>int</returns>
        public async Task<int> DeletePermanentAcmeProduct(string id)
        {
            try
            {
                //Step 1: Logging Information In process
                this._logger.LogInformation("AcmeDataAccess.DeletePermanentAcmeProduct - In process");

                //Step 2: Retrieve SQL Query 
                var acmeDeleteQuery = _sqlQueries["AcmeProduct.DeletePermanentAcmeProduct"];

                using (var _dapperDbConnection = _connectionHelper.CreateConnection())
                {
                    //Step 3: Delete Data from the Database Using Dapper
                    int rowsAffected = await _dapperDbConnection.ExecuteAsync(acmeDeleteQuery, new { id });

                    //Step 4: Logging Information
                    _logger.LogInformation("AcmeDataAccess.DeletePermanentAcmeProduct - Completed");

                    //Step 5: Returns rowsAffected
                    return rowsAffected;
                }
            }
            catch (Exception ex)
            {
                //Step 6: Error Handling: Log & Rethrow Exception
                _logger.LogError("AcmeDataAccess.DeletePermanentAcmeProduct - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Generate Unique AcmeProduct Code
        /// </summary>
        /// <returns>string</returns>
        public async Task<string> GenerateAcmeProductCode()
        {
            //Step 1: Logging Information
            _logger.LogInformation("AcmeDataAccess.GenerateAcmeProductCode - In process");

            int currentYear = DateTime.UtcNow.Year;

            //Step 2: Retrieve SQL Query 
            var generateCodeQuery = _sqlQueries["AcmeProduct.GenerateProductCode"];

            //Step 3: Get latest Product Code
            using (var _dapperDbConnection = _connectionHelper.CreateConnection())
            {
                //Step 4: Fetch data from DB using dapper
                string lastProductCode = await _dapperDbConnection.ExecuteScalarAsync<string>(generateCodeQuery);

                int nextSequence = 1;

                if (!string.IsNullOrWhiteSpace(lastProductCode) && lastProductCode.StartsWith($"PR-{currentYear}-"))
                {
                    var lastSequencePart = lastProductCode.Split('-').LastOrDefault();
                    if (int.TryParse(lastSequencePart, out int lastSequence))
                    {
                        nextSequence = lastSequence + 1;
                    }
                }

                //Step 5: Logging Information
                _logger.LogInformation("AcmeDataAccess.GenerateAcmeProductCode - Completed");

                //Step 6: Return Code
                return $"PR-{currentYear}-{nextSequence.ToString("D6")}";
            }
        }
        #endregion

        /// <summary>
        /// Log event and publish
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task DispatchEvents(Domain.Entities.AcmeProduct entity)
        {
            while (true)
            {
                var domainEventEntity = entity.DomainEvents
                    .Where(domainEvent => !domainEvent.IsPublished)
                    .FirstOrDefault();
                if (domainEventEntity == null) break;

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity);
            }
        }
    }
}

