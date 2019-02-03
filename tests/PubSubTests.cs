using nng.Native;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace nng.Tests
{
    using static nng.Tests.Util;

    [Collection("nng")]
    public class PubSubTests
    {
        NngCollectionFixture Fixture;
        IAPIFactory<IMessage> Factory => Fixture.Factory;

        public PubSubTests(NngCollectionFixture collectionFixture)
        {
            Fixture = collectionFixture;
        }

        // [Fact]
        // public async Task BasicPubSub()
        // {
        //     var url = UrlRandomIpc();
        //     var pub = PubSocket<NngMessage>.Create(url);
        //     await Task.Delay(100);
        //     var sub = SubSocket<NngMessage>.Create(url);
        //     var topic = System.Text.Encoding.ASCII.GetBytes("topic");
        //     Assert.True(sub.Subscribe(topic));
        //     var ret = nng_msg_alloc(out var msg, 0);
        //     ret = nng_msg_append(msg, topic);
        //     ret = nng_sendmsg(pub.NngSocket, msg, 0);
        //     ret = nng_recvmsg(sub.NngSocket, out var recv, 0);
        // }

        [Theory]
        [ClassData(typeof(TransportsClassData))]
        public async Task BasicPubSub(string url)
        {
            ISendAsyncContext<IMessage> pub = null;
            ISubAsyncContext<IMessage> sub = null;
            try
            {
                pub = Factory.PublisherCreate(url).Unwrap().CreateAsyncContext(Factory).Unwrap();
                await WaitReady();
                sub = Factory.SubscriberCreate(url).Unwrap().CreateAsyncContext(Factory).Unwrap();
                var topic = TopicRandom();
                Assert.Equal(0, sub.Subscribe(topic));
                var msg = Factory.CreateMessage();
                msg.Append(topic);
                var sendTask = pub.Send(msg);
                var resvTask = sub.Receive(CancellationToken.None);
                await AssertWait(sendTask, resvTask);
            }
            finally
            {
                sub?.Dispose();
                pub?.Dispose();
            }
        }

        [Theory]
        [ClassData(typeof(TransportsClassData))]
        public Task PubSub(string url)
        {
            return Fixture.TestIterate(() => DoPubSub(url));
        }

        Task DoPubSub(string url)
        {
            var topic = TopicRandom();
            var serverReady = new AsyncBarrier(2);
            var clientReady = new AsyncBarrier(2);
            var cts = new CancellationTokenSource();
            var pubTask = Task.Run(async () =>
            {
                using (var socket = Factory.PublisherCreate(url).Unwrap())
                using (var ctx = socket.CreateAsyncContext(Factory).Unwrap())
                {
                    await serverReady.SignalAndWait();
                    await clientReady.SignalAndWait();
                    // Give receivers a chance to actually start receiving
                    await WaitShort();
                    (await ctx.Send(Factory.CreateTopicMessage(topic))).Unwrap();
                }
            });
            var subTask = Task.Run(async () =>
            {
                await serverReady.SignalAndWait();
                using (var socket = Factory.SubscriberCreate(url).Unwrap())
                using (var ctx = socket.CreateAsyncContext(Factory).Unwrap())
                {
                    ctx.Subscribe(topic);
                    await clientReady.SignalAndWait();
                    await ctx.Receive(cts.Token);
                }
            });
            return CancelAfterAssertwait(cts, pubTask, subTask);
        }

        [Theory]
        [ClassData(typeof(TransportsClassData))]
        public async Task BrokerTest(string url)
        {
            await PubSubBrokerAsync(1, 1, 1);
        }

        async Task PubSubBrokerAsync(int numPublishers, int numSubscribers, int numMessagesPerSender)
        {
            // In pub/sub pattern, each message is sent to every receiver
            int numTotalMessages = numPublishers * numSubscribers * numMessagesPerSender;
            var counter = new AsyncCountdownEvent(numTotalMessages);
            var cts = new CancellationTokenSource();

            using(var broker = new Broker(new PubSubBrokerImpl(Factory)))
            {
                var tasks = await broker.RunAsync(numPublishers, numSubscribers, numMessagesPerSender, counter, cts.Token);
                tasks.Add(counter.WaitAsync());
                await CancelAfterAssertwait(tasks, cts);
            }
        }
    }


    class PubSubBrokerImpl : IBrokerImpl<IMessage>
    {
        public IAPIFactory<IMessage> Factory { get; private set; }

        public PubSubBrokerImpl(IAPIFactory<IMessage> factory)
        {
            Factory = factory;
            topic = TopicRandom();
        }

        public IReceiveAsyncContext<IMessage> CreateInSocket(string url)
        {
            var socket = Factory.PullerCreate(url, true).Unwrap();
            var ctx = socket.CreateAsyncContext(Factory).Unwrap();
            disposable.Add(socket);
            disposable.Add(ctx);
            return ctx;
        }

        public ISendAsyncContext<IMessage> CreateOutSocket(string url)
        {
            var socket = Factory.PublisherCreate(url).Unwrap();
            var ctx = socket.CreateAsyncContext(Factory).Unwrap();
            disposable.Add(socket);
            disposable.Add(ctx);
            return ctx;
        }

        public IReceiveAsyncContext<IMessage> CreateClient(string url)
        {
            var socket = Factory.SubscriberCreate(url).Unwrap();
            var ctx = socket.CreateAsyncContext(Factory).Unwrap();
            ctx.Subscribe(topic);
            disposable.Add(socket);
            disposable.Add(ctx);
            return ctx;
        }

        public IMessage CreateMessage()
        {
            return Factory.CreateTopicMessage(topic);
        }

        public void Dispose()
        {
            foreach (var obj in disposable)
            {
                obj.Dispose();
            }
        }

        byte[] topic;
        ConcurrentBag<IDisposable> disposable = new ConcurrentBag<IDisposable>();
    }
}
