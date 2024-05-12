using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Azure.Core;
using Newtonsoft.Json;
using StudyHub.Domain.Enums;
using StudyHub.Infrastructure.Services;
using Telegram.Bot.Types.ReplyMarkups;
using StudyHub.Application.Handlers;
using StudyHub.Domain.Models;

namespace StudyHub
{
    public class StudyHubFunction
    {
        private readonly ILogger<StudyHubFunction> _logger;
        private readonly ITelegramBotClient _telegramBot;
        private readonly ICommandProcessor _commandProcessor;
        private readonly ICacheService _cacheService;
        private readonly Dictionary<string, Commands> CommandMappings = new Dictionary<string, Commands>
        {
            { "/remind", Commands.Remind },
            { "/feedback", Commands.GetFeedback },
            { "/getReminders", Commands.GetReminders }
        };

        public StudyHubFunction(
            ILogger<StudyHubFunction> logger,
            ITelegramBotClient telegramBot,
            ICommandProcessor commandProcessor,
            ICacheService cacheService)
        {
            _logger = logger;
            _telegramBot = telegramBot;
            _commandProcessor = commandProcessor;
            _cacheService = cacheService;
        }

        [Function("StudyHub")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation(requestBody);

            var update = JsonConvert.DeserializeObject<Update>(requestBody);

            if (update == null)
            {
                _logger.LogError("Failed to deserialize update.");
                return new BadRequestResult();
            }

            try
            {
                await HandleUpdate(update, CancellationToken.None);

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await _telegramBot.SendTextMessageAsync(
                   update.Message.Chat.Id,
                   $"Something went wrong &#129430;",
                   disableWebPagePreview: true,
                   parseMode: ParseMode.Html);
            }

            return new OkResult();
        }

        private async Task HandleUpdate(Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message or
                UpdateType.EditedMessage => BotOnMessageReceived(update.Message, cancellationToken),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery, cancellationToken),
                UpdateType.InlineQuery => BotOnInlineQueryReceived(update.InlineQuery, cancellationToken),
                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(update.ChosenInlineResult, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update, cancellationToken)
            };

            await handler;
        }

        private async Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task BotOnChosenInlineResultReceived(ChosenInlineResult? chosenInlineResult, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task BotOnInlineQueryReceived(InlineQuery? inlineQuery, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task BotOnCallbackQueryReceived(CallbackQuery? callbackQuery, CancellationToken cancellationToken)
        {
            await HandleCallbackQuery(callbackQuery);
        }

        private async Task BotOnMessageReceived(Message? message, CancellationToken cancellationToken)
        {
            var command = message.Text.Split(new[] { ' ' })[0];

            if (!CommandMappings.TryGetValue(command, out var parsedCommand))
            {
                await _telegramBot.SendTextMessageAsync(
                    message.Chat.Id,
                    "I don't think that command exists!",
                    disableWebPagePreview: true,
                    parseMode: ParseMode.Html);

                return;
            }

            var response = await _commandProcessor.HandleCommand(message, parsedCommand, cancellationToken);
            if (response.CommandType == Commands.Remind)
            {
                await _cacheService.SetCachedReminderTitle(message.Chat.Id.ToString(), response.Response as string);
            }

            switch (response.CommandType)
            {
                case Commands.Undefined:
                    break;
                case Commands.Remind:
                    await _telegramBot.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Please choose a month:",
                        replyMarkup: AddReminderCommandHandler.GetMonthSelection());
                    break;
                case Commands.AddFeedback:
                    break;
                case Commands.GetFeedback:
                    break;
                case Commands.GetReminders:
                    var reminders = response.Response as List<Reminder>;
                    await _telegramBot.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Your reminders:\n" + string.Join("\n", reminders.Select(s => $"&#128073 <code> {s.Text} {s.SendTime} </code>")),
                        parseMode: ParseMode.Html);
                    break;
                default:
                    break;
            }
            
        }

        public async Task HandleCallbackQuery(CallbackQuery callbackQuery)
        {
            var parts = callbackQuery.Data.Split('_');
            var command = parts[0];

            int year = DateTime.Now.Year, month = 0, day = 0;
            DateTime selectedTime;

            switch (command)
            {
                case "month":
                    month = int.Parse(parts[1]);

                    await _telegramBot.EditMessageTextAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        messageId: callbackQuery.Message.MessageId,
                        text: "Please choose a day:",
                        replyMarkup: AddReminderCommandHandler.GetDaySelection(year, month)
                    );
                    break;

                case "date":
                    year = int.Parse(parts[1]);
                    month = int.Parse(parts[2]);
                    day = int.Parse(parts[3]);

                    await _telegramBot.EditMessageTextAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        messageId: callbackQuery.Message.MessageId,
                        text: "Please choose a time:",
                        replyMarkup: AddReminderCommandHandler.GetTimeSelection(year, month, day)
                    );
                    break;

                case "time":
                    year = int.Parse(parts[1]);
                    month = int.Parse(parts[2]);
                    day = int.Parse(parts[3]);
                    int hour = int.Parse(parts[4]);
                    int minute = int.Parse(parts[5]);

                    selectedTime = new DateTime(year, month, day, hour, minute, 0);

                    await _telegramBot.EditMessageTextAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        messageId: callbackQuery.Message.MessageId,
                        text: $"You selected: {selectedTime}",
                        replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Confirm", $"confirm_{year}_{month}_{day}_{hour}_{minute}_0"))
                    );
                    break;

                case "confirm":
                    var title = await _cacheService.GetCachedReminderTitle(callbackQuery.Message.Chat.Id.ToString());
                    callbackQuery.Data += $"_{title}";
                    await _commandProcessor.HandleCommand(callbackQuery, Commands.Remind);
                    await _telegramBot.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Your reminder has been set!"
                    );
                    break;
            }
        }
    }
}
