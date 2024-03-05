using AirBro.TelegramBot;
using AirBro.TelegramBot.Handlers;
using AirBro.TelegramBot.Models;

const string botKey = "6862075580:AAGGDxC9Ut0p0OrweBjl-2FsUCLN-7ZFS30";
const string iqAirKey = "499036c0-fec3-4dc1-b256-4452b296b7e1";

using CancellationTokenSource cts = new ();
Dictionary<long, UserProfile> usersData = new();

IBotHandlers handlers = new BotHandlers(iqAirKey, usersData);
var bot = new AirBroBot(botKey, handlers);

await bot.StartReceivingAsync(cts.Token);
// Send cancellation request to stop bot
cts.Cancel();

