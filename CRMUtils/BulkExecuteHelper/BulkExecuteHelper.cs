using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMUtils.BulkExecuteHelper
{
    /// <summary>
    /// This class allows you queue up a list of CRM actions along with handlers for the result of the action
    /// </summary>
    public class BulkExecuteHelper
    {
        public const int MAX_BATCH_SIZE = 1000;
        /// <summary>
        /// Contstructor for a new Bulk Execute Helper
        /// </summary>
        /// <param name="size"></param>
        public BulkExecuteHelper(int size = 800)
        {
            // This is set to a safe default of 800. When you get closer to 1000 you can run into failures
            // Test that the size isn't greater then the CRM max
            if (size > MAX_BATCH_SIZE)
            {
                throw new ArgumentOutOfRangeException("size", "Size is greater then the maximum size of 1000");
            }
            this.Requests = new List<ExecuteMultipleCallback>();
            this.batchSize = size;
        }

        /// <summary>
        /// The number of requests to add to a batch. M
        /// </summary>
        private readonly int batchSize;

        /// <summary>
        /// A list of requests
        /// </summary>
        public List<ExecuteMultipleCallback> Requests;

        /// <summary>
        /// Add a new item to the requests
        /// </summary>
        /// <param name="request">The organization request to add</param>
        /// <param name="callback">The function to call when the request is done. (Null if you don't want a callback)</param>
        public void Add(OrganizationRequest request, Action<OrganizationResponse> callback = null)
        {
            this.Requests.Add(new ExecuteMultipleCallback(request, callback));
        }

        /// <summary>
        /// Add a new retrive multiple request to the requests
        /// </summary>
        /// <param name="query">A query to add</param>
        /// <param name="callback">The function to call when the request is done. (Null if you don't want a callback)</param>
        public void AddQuery(QueryBase query, Action<OrganizationResponse> callback = null)
        {
            this.Requests.Add(new ExecuteMultipleCallback(new RetrieveMultipleRequest() { Query = query }, callback));
        }

        /// <summary>
        /// Execute the entire batch
        /// </summary>
        /// <param name="service"></param>
        public void Execute(IOrganizationService service)
        {
            ExecuteMultipleRequest batch = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ReturnResponses = true,
                    ContinueOnError = false,
                },
                Requests = new OrganizationRequestCollection(),
            };

            int numberOfBatches = 0;
            for (var i = 0; i < Requests.Count; i++)
            {
                batch.Requests.Add(Requests[i].Request);
                // If the batch size has been met or this is the last request execute the batch
                if ((i + 1) % batchSize == 0 || i == Requests.Count - 1)
                {
                    ExecuteMultipleResponse response = (ExecuteMultipleResponse)service.Execute(batch);
                    for (var ii = 0 ; ii < response.Responses.Count; ii++)
                    {
                        if (Requests[numberOfBatches * this.batchSize + ii].CallBack != null)
                        {
                            Requests[numberOfBatches * this.batchSize + ii]?.CallBack(response.Responses[ii].Response);
                        }
                    }
                    batch.Requests.Clear();
                    numberOfBatches++;
                }
            }
        }
    }
}
