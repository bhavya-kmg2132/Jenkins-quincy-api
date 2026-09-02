using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Application.AcmeImportExport.Queries.Get;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.AcmeProductDetailExport.Commands.Create
{
    /// <summary>
    /// class AcmeProductImportFromCsv extends the IRequest interface of MediatR
    /// </summary>
    public class AcmeProductImportFromCsvRequest : IRequest<AcmeImportFileMetaDataDto>
    {
        public IFormFile formFile { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request , created AcmeProductImportFromCsvRequest class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>

    public class AcmeProductImportFromCsvRequestHandler : IRequestHandler<AcmeProductImportFromCsvRequest, AcmeImportFileMetaDataDto>
    {
        private readonly IAcmeProductFileReaderWriter _AcmeProductReaderWriter;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IAcmeDataAccess _dataAccess;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Instantiates AcmeImportFromCsvHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="fileReaderWriter"></param>

        public AcmeProductImportFromCsvRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IAcmeDataAccess dataAccess, IWebHostEnvironment webHostEnvironment, IAcmeProductFileReaderWriter fileReaderWriter)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
            this._webHostEnvironment = webHostEnvironment;
            this._logger.LogInformation("AcmeProductImportFromCsvRequestHandler - constructor");
            this._AcmeProductReaderWriter = fileReaderWriter;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        public async Task<AcmeImportFileMetaDataDto> Handle(AcmeProductImportFromCsvRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AcmeProductImportFromCsvRequest.Handle - In process");

            AcmeImportFileMetaDataDto fileMetaData = new AcmeImportFileMetaDataDto();

            string path = Path.Combine(this._webHostEnvironment.WebRootPath, "files\\Imports\\");

            try
            {
                //save file in location and get fileId
                fileMetaData = await _AcmeProductReaderWriter.SavePostedFileInSharedLocation(request.formFile, path);
                if (fileMetaData != null)
                {
                    fileMetaData = _AcmeProductReaderWriter.ReadCsvFields(fileMetaData, path);
                }
                //read line from csv file
                string[] lines = File.ReadAllLines(Path.Combine(this._webHostEnvironment.WebRootPath, "files\\Imports\\") + ".csv");
                fileMetaData.FilePath = Path.Combine(this._webHostEnvironment.WebRootPath, "files\\Imports\\");

                //await _dataAccess.Add(fileMetaData);

            }
            catch (Exception ex)
            {
                _logger.LogError($"ImportAcmeRequestHandler.Handle - {ex.Message}");
            }

            return fileMetaData;
        }
    }
}
