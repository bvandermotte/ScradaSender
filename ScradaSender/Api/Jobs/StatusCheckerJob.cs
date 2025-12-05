using Hangfire;
using ScradaSender.Agents.Interfaces;
using ScradaSender.Agents.Models;
using ScradaSender.Api.Jobs.Interface;
using ScradaSender.DataAccess.Entities;
using ScradaSender.DataAccess.Repositories.Interfaces;
using static ScradaSender.Shared.Enums.Enums;

namespace ScradaSender.Api.Jobs
{
    public class StatusCheckerJob(
        IScradaServiceAgent scradaAgent,
        IFileStatussesRepository fileStatusRepository,
        ILogger<StatusCheckerJob> logger) : IStatusCheckerJob
    {
        [DisableConcurrentExecution(timeoutInSeconds: 300)]
        public async Task CheckStatusAsync()
        {
            List<FileStatusses> sentDocuments;

            try
            {
                sentDocuments = await fileStatusRepository.GetFilesWithStatusSent();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Could not reach database.");
                return;
            }

            if (sentDocuments.Count == 0)
                return;

            foreach (var sentDocument in sentDocuments)
            {
                var response = await scradaAgent.CheckDocumentStatusAsync<OutboundDocumentStatus>(sentDocument.PeppolId);

                if(response.Error != null)
                {
                    logger.LogError("Could not get status for file {fileName}. Error: {error}", sentDocument.FileName, response.Error);
                    continue;
                }

                if(response.ResponseObject.Status == "Error")
                {
                    sentDocument.Status = nameof(Status.ErrorOnPeppol);
                    sentDocument.Error = response.ResponseObject.ErrorMessage;
                    sentDocument.LastProcessed = DateTime.UtcNow;
                }
                else
                {
                    sentDocument.Status = nameof(Status.Success);
                    sentDocument.LastProcessed = DateTime.UtcNow;
                }
            }

            try
            {
                await fileStatusRepository.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Could not save statusses to database.");
            }
        }
    }
}
