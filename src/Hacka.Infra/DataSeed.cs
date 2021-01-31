using Hacka.Domain;
using Hacka.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Hacka.Infra
{
    public static class DataSeed
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var repository = serviceProvider.GetService<ISquadRepository>();
            await repository.AddAsync(new Squad
            {
                Id = 1,
                Name = "Squad 1",
                ChannelTeams =
                    "https://xpcorretora.webhook.office.com/webhookb2/9e74fc8a-77ec-4c42-9c8c-2590bcf0492f@cf56e405-d2b0-4266-b210-aa04636b6161/IncomingWebhook/67721fccac1743c49ca54452f68d33b0/d9be2d45-f7de-458f-9fa7-f346a435d072"
            });
            await repository.AddAsync(new Squad
            {
                Id = 2,
                Name = "Squad 2",
                ChannelTeams =
                    "https://xpcorretora.webhook.office.com/webhookb2/9e74fc8a-77ec-4c42-9c8c-2590bcf0492f@cf56e405-d2b0-4266-b210-aa04636b6161/IncomingWebhook/b47cde234d8a4d91af69c69db4af9d11/d9be2d45-f7de-458f-9fa7-f346a435d072"
            });
        }
    }
}
