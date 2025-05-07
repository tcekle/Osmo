using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Osmo.ConneX.Models;
using Osmo.ConneX.Providers;
using Osmo.Database.Extensions;
using System.Text.Json;

namespace Osmo.ConneX.GraphQl.Queries;

[ExtendObjectType(OsmoDatabaseExtensions.ROOT_QUERY_NAME)]
internal class RecordsQuery
{
    public async Task<IEnumerable<ConneXAuditEntry>> GetRecordById(
        [Service] IDbContextFactory<ConneXMetricsProviderContext> dbContextFactory,
        Guid id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        
        var mqttMessage = await context.MqttMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    
        if (mqttMessage is null)
        {
            return null;
        }
    
        return DeserializePayload(mqttMessage.PayloadAsString);
    }
    
    private static List<ConneXAuditEntry> DeserializePayload(string payload)
    {
        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        try
        {
            return JsonSerializer.Deserialize<List<ConneXAuditEntry>>(payload, jsonSerializerOptions);
        }
        catch (Exception)
        {
            // If the payload is not an array, try to deserialize it as a single object.
        }
    
        return [JsonSerializer.Deserialize<ConneXAuditEntry>(payload, jsonSerializerOptions)];
    }
}