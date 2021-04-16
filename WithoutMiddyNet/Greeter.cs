using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace WithoutMiddyNet
{
    public class Greeter
    {
       public APIGatewayHttpApiV2ProxyResponse Handler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
       {
            var name = request.PathParameters["name"];

            var message = $"Hello {name}. Welcome to our online store.";
            
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = message
            };
       }
    }
}
