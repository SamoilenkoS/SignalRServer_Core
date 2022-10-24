using Microsoft.AspNetCore.SignalR.Client;

var url = "https://localhost:5001/chat";

var connection = new HubConnectionBuilder()
    .WithUrl(url)
    .WithAutomaticReconnect()
    .Build();

// receive a message from the hub
connection.On<string>("GetMessage", (message) => Console.WriteLine(message));

await connection.StartAsync();

string input;
do
{
    input = Console.ReadLine();
    await connection.InvokeAsync("SendMessageToAll", input);
} while (!string.IsNullOrEmpty(input));
// send a message to the hub
