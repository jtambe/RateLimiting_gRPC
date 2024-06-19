using Grpc.Core;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using gRPC_RateLimiter;


namespace gRPC_RateLimiter.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        [EnableRateLimiting("fixed")]
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        [EnableRateLimiting("sliding")]
        public override Task<HelloReply> SayHelloSliding(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        [EnableRateLimiting("concurrency")]
        public override Task<HelloReplyList> SayHelloList(HelloRequest request, ServerCallContext context)
        {
            var res = new HelloReplyList();
            var msges = new[] { "msg1", "msg2" };
            res.Messages.Add(msges);

            return Task.FromResult(res);
        }

        [EnableRateLimiting("token")]
        public override Task<HelloReplyDictionary> SayHelloDictionary(HelloRequest request, ServerCallContext context)
        {
            var res = new HelloReplyDictionary();
            res.Messages.Add(1, "msg1");
            res.Messages.Add(2, "msg2");

            return Task.FromResult(res);
        }
    }
}