using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;

namespace CRMUtils.Extensions
{
    public static class IOrganizationServiceExtensions
    {
        /// <summary>
        /// A utility function to return more then 5000 records from a query
        /// </summary>
        /// <param name="service">The crm service</param>
        /// <param name="query">The query that we want to execute</param>
        /// <returns>A list of all the entities returned by the query</returns>
        public static List<Entity> RetrieveAll(this IOrganizationService service, QueryExpression query)
        {
            List<Entity> output = new List<Entity>();
            bool done = false;

            // Initialize the paging info
            if (query.PageInfo.PageNumber == 0)
            {
                query.PageInfo = new PagingInfo();
                query.PageInfo.PageNumber = 1;
            }

            // While we aren't done with the results keep querying
            while (!done)
            {
                // Submit the query
                EntityCollection result = service.RetrieveMultiple(query);

                // Add the result to the output
                output.AddRange(result.Entities);

                // if this is all of the records?
                if (!result.MoreRecords)
                {
                    done = true;
                }
                else
                {
                    // Increment the paging and set the paging cookie to get the next row
                    query.PageInfo.PageNumber = query.PageInfo.PageNumber + 1;
                    query.PageInfo.PagingCookie = result.PagingCookie;
                }
            }

            return output;
        }
    }

}
