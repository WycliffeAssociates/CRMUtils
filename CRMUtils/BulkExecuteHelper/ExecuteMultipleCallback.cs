using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMUtils.BulkExecuteHelper
{
    public class ExecuteMultipleCallback
    {
        public ExecuteMultipleCallback(OrganizationRequest request, Action<OrganizationResponse> callback)
        {
            this.Request = request;
            this.CallBack = callback;
        }
        public Action<OrganizationResponse> CallBack;
        public OrganizationRequest Request;
    }
}
