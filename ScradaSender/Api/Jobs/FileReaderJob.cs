using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Runtime.Intrinsics.X86;
using System.Xml;
using System.Xml.Linq;
using Hangfire;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using ScradaSender.Agents.Interfaces;
using ScradaSender.Api.Jobs.Interface;
using ScradaSender.DataAccess.Entities;
using ScradaSender.DataAccess.Repositories.Interfaces;
using ScradaSender.Shared.Options;
using static ScradaSender.Shared.Enums.Enums;

namespace ScradaSender.Api.Jobs
{
    public class FileReaderJob(
        IFileStatussesRepository fileStatusRepository,
        IScradaServiceAgent scradaAgent,
        IOptions<FileReaderSettings> fileReaderOptions,
        ILogger<FileReaderJob> logger) : IFileReaderJob
    {
        private readonly FileReaderSettings fileReaderSettings = fileReaderOptions.Value;

        [DisableConcurrentExecution(timeoutInSeconds: 300)]
        public async Task ReadFilesAsync()
        {
            List<(string fileName, string content)> files;
            List<FileStatusses> entities;

            try
            {
                files = ReadAllFilesInFolder(fileReaderSettings.Path);
                entities = await fileStatusRepository.GetFilesFromNames([.. files.Select(i => i.fileName)]);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Could not read files or database.");
                throw;
            }

            List<FileStatusses> statusses = [];

            foreach (var (fileName, content) in files)
            {
                var entity = entities.SingleOrDefault(i => i.FileName == fileName);

                if (entity != null)
                    continue;

                var xDoc = XDocument.Parse(content);
                var (SupplierScheme, SupplierId, CustomerScheme, CustomerId) = ExtractPartyInfo(xDoc);

                if (SupplierScheme == null || SupplierId == null || CustomerScheme == null || CustomerId == null)
                {
                    logger.LogError("Could not read SupplierScheme, SupplierId, CustomerScheme, CustomerId from file {fileName}.", fileName);
                    statusses.Add(new FileStatusses
                    {
                        FileName = fileName,
                        LastProcessed = DateTime.UtcNow,
                        Status = nameof(Status.ErrorWhileSending),
                        Error = "Could not read ShemeId or VatNumber from xml."
                    });
                    continue;
                }

                var response = await scradaAgent.SendDocumentAsync<string>(xDoc, SupplierScheme, SupplierId, CustomerScheme, CustomerId, Path.GetFileNameWithoutExtension(fileName));
                    
                if(response.Error != null)
                {
                    logger.LogError("Could not process file with filename: {filename}.", fileName);
                    statusses.Add(new FileStatusses
                    {
                        FileName = fileName,
                        LastProcessed = DateTime.UtcNow,
                        Status = nameof(Status.ErrorWhileSending),
                        Error = response.Error
                    });
                    continue;
                }

                statusses.Add(new FileStatusses
                {
                    FileName = fileName,
                    LastProcessed = DateTime.UtcNow,
                    Status = nameof(Status.Sent),
                    PeppolId = response.ResponseObject
                });
            }

            await fileStatusRepository.AddFileStatus(statusses);
            await fileStatusRepository.SaveChangesAsync();
        }

        private static List<(string FileName, string Content)> ReadAllFilesInFolder(string folderPath)
        {
            var files = new List<(string Name, string Content)>();

            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"The folder '{folderPath}' does not exist.");

            foreach (string filePath in Directory.GetFiles(folderPath))
            {
                string fileName = Path.GetFileName(filePath);
                string content = File.ReadAllText(filePath);

                files.Add((fileName, content));
            }

            return files;
        }

        public static (string SupplierScheme, string SupplierId, string CustomerScheme, string CustomerId) ExtractPartyInfo(XDocument xDoc)
        {
            XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
            XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

            var allEndpoints = xDoc.Descendants(cbc + "EndpointID").ToList();

            var supplierEndpoint = allEndpoints.ElementAtOrDefault(0);
            var customerEndpoint = allEndpoints.ElementAtOrDefault(1);

            string supplierScheme = supplierEndpoint?.Attribute("schemeID")?.Value ?? string.Empty;
            string supplierId = supplierEndpoint?.Value ?? string.Empty;

            string customerScheme = customerEndpoint?.Attribute("schemeID")?.Value ?? string.Empty;
            string customerId = customerEndpoint?.Value ?? string.Empty;

            return (supplierScheme, supplierId, customerScheme, customerId);
        }
    }
}
