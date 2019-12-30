namespace ProcessTelemetry
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Client.Transport.Mqtt;
    using IOTReefLib.Telemetry;
    using Newtonsoft.Json;
    using Npgsql;
    using System.Diagnostics;

    class Program
    {
        static int counter;

        static void Main(string[] args)
        {
            Init().Wait();

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            WhenCancelled(cts.Token).Wait();
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }

        /// <summary>
        /// Initializes the ModuleClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        static async Task Init()
        {
            AmqpTransportSettings amqpSetting = new AmqpTransportSettings(TransportType.Amqp_Tcp_Only);
            ITransportSettings[] settings = { amqpSetting };

            // Open a connection to the Edge runtime
            ModuleClient ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            await ioTHubModuleClient.OpenAsync();
            Console.WriteLine("IoT Hub module client initialized.");

            // Register callback to be called when a message is received by the module
            await ioTHubModuleClient.SetInputMessageHandlerAsync("input1", PipeMessage, ioTHubModuleClient);
        }

        /// <summary>
        /// This method is called whenever the module is sent a message from the EdgeHub. 
        /// It just pipe the messages without any change.
        /// It prints all the incoming messages.
        /// </summary>
        static async Task<MessageResponse> PipeMessage(Message message, object userContext)
        {
            int counterValue = Interlocked.Increment(ref counter);

            var moduleClient = userContext as ModuleClient;
            if (moduleClient == null)
            {
                throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
            }

            byte[] messageBytes = message.GetBytes();
            string messageString = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine($"Received message: {counterValue}, Body: [{messageString}]");

            if (!string.IsNullOrEmpty(messageString))
            {
                await DetermineMessageType(messageString);
                //this stuff here pipes the message upstream. 
                //var pipeMessage = new Message(messageBytes);
                //foreach (var prop in message.Properties)
                //{
                //    pipeMessage.Properties.Add(prop.Key, prop.Value);
                //}
                //await moduleClient.SendEventAsync("output1", pipeMessage);
                //Console.WriteLine("Received message sent");
            }
            return MessageResponse.Completed;
        }

        static async Task<MessageResponse> DetermineMessageType(string msg)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            TelemetryBase tbase = JsonConvert.DeserializeObject<TelemetryBase>(msg);
            
            switch(tbase.Type)
            {
                case TelemetryType.Doser:
                    Console.WriteLine("Doser Telemetry");
                    await ProcessDoserTelemetry(msg);
                    break;
                case TelemetryType.Sience:
                    Console.WriteLine("Science Telemetry");
                    await ProcessScienceTelemetry(msg);
                    break;
                case TelemetryType.Power:
                    Console.WriteLine("Power Telemetry");
                    await ProcessPowerTelemetry(msg);
                    break;
                default:
                    Console.WriteLine("Unknown Telemetry");
                    break;
            }
            sw.Stop();
            Console.WriteLine("Operation took {0} millseconds", sw.ElapsedMilliseconds);
            return MessageResponse.None;
        }

        private static Task ProcessPowerTelemetry(string msg)
        {
            throw new NotImplementedException();
        }

        private static Task ProcessScienceTelemetry(string msg)
        {
            throw new NotImplementedException();
        }

        private static async Task ProcessDoserTelemetry(string msg)
        {
            string cnString = "Host=postgres;Username=postgres;Password=;Database=iotreefdata";
            DoserTelemetry doserTelemetry = JsonConvert.DeserializeObject<DoserTelemetry>(msg);
            var cn = new NpgsqlConnection(cnString);
            //NpgsqlTransaction txn;

            try
            {
                await cn.OpenAsync();
                var txn = cn.BeginTransaction();
                //var txn = await cn.BeginTransactionAsync(); this compiles here but does not build in docker??
                
                var basecmd = new NpgsqlCommand("INSERT INTO devicetelemetry (id, deviceid, collectedtime) VALUES (@id, @deviceid, @collectedtime)", cn, txn);
                var gid = Guid.NewGuid();
                basecmd.Parameters.AddWithValue("id", gid);
                basecmd.Parameters.AddWithValue("deviceid", doserTelemetry.Deviceid);
                basecmd.Parameters.AddWithValue("collectedtime", doserTelemetry.Datetime);
                await basecmd.ExecuteNonQueryAsync();

                var dosercmd = new NpgsqlCommand("INSERT INTO dosertelemetry (id, dosername, amtdosed) VALUES (@id, @dosername, @amtdosed)", cn, txn);
                dosercmd.Parameters.AddWithValue("id", gid);
                dosercmd.Parameters.AddWithValue("dosername", doserTelemetry.Name);
                dosercmd.Parameters.AddWithValue("amtdosed", doserTelemetry.Amtdosed);
                await dosercmd.ExecuteNonQueryAsync();

                await txn.CommitAsync();
                await cn.CloseAsync();
            }
            catch(NpgsqlException ex)
            {
                //await txn.RollbackAsync();
                Console.WriteLine(ex.ToString());       
            }
            finally
            {
                await cn.CloseAsync();
            }
            return;
        }
    }
}
