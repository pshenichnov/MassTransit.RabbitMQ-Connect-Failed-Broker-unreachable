using MassTransit;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Receiver
{
    public interface TestCommand
    {
        Guid Id { get; }
    }

    public class TestHandler : IConsumer<TestCommand>
    {
        static int count = 0;

        public Task Consume(ConsumeContext<TestCommand> context)
        {
            var redeliveryCount = context.Headers.Get<string>("MT-Redelivery-Count");

            Log.Information($"MT-Redelivery-Count: {redeliveryCount}; Total: {++count}");

            throw new Exception("something went wrong...");
        }
    }
}
