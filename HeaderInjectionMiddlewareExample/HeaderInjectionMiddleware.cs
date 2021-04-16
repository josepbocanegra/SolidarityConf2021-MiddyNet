using Amazon.Lambda.APIGatewayEvents;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voxel.MiddyNet;

namespace HeaderInjectionMiddlewareExample
{
    public class HeaderInjectionMiddleware : ILambdaMiddleware<APIGatewayHttpApiV2ProxyRequest, APIGatewayHttpApiV2ProxyResponse>
    {
        private readonly string headerName;
        private readonly string headerValue;

        public HeaderInjectionMiddleware(string headerName, string headerValue)
        {
            this.headerName = headerName;
            this.headerValue = headerValue;
        }
        public Task<APIGatewayHttpApiV2ProxyResponse> After(APIGatewayHttpApiV2ProxyResponse lambdaResponse, MiddyNetContext context)
        {
            if (lambdaResponse.Headers == null)
                lambdaResponse.Headers = new Dictionary<string, string>();
            lambdaResponse.Headers.Add(new KeyValuePair<string, string>(headerName, headerValue));
            return Task.FromResult(lambdaResponse);
        }

        public Task Before(APIGatewayHttpApiV2ProxyRequest lambdaEvent, MiddyNetContext context) => Task.CompletedTask;
    }
}
