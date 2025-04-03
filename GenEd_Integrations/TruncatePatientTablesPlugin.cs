using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace GenEd_Integrations
{
    public class TruncatePatientTablesPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                DeleteAllRecords(service, "cr480_accountinformation", tracer);
                DeleteAllRecords(service, "cr480_form190", tracer);
            }
            catch (Exception ex)
            {
                tracer.Trace("Plugin Error: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("Error during truncation.", ex);
            }
        }

        private void DeleteAllRecords(IOrganizationService service, string entityLogicalName, ITracingService tracer)
        {
            QueryExpression query = new QueryExpression(entityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                PageInfo = new PagingInfo { PageNumber = 1, Count = 5000 }
            };

            EntityCollection results;
            int totalDeleted = 0;

            do
            {
                results = service.RetrieveMultiple(query);

                foreach (var record in results.Entities)
                {
                    service.Delete(entityLogicalName, record.Id);
                    totalDeleted++;
                }

                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = results.PagingCookie;

            } while (results.MoreRecords);

            tracer.Trace($"Deleted {totalDeleted} records from {entityLogicalName}");
        }
    }
}
