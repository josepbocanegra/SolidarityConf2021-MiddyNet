using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace WithoutMiddyNet
{
    public class Greeter
    {
       public async Task<APIGatewayHttpApiV2ProxyResponse> Handler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
       {
            try
            {
                var traceParent = request.Headers.ContainsKey("traceparent")
                    ? request.Headers["traceparent"]
                    : string.Empty;
                var traceParentSplitted = traceParent.Split('-');
                var random = new Random();
                var newParentId = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 16).Select(s => s[random.Next(s.Length)]).ToArray());
                var newTraceParent = $"{traceParentSplitted[0]}-{traceParentSplitted[1]}-{newParentId}-{traceParentSplitted[3]}";

                var traceState = request.Headers.ContainsKey("tracestate")
                    ? request.Headers["tracestate"]
                    : string.Empty;

                var name = request.PathParameters["name"];

                context.Logger.Log($"Greeter called with name {name}, traceparent {newTraceParent}, tracestate {traceState}");

                string greetingsType;
                using (var client = new AmazonSimpleSystemsManagementClient())
                {
                    var response = await client.GetParameterAsync(new GetParameterRequest
                    {
                        Name = "greetingsType",
                        WithDecryption = false
                    }).ConfigureAwait(false);
                    greetingsType = response.Parameter.Value;
                }

                if (greetingsType != "formal" && greetingsType != "colloquial")
                    throw new WrongGreetingTypeException();

                var greeting = greetingsType == "formal" ? "Hello" : "Hi";

                var message = $"{greeting} {name}. Welcome to our online store.";

                context.Logger.Log($"Greeting message generated. traceparent {newTraceParent}, tracestate {traceState} ");

                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Headers = new Dictionary<string, string>
                    {
                        { "traceparent", newTraceParent },
                        { "tracestate", traceState }
                    },
                    StatusCode = 200,
                    Body = message
                };
            }
            catch (WrongGreetingTypeException)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 400,
                    Body = "Wrong greeting type"
                };
            }
            catch (Exception ex)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    StatusCode = 500,
                    Body = ex.Message
                };
            }
       }
    }
}
