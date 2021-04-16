using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voxel.MiddyNet;
using Voxel.MiddyNet.ProblemDetailsMiddleware;
using Voxel.MiddyNet.SSMMiddleware;
using Voxel.MiddyNet.Tracing.ApiGatewayMiddleware;
using HeaderInjectionMiddlewareExample;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace WithMiddyNet
{
    public class Greeter : MiddyNet<APIGatewayHttpApiV2ProxyRequest, APIGatewayHttpApiV2ProxyResponse>
    {
        public Greeter()
        {
            Use(new ApiGatewayHttpApiV2TracingMiddleware());
            Use(new ProblemDetailsMiddlewareV2(new ProblemDetailsMiddlewareOptions().Map<WrongGreetingTypeException>(400)));
            Use(new SSMMiddleware<APIGatewayHttpApiV2ProxyRequest, APIGatewayHttpApiV2ProxyResponse>(new SSMOptions
                {
                    ParametersToGet = new List<SSMParameterToGet> { new SSMParameterToGet("greetingsType", "greetingsType") }
                }));       
        }

        protected override Task<APIGatewayHttpApiV2ProxyResponse> Handle(APIGatewayHttpApiV2ProxyRequest request, MiddyNetContext context)
        {
                var name = request.PathParameters["name"];

                context.Logger.Log(LogLevel.Info, $"Greeter called with name {name}");

                string greetingsType = context.AdditionalContext["greetingsType"].ToString();

                if (greetingsType != "formal" && greetingsType != "colloquial")
                    throw new WrongGreetingTypeException();

                var greeting = greetingsType == "formal" ? "Hello" : "Hi";

                var message = $"{greeting} {name}. Welcome to our online store.";

                context.Logger.Log(LogLevel.Info, "Greeting message generated.");

                return Task.FromResult(new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 200,
                    Body = message
                });
        }
    }
}