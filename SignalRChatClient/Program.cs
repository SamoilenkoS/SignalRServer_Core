using Lection2_Core.Core;
using Microsoft.AspNetCore.SignalR.Client;

var url = "https://localhost:5001/chat";

var connection = new HubConnectionBuilder()
    .WithUrl(url)
    .WithAutomaticReconnect()
    .Build();

connection.On<string>(nameof(ISignalRClient.GetMessage), (message) => Console.WriteLine(message));

await connection.StartAsync();
string? input;
do
{
    input = GetNickname();
}
while (!await SetNickname(input));

do
{
    input = GetInputMessage();
    if (input == "/menu")
    {
        await CallMenuAsync();
    }
    else
    {
        await connection.InvokeAsync(nameof(ISignalRServer.SendMessageToAll), input);
    }
} while (!string.IsNullOrEmpty(input));

async Task CallMenuAsync()
{
    int menuItem;
    do
    {
        Console.Clear();
        Console.WriteLine("1 - personal message\n2 - Change nickname\n3 - Global message");
    } while (!int.TryParse(Console.ReadLine(), out menuItem));

    switch (menuItem)
    {
        case 1:
            var nickname = GetNickname();
            var message = GetInputMessage();
            await SendPersonalMessage(nickname, message);
            break;
        case 2:
            nickname = GetNickname();
            await SetNickname(nickname);
            break;
    }

    Console.Clear();
}

async Task<bool> SetNickname(string? nickname)
{
    var response = await connection.InvokeAsync<bool>(nameof(ISignalRServer.SetNickname), nickname);
    Console.Clear();
    return response;
}

async Task SendPersonalMessage(string nickname, string message)
{
    await connection.InvokeAsync(nameof(ISignalRServer.SendPersonalMessage), nickname, message);
}

static string? GetNickname()
{
    Console.Write("Enter nickname: ");
    return Console.ReadLine();
}

static string? GetInputMessage()
{
    return Console.ReadLine();
}