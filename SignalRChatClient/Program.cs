using Lection2_Core.Core;
using Microsoft.AspNetCore.SignalR.Client;

var url = "https://localhost:5001/chat";

var connection = new HubConnectionBuilder()
    .WithUrl(url)
    .WithAutomaticReconnect()
    .Build();

connection.On<MessageSnapshot>("GetMessage", PrintMessage);

await connection.StartAsync();
string? input;
do
{
    input = GetNickname();
}
while (!await SetNickname(input));

var nickname = input;

await ShowRecentMessages(connection, nickname);
do
{
    input = GetInputMessage();
    if (input == "/menu")
    {
        await CallMenuAsync();
    }
    else
    {
        await connection.InvokeAsync("SendMessageToAll", input);
    }
} while (!string.IsNullOrEmpty(input));

async Task CallMenuAsync()
{
    int menuItem;
    do
    {
        Console.Clear();
        Console.WriteLine("1 - personal message\n2 - Change nickname\n3 - Change personal color");
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
        case 3:
            var color = GetInputMessage();
            var result = Enum.TryParse(typeof(ConsoleColor), color, out var colorEnum);
            if (result)
            {
                await connection.InvokeAsync(
                    "SetPersonalColor", (ConsoleColor)colorEnum);
            }
            break;
    }

    Console.Clear();
    await ShowRecentMessages(connection, nickname);
}

string nameof(object setPersonalColor)
{
    throw new NotImplementedException();
}

async Task<bool> SetNickname(string? nickname)
{
    var response = await connection.InvokeAsync<bool>("SetNickname", nickname);
    Console.Clear();
    return response;
}

async Task SendPersonalMessage(string nickname, string message)
{
    await connection.InvokeAsync("SendPersonalMessage", nickname, message);
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

static async Task ShowRecentMessages(HubConnection connection, string? nickname)
{
    var messages = await connection.InvokeAsync<IEnumerable<MessageSnapshot>>(
        "GetRecentMessages", 10);

    foreach (var message in messages)
    {
        PrintMessage(message);
    }
}

static void PrintMessage(MessageSnapshot message)
{
    var current = Console.ForegroundColor;
    Console.ForegroundColor = message.SenderUserInfo.Color ?? current;
    Console.WriteLine($"{DateTime.UtcNow}:{message.SenderUserInfo.Nickname}:{message.ReceiverNickname}:{message.Message}");
    Console.ForegroundColor = current;
}