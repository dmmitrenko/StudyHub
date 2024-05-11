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

namespace StudyHub
{
    public class StudyHubFunction
    {
        private readonly ILogger<StudyHubFunction> _logger;
        private readonly ITelegramBotClient _telegramBot;
        private readonly ICommandProcessor _commandProcessor;
        private readonly Dictionary<string, Commands> CommandMappings = new Dictionary<string, Commands>
        {
            { "/remind", Commands.Remind },
        };

        public StudyHubFunction(
            ILogger<StudyHubFunction> logger,
            ITelegramBotClient telegramBot,
            ICommandProcessor commandProcessor)
        {
            _logger = logger;
            _telegramBot = telegramBot;
            _commandProcessor = commandProcessor;
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
            var isHelpNeeded = message.Text.Contains("-help");

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

            await _telegramBot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Choose date:",
                replyMarkup: response.Response as InlineKeyboardMarkup
            );
        }

        public async Task HandleCallbackQuery(CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data.StartsWith("choose_"))
            {
                string[] parts = callbackQuery.Data.Split('_');
                int year = int.Parse(parts[1]);
                int month = int.Parse(parts[2]);
                int day = int.Parse(parts[3]);
                DateTime chosenDate = new DateTime(year, month, day);

                await _telegramBot.AnswerCallbackQueryAsync(callbackQuery.Id);
                await _telegramBot.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: $"You selected: {chosenDate.ToShortDateString()}"
                );
            }
        }
    }
}
