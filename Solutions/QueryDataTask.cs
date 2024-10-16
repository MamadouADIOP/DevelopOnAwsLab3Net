using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBOperations
{
    class QueryDataTask
    {
        public async Task Run()
        {
            var configSettings = ConfigSettingsReader<DynamoDBConfigSettings>.Read("DynamoDB");

            try
            {
                var ddbClient = new AmazonDynamoDBClient();

                var tableName = configSettings.TableName;
                var userId = configSettings.QueryUserId;

                Console.WriteLine($"\n************\nQuerying for notes that belong to user {userId}...\n");

                var notes = await QueryNotesByPartitionKey(ddbClient, tableName, userId);
                Print(notes);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        async Task<IEnumerable<Dictionary<string, AttributeValue>>> QueryNotesByPartitionKey(IAmazonDynamoDB ddbClient, string tableName, string userId)
        {
            IEnumerable<Dictionary<string, AttributeValue>> items = null;

            // TODO 5: Add code to query for a specific note with the parameter 
            // values available for use.

            var request = new QueryRequest
            {
                TableName = tableName,
                KeyConditionExpression = "UserId = :userId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":userId", new AttributeValue(userId) }
                },
                ProjectionExpression = "NoteId, Note"
            };

            var response = await ddbClient.QueryAsync(request);
            items = response.Items;

            // End TODO 5
            
            return items;
        }

        void Print(IEnumerable<Dictionary<string, AttributeValue>> notes)
        {
            foreach (var note in notes)
            {
                var json = JsonSerializer.Serialize(new
                {
                    NoteId = note["NoteId"].N.ToString(),
                    Note = note["Note"].S
                });

                Console.WriteLine(json.ToString());
            }
        }
    }
}
