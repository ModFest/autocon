﻿using Discord;
using Discord.WebSocket;
using Models;
using Tasks;

Task Log(LogMessage message) {
	Console.WriteLine(message.ToString());
	return Task.CompletedTask;
}

var config = Config.Read();
var properties = Properties.Read();

var client = new DiscordSocketClient();
var token = Environment.GetEnvironmentVariable("AUTOCON_TOKEN");

client.Log += Log;
client.Ready += () => Ready.OnReady(client, config);
client.SlashCommandExecuted += (command) => Commands.OnCommand(command, properties);

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();

await Task.Delay(-1);
