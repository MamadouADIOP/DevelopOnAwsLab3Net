using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

namespace DynamoDBOperations
{
    class LoadDataTask
    {
        public async Task Run()
        {
            var configSettings = ConfigSettingsReader<DynamoDBConfigSettings>.Read("DynamoDB");

            try
            {
                var ddbClient = new AmazonDynamoDBClient();

                var tableName = configSettings.TableName;
                var jsonFilename = configSettings.Sourcenotes;

                var pathedJsonFilename = Path.Join(Environment.CurrentDirectory, jsonFilename);

                var jsonString = await File.ReadAllTextAsync(pathedJsonFilename);
                var json = JsonDocument.Parse(jsonString);

                var table = Table.LoadTable(ddbClient, tableName);

                Console.WriteLine($"\nLoading {tableName} table with data from file {jsonFilename}\n");

                var root = json.RootElement;
                foreach (var note in root.EnumerateArray())
                {
                    await PutNote(table, note);
                }

                Console.WriteLine("\nFinished loading notes from the JSON file.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        async Task PutNote(Table table, JsonElement note)
        {
            Console.WriteLine($"Loading note {note}");
            //var noteString = note.GetRawText();
            //var pattern = "(?<NoteId>\"NoteId\":\\s*)\"(?<ValueNoteId>\\d+)\"";
            //var result = Regex.Replace(noteString, pattern, evaluator);



            // TODO 4: Add code that uses the function parameters to 
            // add a new note to the table.
            //var doc = Document.FromJson(result);
            var UserId = "UserId";
            var NoteId = "NoteId";
            var Note = "Note";
            var doc = new Document
            {
                [UserId] = note.GetProperty(UserId).GetString(),
                [NoteId] =int.Parse(note.GetProperty(NoteId).GetString()),
                [Note] = note.GetProperty(Note).GetString(),
            };

           var x = doc.Values;
            await table.PutItemAsync(doc);

            // End TODO 4
        }

        private string evaluator(Match match)
        {
            return match.Groups["NoteId"].Value +int.Parse(match.Groups["ValueNoteId"].Value).ToString();
        }
    }
}
