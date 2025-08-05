using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace SimSDK.Tests
{
    // -------------------------
    // TestServerStreamWriter<T>
    // -------------------------
    public class TestServerStreamWriter<T> : IServerStreamWriter<T>
    {
        public List<T> Written { get; } = new List<T>();

        // Nullable to match IServerStreamWriter<T>
        public WriteOptions? WriteOptions { get; set; } = null;

        public Task WriteAsync(T message)
        {
            Written.Add(message);
            return Task.CompletedTask;
        }
    }

    // -------------------------
    // TestAsyncStreamReader<T>
    // -------------------------
    public class TestAsyncStreamReader<T> : IAsyncStreamReader<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public TestAsyncStreamReader(IEnumerable<T> items)
        {
            _enumerator = items.GetEnumerator();
        }

        public T Current => _enumerator.Current;

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(_enumerator.MoveNext());
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }
    }

    // -------------------------
    // TestServerCallContext
    // -------------------------
    public class TestServerCallContext : ServerCallContext
    {
        private readonly Metadata _requestHeaders;
        private readonly CancellationToken _cancellationToken;
        private readonly string _host;
        private readonly string _method;
        private readonly string _peer;
        private readonly AuthContext _authContext;

        public TestServerCallContext(
            Metadata? requestHeaders = null,
            CancellationToken cancellationToken = default,
            string host = "localhost",
            string method = "testMethod",
            string peer = "localhost",
            AuthContext? authContext = null)
        {
            _requestHeaders = requestHeaders ?? new Metadata();
            _cancellationToken = cancellationToken;
            _host = host;
            _method = method;
            _peer = peer;
            _authContext = authContext ?? new AuthContext("", new Dictionary<string, List<AuthProperty>>());
        }

        protected override string MethodCore => _method;
        protected override string HostCore => _host;
        protected override string PeerCore => _peer;
        protected override DateTime DeadlineCore => DateTime.UtcNow.AddMinutes(1);
        protected override Metadata RequestHeadersCore => _requestHeaders;
        protected override CancellationToken CancellationTokenCore => _cancellationToken;
        protected override Metadata ResponseTrailersCore { get; } = new Metadata();

        // Nullable to match base class
        protected override Status StatusCore { get; set; }
        protected override WriteOptions? WriteOptionsCore { get; set; } = null;
        protected override AuthContext AuthContextCore => _authContext;

       protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options) =>
            throw new NotImplementedException();

        protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders) =>
            Task.CompletedTask;
    }
}