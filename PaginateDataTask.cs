using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBOperations
{
    class PaginateDataTask
    {
        public async Task Run()
        {
            var configSettings = ConfigSettingsReader<DynamoDBConfigSettings>.Read("DynamoDB");

            try
            {
                var ddbClient = new AmazonDynamoDBClient();

                var tableName = configSettings.TableName;
                var pageSize = configSettings.PageSize;

                Console.WriteLine("\nScanning with pagination...\n");

                await QueryAllNotesPaginator(ddbClient, tableName, pageSize);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        async Task QueryAllNotesPaginator(IAmazonDynamoDB ddbClient, string tableName, int pageSize)
        {
            // TODO 6: Add code that creates a paginator and prints the returned items


            // End TODO 6
        }

        void Print(IEnumerable<Dictionary<string, AttributeValue>> notes)
        {
            foreach (var note in notes)
            {
                var json = JsonSerializer.Serialize(new
                {
                    UserId = note["UserId"].S,
                    NoteId = note["NoteId"].N.ToString(),
                    Note = note["Note"].S
                });

                Console.WriteLine(json.ToString());
            }
        }
    }
}
