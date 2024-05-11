using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Azure.Core;
using Newtonsoft.Json;

namespace StudyHub
{
    public class StudyHubFunction
    {
        private readonly ILogger<StudyHubFunction> _logger;
        private readonly ITelegramBotClient _telegramBot;

        public StudyHubFunction(
            ILogger<StudyHubFunction> logger,
            ITelegramBotClient telegramBot)
        {
            _logger = logger;
            _telegramBot = telegramBot;
        }

        [Function("StudyHub")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
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
            catch (Exception)
            {
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
            throw new NotImplementedException();
        }

        private async Task BotOnMessageReceived(Message? message, CancellationToken cancellationToken)
        {
            await _telegramBot.SendTextMessageAsync(
                message.Chat.Id,
                "Hello world!",
                disableWebPagePreview: true,
                parseMode: ParseMode.Html);
        }
    }
}
