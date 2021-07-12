using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Confluent.Kafka;
using Consul;
using Microsoft.Data.SqlClient;

namespace ChangeTrackerHandler
{
    class Program
    {
        private const string connectionString = "Data Source=EPBYBREW0106;Initial Catalog=Tmsa;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private const string getCurrentVersionCommand = "SELECT CHANGE_TRACKING_CURRENT_VERSION()";
        private const string keyVersionHandled = "versionHandled";
        private const string consulUri = "http://127.0.0.1:8500";
        private const string kafkaUri = "localhost:9092";
        private const int firstChangeTrackingCommandNumber = 1;
        private static long currentVersion = 1;
        private static readonly Dictionary<int, string> tablesNumbers = new Dictionary<int, string>()
        {
            [0] = "Events",
            [1] = "Clients",
            [2] = "Bookings"
        };
        private static string CommandText(int i) => $"Select * from CHANGETABLE(changes dbo.{tablesNumbers[i]}, {currentVersion}) as bookings";

        static void Main(string[] args)
        {
            OnTimerElapsed(null, null);
            Timer timer = new Timer(30 * 60 * 1000);
            timer.Enabled = true;
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = true;
            Console.ReadLine();
            Console.WriteLine($"Application stopped: {DateTime.Now}");
        }
        static async void OnTimerElapsed(object sender, ElapsedEventArgs args)
        {
            Console.WriteLine($"Starting check: {DateTime.Now}");
            await Job();
            Console.WriteLine($"Check completed: {DateTime.Now}");
        }
        static async Task Job()
        {
            ConsulClient consul = new ConsulClient(c => c.Address = new Uri(consulUri));
            var versionPair = consul.KV.Get(keyVersionHandled).Result.Response;
            string consulVersionHandled = Encoding.UTF8.GetString(versionPair.Value).Trim();
            currentVersion = long.Parse(consulVersionHandled);

            string[] commands = {getCurrentVersionCommand, CommandText(0), CommandText(1), CommandText(2)};
            List<int>[] matrixTrackedEntities = new List<int>[commands.Length - firstChangeTrackingCommandNumber];
            List<string> dbResponses = new List<string>(); // can be simplified

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                for (int j = 0; j < commands.Length; j++)
                {
                    if (j >= firstChangeTrackingCommandNumber)
                    {
                        matrixTrackedEntities[j - firstChangeTrackingCommandNumber] = new List<int>();
                    }
                    var command = new SqlCommand(commands[j], connection);
                    using (var reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            var resultBuilder = new StringBuilder();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var record = reader[i];
                                resultBuilder.Append($"{record}\t");
                                if (j >= firstChangeTrackingCommandNumber && (i + 1) == reader.FieldCount)
                                {
                                    matrixTrackedEntities[j - firstChangeTrackingCommandNumber].Add(int.Parse($"{record}"));
                                }
                            }
                            dbResponses.Add(resultBuilder.ToString().Trim());
                        }
                    }
                }
            }

            if (consulVersionHandled != dbResponses[0]) // versions in db and consul differ
            {
                Console.WriteLine($"Getting modified entities: {DateTime.Now}");
                // get json of interest

                var kafkaMessages = new List<string>();
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    for (int i = 0; i < matrixTrackedEntities.Length; i++) // walk through all tables needed
                    {
                        StringBuilder idRange = new StringBuilder();
                        foreach (var id in matrixTrackedEntities[i])
                        {
                            idRange.Append($"{id}, ");
                        }
                        if (idRange.Length > 2) idRange.Remove(idRange.Length - 2, 2);
                        else continue;
                        string commandText = $"SELECT * FROM dbo.{tablesNumbers[i]} WHERE Id in ({idRange}) FOR JSON PATH, ROOT('{tablesNumbers[i]}')";
                        SqlCommand command = new SqlCommand(commandText, connection);
                        using (var reader = command.ExecuteReader())
                        {
                            await reader.ReadAsync();
                            kafkaMessages.Add($"{reader[0]}");
                        }
                    }
                }

                // kafka message
                Console.WriteLine($"Producing messages in kafka: {DateTime.Now}");

                var config = new ProducerConfig()
                {
                    BootstrapServers = kafkaUri
                };
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    for (int i = 0; i < kafkaMessages.Count; i++)
                    {
                        await producer.ProduceAsync($"{tablesNumbers[i]}-news", new Message<Null, string>() { Value = $"{kafkaMessages[i]}" });
                    }
                }

                // version update

                currentVersion = long.Parse(dbResponses[0]);
                versionPair.Value = Encoding.UTF8.GetBytes(dbResponses[0]);
                await consul.KV.Put(versionPair);
            }
        }
    }
}