using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BotByNinho.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;

namespace BotByNinho
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            var luisAttributes = new LuisModelAttribute(
                ConfigurationManager.AppSettings["LuisID"],
                ConfigurationManager.AppSettings["LuisSubscriptionKey"]);
            var luisService = new LuisService(luisAttributes);

            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    // If receive a start command from Telegram...
                    if (activity.Text == "/start")
                    {
                        var telegram = activity.CreateReply();
                        telegram.Text = "Olá, que bom ver você por aqui no Telegram!";

                        await connector.Conversations.ReplyToActivityAsync(telegram);
                        break;
                    }
                    // Send to Luis consideration...
                    await Conversation.SendAsync(activity, () => new RootDialog(luisService));
                    break;

                case ActivityTypes.ConversationUpdate:
                    if (activity.MembersAdded.Any(o => o.Id == activity.Recipient.Id))
                    {
                        var reply = activity.CreateReply();
                        reply.Text = "Olá, que bom ver você por aqui!";

                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    break;
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}