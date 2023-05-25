using Database;
using Discord;

namespace Forms;

public static class FormInteractions
{

	public static async Task Next(IDiscordInteraction interaction, IDiscordInteractionData data, string componentId, BotContext context)
	{
		if (!FormQuery.IsFormQuery(componentId))
			return;

		var (form, index) = FormManager.Parse(componentId);
		var next = form.HasQuery(index + 1);

		using (var db = new AutoConDatabase()) {
			var formData = db.FindForm(form.Id);
			var app = formData?.FindResumable(interaction.User.Id);

			if (app is not null)
			{
				var responses = form.GetQueryResponseData(data, index)
					.Select(x => new FormResponseModel { Title = x.Title, Value = x.Value })
					.AsEnumerable();

				app.CurrentQuery++;
				app.Responses?.AddRange(responses);
				
				if (!next)
				{
					var allResponses = app.Responses.Select(x => x.Revert()).ToList();
					var embed = form.GenerateResponseBuilder(interaction, allResponses);
					await interaction.RespondAsync(embed: embed.Build());

					app.InProgress = false;
				}

				await db.SaveChangesAsync();
			}
		}

		if (next)
		{
			await form.DisplayQuery(interaction, index + 1);
		}
	}

	public static void Register(BotContext context)
	{
		context.Client.ModalSubmitted += (modal) => Next(modal, modal.Data, modal.Data.CustomId, context);
		context.Client.SelectMenuExecuted += (menu) => Next(menu, menu.Data, menu.Data.CustomId, context);
	}
}