using nng.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace nng.Tests
{
    using static nng.Tests.Util;

    [Collection("nng")]
    public class PairTests
    {
        NngCollectionFixture Fixture;
        IAPIFactory<IMessage> Factory => Fixture.Factory;

        public PairTests(NngCollectionFixture collectionFixture)
        {
            Fixture = collectionFixture;
        }

        [Theory]
        [ClassData(typeof(TransportsClassData))]
        public Task Pair(string url)
        {
            return Fixture.TestIterate(() => DoPair(url));
        }

        Task DoPair(string url)
        {
            var barrier = new AsyncBarrier(2);
            var cts = new CancellationTokenSource();
            var push = Task.Run(async () =>
            {
                using (var socket = Factory.PairCreate(url, true).Unwrap().CreateAsyncContext(Factory).Unwrap())
                {
                    await barrier.SignalAndWait();
                    Assert.True(await socket.Send(Factory.CreateMessage()));
                    await WaitShort();
                }
            });
            var pull = Task.Run(async () =>
            {
                await barrier.SignalAndWait();
                using (var socket = Factory.PairCreate(url, false).Unwrap().CreateAsyncContext(Factory).Unwrap())
                {
                    await socket.Receive(cts.Token);
                }
            });
            cts.CancelAfter(DefaultTimeoutMs);
            return Task.WhenAll(pull, push);
        }

        [Theory]
        [ClassData(typeof(TransportsNoWsClassData))]
        public Task PairShared(string url)
        {
            return Fixture.TestIterate(() => DoPairShared(url));
        }

        async Task DoPairShared(string url)
        {
            int numListeners = 2;
            int numDialers = 2;

            var listenerReady = new AsyncBarrier(numListeners + 1);
            var dialerReady = new AsyncBarrier(numListeners + numDialers);
            var cts = new CancellationTokenSource();

            using (var listenSocket = Factory.PairCreate(url, true).Unwrap())
            using (var dialerSocket = Factory.PairCreate(url, false).Unwrap())
            {
                var tasks = new List<Task>();

                // On listening socket create send/receive AIO
                {
                    var task = Task.Run(async () =>
                    {
                        var ctx = listenSocket.CreateAsyncContext(Factory).Unwrap();
                        await listenerReady.SignalAndWait();
                        await dialerReady.SignalAndWait();
                        while (!cts.IsCancellationRequested)
                        {
                            var msg = await ctx.Receive(cts.Token);
                        }
                    });
                    tasks.Add(task);
                    task = Task.Run(async () =>
                    {
                        var ctx = listenSocket.CreateAsyncContext(Factory).Unwrap();
                        await listenerReady.SignalAndWait();
                        await dialerReady.SignalAndWait();
                        while (!cts.IsCancellationRequested)
                        {
                            var _ = await ctx.Send(Factory.CreateMessage());
                        }
                    });
                    tasks.Add(task);
                }

                await listenerReady.SignalAndWait();

                // On dialing socket create send/receive AIO
                {
                    var task = Task.Run(async () =>
                    {
                        var ctx = dialerSocket.CreateAsyncContext(Factory).Unwrap();
                        await dialerReady.SignalAndWait();
                        while (!cts.IsCancellationRequested)
                        {
                            var msg = await ctx.Receive(cts.Token);
                        }
                    });
                    tasks.Add(task);
                    task = Task.Run(async () =>
                    {
                        var ctx = dialerSocket.CreateAsyncContext(Factory).Unwrap();
                        await dialerReady.SignalAndWait();
                        while (!cts.IsCancellationRequested)
                        {
                            var _ = await ctx.Send(Factory.CreateMessage());
                        }
                    });
                    tasks.Add(task);
                }

                await Util.CancelAfterAndWait(tasks, cts, DefaultTimeoutMs);
            }
        }
    }
}
