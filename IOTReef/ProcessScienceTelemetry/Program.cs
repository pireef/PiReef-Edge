namespace ProcessScienceTelemetry
{
    using IOTReefLib.Telemetry;
    using Microsoft.Azure.Devices.Client;
    using Newtonsoft.Json;
    using Npgsql;
    using System;
    using System.Runtime.Loader;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

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
            string cnString = "Host=postgres1;Username =postgres;Password=;Database=iotreefdata";

            var moduleClient = userContext as ModuleClient;
            if (moduleClient == null)
            {
                throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
            }

            byte[] messageBytes = message.GetBytes();
            string messageString = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine($"Received message: {counterValue}, Body: [{messageString}]");
            ScienceTelemetry sT = JsonConvert.DeserializeObject<ScienceTelemetry>(messageString);

            var cn = new NpgsqlConnection(cnString);
            await cn.OpenAsync();
            var txn = cn.BeginTransaction();

            if (!string.IsNullOrEmpty(messageString))
            {
                try
                {

                    if (cn.State == System.Data.ConnectionState.Open)
                    {
                        Console.WriteLine("Saving data to DB");
                        var basecmd = new NpgsqlCommand("INSERT INTO devicetelemetry(id, device_id, collectedtime) VALUES(@id, @device_id, @collectedtime)", cn, txn);
                        var gid = Guid.NewGuid();
                        basecmd.Parameters.AddWithValue("id", gid);
                        basecmd.Parameters.AddWithValue("device_id", Guid.Parse(sT.Deviceid));
                        basecmd.Parameters.AddWithValue("collectedtime", sT.Datetime);
                        Console.WriteLine("Execute First Query");
                        try
                        {
                            await basecmd.ExecuteNonQueryAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }

                        Console.WriteLine("Done");

                        var sciencecmd = new NpgsqlCommand("INSERT INTO sciencetelemetry (id, sciencetype, value) VALUES (@id, @sciencetype, @value)", cn, txn);
                        sciencecmd.Parameters.AddWithValue("id", gid);
                        int val = (int)sT.DataType;
                        sciencecmd.Parameters.AddWithValue("sciencetype", val);
                        sciencecmd.Parameters.AddWithValue("value", sT.Value);
                        Console.WriteLine("Execute Second Query");
                        try
                        {
                            await sciencecmd.ExecuteNonQueryAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        Console.WriteLine("Done");

                        Console.WriteLine("Comitting Transactions.");
                        await txn.CommitAsync();
                    }
                    else
                    {
                        throw new NpgsqlException("Connection is not open!");
                    }
                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine("Rollback Transactions");
                    await txn.RollbackAsync();
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    await cn.CloseAsync();
                }
            }


            //if (!string.IsNullOrEmpty(messageString))
            //{
            //    var pipeMessage = new Message(messageBytes);
            //    foreach (var prop in message.Properties)
            //    {
            //        pipeMessage.Properties.Add(prop.Key, prop.Value);
            //    }
            //    await moduleClient.SendEventAsync("output1", pipeMessage);
            //    Console.WriteLine("Received message sent");
            //}
            return MessageResponse.Completed;
        }
    }
}
