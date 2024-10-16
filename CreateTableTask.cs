using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBOperations
{
    class CreateTableTask
    {
        class TableDefinition
        {
            public string TableName { get; set; }
            public string PartitionKey { get; set; }
            public string SortKey { get; set; }
            public long ReadCapacity { get; set; }
            public long WriteCapacity { get; set; }
        }

        public async Task Run()
        {
            var configSettings = ConfigSettingsReader<DynamoDBConfigSettings>.Read("DynamoDB");

            try
            {
                IAmazonDynamoDB ddbClient = null;

                // TODO 1: create an Amazon DynamoDB service client to pass to the
                // main function.
                ddbClient = new AmazonDynamoDBClient();

                // End TODO1

                var tableDefinition = new TableDefinition
                {
                    TableName = configSettings.TableName,
                    PartitionKey = configSettings.PartitionKey,
                    SortKey = configSettings.SortKey,
                    ReadCapacity = configSettings.ReadCapacity,
                    WriteCapacity = configSettings.WriteCapacity
                };

                Console.WriteLine($"\nCreating an Amazon DynamoDB table \"{configSettings.TableName}\"");
                Console.WriteLine($"with a partition key: {configSettings.PartitionKey}");
                Console.WriteLine($"and sort key: {configSettings.SortKey}.\n");

                var createResponse = await CreateTable(ddbClient, tableDefinition);
                Console.WriteLine($"Table Status: {createResponse.TableDescription.TableStatus}");

                Console.WriteLine("\nWaiting for the table to be available...");
                await WaitForTableCreation(ddbClient, configSettings.TableName);

                Console.WriteLine("\nTable is now available.");
                var tableOutput = await GetTableInfo(ddbClient, configSettings.TableName);
                Console.WriteLine($"Table Status: {tableOutput.Table.TableStatus}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        async Task<CreateTableResponse> CreateTable(IAmazonDynamoDB ddbClient, TableDefinition tableDefinition)
        {
            CreateTableResponse response = null;

            // TODO 2: Add logic to create a table with UserId as the 
            // partition key and NoteId as the sort key, and return the
            // table's status.
            var listOfKeySchemaElements = new List<KeySchemaElement> { new KeySchemaElement { AttributeName = tableDefinition.PartitionKey, KeyType = KeyType.HASH } ,
                new KeySchemaElement { AttributeName = tableDefinition.SortKey, KeyType = KeyType.RANGE } };

            var listOfAttributeDefinition = new List<AttributeDefinition> {
                new AttributeDefinition {AttributeName= tableDefinition.PartitionKey, AttributeType = ScalarAttributeType.S },
            new AttributeDefinition{ AttributeName = tableDefinition.SortKey,  AttributeType = ScalarAttributeType.N} };
            var createTableRequest = new CreateTableRequest(tableDefinition.TableName, listOfKeySchemaElements, listOfAttributeDefinition,
                new ProvisionedThroughput { ReadCapacityUnits = tableDefinition.ReadCapacity, WriteCapacityUnits = tableDefinition.WriteCapacity });
            response = await ddbClient.CreateTableAsync(createTableRequest);
            Console.WriteLine($"Table status:{response.TableDescription.TableStatus}");

            // End TODO 2

            return response;
        }

        async Task WaitForTableCreation(IAmazonDynamoDB ddbClient, string tableName)
        {
            // TODO 3: wait for creation of your new table to complete

            while ((await GetTableInfo(ddbClient,tableName)).Table.TableStatus != TableStatus.ACTIVE)
            {
                await Task.Delay(10);
            }
            // End TODO 3
        }

        async Task<DescribeTableResponse> GetTableInfo(IAmazonDynamoDB ddbClient, string tableName)
        {
            return await ddbClient.DescribeTableAsync(tableName);
        }
    }
}
