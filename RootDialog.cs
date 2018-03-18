using System;
using System.Threading.Tasks;
using BotByNinho.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace BotByNinho.Dialogs
{
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        private enum dlgType { greeting, emotion, translate };

        public RootDialog(ILuisService service) : base(service) { }

        [LuisIntent("None")]
        public async Task NoneAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Desculpe, não entendi o que você me disse. Consegue me explicar melhor?");
            context.Done<string>(null);
        }

        [LuisIntent("")]
        public async Task UnkknownAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Ops, não reconheci o que você me disse... Consegue me explicar melhor?");
            context.Done<string>(null);
        }

        [LuisIntent("Greetings")]
        public async Task GreetingAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Oi! Hoje estou muito bem... espero que com você também esteja tudo ótimo!");
            context.Done<string>(null);
        }

        [LuisIntent("Consciousness")]
        public async Task ConsciousAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Bom, eu sou um robô aprendendo a conversar com humanos. O Nilton é meu mestre!");
            context.Done<string>(null);
        }

        [LuisIntent("Personal")]
        public async Task PersonalAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Cara, sou carinhosamente conhecido como NinhoBot :D");
            context.Done<string>(null);
        }

        [LuisIntent("Gratitude")]
        public async Task GratitudeAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("de nada...");
            context.Done<string>(null);
        }

        [LuisIntent("Translate")]
        public async Task TranslateAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Vamos traduzir algumas coisas então! Aliás, no momento eu só sei inglês ;D" + 
                                    "\n É só me dizer o que você deseja traduzir...");
            context.Wait(TranslatePtBr);
        }

        [LuisIntent("Emotion")]
        public async Task EmotionAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Então você quer falar sobre emoções... No momento estou aprendendo sobre isso.\n" + 
                                    "Não é tão simples assim entender as emoções humanas...");
            context.Done<string>(null);
        }

        #region Functions

        private async Task TranslatePtBr(IDialogContext context, IAwaitable<IMessageActivity> value)
        {
            var msg = await value;
            var txt = msg.Text;

            var response = await new Translation().TranslateAsync(txt);

            await context.PostAsync(response);
            context.Wait(MessageReceived);
        }

        #endregion
    }
}