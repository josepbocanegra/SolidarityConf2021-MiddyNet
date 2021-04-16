using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using FluentAssertions;
using NSubstitute;
using System.Threading.Tasks;
using Voxel.MiddyNet;
using Xunit;

namespace HeaderInjectionMiddlewareExample.Tests
{
    public class HeaderInjectionMiddlewareExampleShould
    {
        [Fact]
        public async Task InjectAHeader()
        {
            var previousResponse = new APIGatewayHttpApiV2ProxyResponse();
            var context = new MiddyNetContext(Substitute.For<ILambdaContext>());
            var headerInjectionMiddleware = new HeaderInjectionMiddleware("headerName", "headerValue");

            var response = await headerInjectionMiddleware.After(previousResponse, context);

            response.Headers.Should().ContainKey("headerName");
            response.Headers["headerName"].Should().Be("headerValue");
        }
    }
}
