using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Osmo.Common.Messages;
using Osmo.ConneX.Models;
using Osmo.ConneX.Providers;
using Osmo.ConneX.Ui;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Osmo.ConneX.Consumers.Processors;

internal class HandlerEventProcessor
{
    private readonly IDbContextFactory<ConneXMetricsProviderContext> _connexMetricsProviderContextFactory;
    
    internal const string SYSTEM_BEGIN_RUN = "BeginRun";
    internal const string SYSTEM_END_RUN = "EndRun";
    internal const string SYSTEM_LIGHT_TOWER_CHANGED = "LightTowerChanged";
    internal const string SYSTEM_OPERATION_DEVICE_COMPLETE = "DeviceComplete";
    internal const string SYSTEM_OPERATION_PICK = "DevicePicked";
    internal const string SYSTEM_OPERATION_PLACE = "DevicePlaced";
    internal const string SYSTEM_SHUTDOWN = "SystemShutdown";
    internal const string SYSTEM_STARTUP = "SystemStartup";
    internal const string SYSTEM_STATISTICS_EVENT = "SystemStatistic";
    internal const string SYSTEM_STATUS_EVENT = "SystemStatus";
    internal const string USER_LOGIN = "UserLogin";
    
    private static readonly Func<ConneXMetricsProviderContext, Guid, DateTime, Task<bool>> DoesGuidExist =
        EF.CompileAsyncQuery((ConneXMetricsProviderContext context, Guid id, DateTime minDate) =>
            context.HandlerEvents
                .AsNoTracking()
                .Any(e => e.RelatedMessageId == id && e.Timestamp >= minDate));
    
    private static readonly Func<ConneXMetricsProviderContext, Guid, DateTime, Task<bool>> DoesHandlerStatisticExist =
        EF.CompileAsyncQuery((ConneXMetricsProviderContext context, Guid id, DateTime minDate) =>
            context.HandlerStatistics
                .AsNoTracking()
                .Any(e => e.RelatedMessageId == id && e.TimeStamp >= minDate));

    public HandlerEventProcessor(IDbContextFactory<ConneXMetricsProviderContext> connexMetricsProviderContextFactory)
    {
        _connexMetricsProviderContextFactory = connexMetricsProviderContextFactory;
    }

    public async Task ProcessEvent(MqttMessage message)
    {
        await using var context = await _connexMetricsProviderContextFactory.CreateDbContextAsync();

        if (await DoesGuidExist(context, message.Id, message.Timestamp.ToUniversalTime().AddDays(-1)))
        {
            return;
        }

        string[] levels = message.Topic.Split("/");
        
        Task processor = levels[1] switch
        {
            "beginrun" => ParseBeginRun(message),
            "endrun" => ParseAhEndRun(message),
            "lightowerchanged" => ParseAhLightTowerStatusChanged(message),
            "operations" => ParseAhOperationMessage(message),
            "shutdown" => ParseAhShutdown(message),
            "startup" => ParseAhStartup(message),
            "systemstatistics" => ParseAhSystemStatisticsMessage(message),
            "systemstatus" => ParseAhStatus(message),
            "users" => ParseAhUsersMessage(message),
            _ => Task.CompletedTask
        };

        await processor;
    }
    
    private Task ParseAhOperationMessage(MqttMessage message)
    {
        string[] levels = message.Topic.Split("/");
        
        return levels[2] switch
        {
            "devicecomplete" => ParseAhOperationDeviceComplete(message),
            "pick" => ParseAhPick(message),
            "place" => ParseAhPlace(message),
            _ => Task.CompletedTask
        };
    }
    
    private async Task ParseAhUsersMessage(MqttMessage message)
    {
        string[] levels = message.Topic.Split("/");
        
        Task processor = levels[2] switch
        {
            "login" => ParseAhUserLogin(message),
            _ => Task.CompletedTask
        };

        await processor;
    }

    private async Task ParseBeginRun(MqttMessage message)
    {
        var jsonObject = JObject.Parse(message.PayloadAsString);

        string[] levels = message.Topic.Split("/");

        string systemName = levels[2];
        string sessionId = levels[3];

        await AddHandlerEventToDatabase(new HandlerEvent
        {
            Type = SYSTEM_BEGIN_RUN,
            Title = $"Begin run ({systemName})",
            Message = $@"Task Name: {jsonObject["TaskName"]}
Version: {jsonObject["xH700Version"]}",
            RawData = message.PayloadAsString,
            Timestamp = message.Timestamp.ToUniversalTime(),
            RelatedMessageId = message.Id,
            HandlerName = systemName,
            SessionId = sessionId
        });
    }
    
    private async Task ParseAhEndRun(MqttMessage message)
    {
        var jsonObject = JObject.Parse(message.PayloadAsString);

        string[] levels = message.Topic.Split("/");

        string systemName = levels[2];
        string sessionId = levels[3];
        string handlerIdentifier = jsonObject["HandlerIdentifier"].Value<string>();
        
        string formattedMessage = $"""
                                   Termination Reason: {jsonObject["TerminationReason"]}
                                   Pass Quantity: {jsonObject["PassQuantity"]}
                                   Fail Quantity: {jsonObject["FailQuantity"]}
                                   """;

        await AddHandlerEventToDatabase(new HandlerEvent
        {
            Type = SYSTEM_END_RUN,
            Title = $"End run ({systemName})",
            Message = formattedMessage,
            RawData = message.PayloadAsString,
            Timestamp = message.Timestamp.ToUniversalTime(),
            RelatedMessageId = message.Id,
            HandlerName = systemName,
            SessionId = sessionId,
            HandlerIdentifier = handlerIdentifier
        });
    }
    
    private async Task ParseAhUserLogin(MqttMessage message)
    {
        var jsonObject = JObject.Parse(message.PayloadAsString);

        string[] levels = message.Topic.Split("/");

        string systemName = levels[3];
        string sessionId = levels[4];
        
        string formattedMessage = $"""
                                   User login attempt ({systemName})
                                   Username: {jsonObject["Username"]}
                                   Success: {jsonObject["Success"]}
                                   """;

        await AddHandlerEventToDatabase(new HandlerEvent
        {
            Type = USER_LOGIN,
            Title = $"User login attempt ({systemName})",
            Message = formattedMessage,
            RawData = message.PayloadAsString,
            Timestamp = message.Timestamp.ToUniversalTime(),
            RelatedMessageId = message.Id,
            HandlerName = systemName,
            SessionId = sessionId
        });
    }
    
    private async Task ParseAhSystemStatisticsMessage(MqttMessage message)
    {
        try
        {
            await using var context = await _connexMetricsProviderContextFactory.CreateDbContextAsync();
            
            // if (await context.HandlerStatistics.AnyAsync(e => e.RelatedMessageId == message.Id))
            // {
            //     return;
            // }
            if (await DoesHandlerStatisticExist(context, message.Id, message.Timestamp.AddDays(-1)))
            {
                return;
            }
            
            string[] levels = message.Topic.Split("/");

            var handlerStatistics = JsonSerializer.Deserialize<HandlerStatisticsMessage>(message.PayloadAsString);
            
            string systemName = levels[2];
            string sessionId = levels[3];
            
            await context.HandlerStatistics.AddAsync(new HandlerStatistics
            {
                TimeStamp = message.Timestamp.ToUniversalTime(),
                RelatedMessageId = message.Id,
                RelatedHandlerId = handlerStatistics.HandlerIdentifier,
                SessionId = sessionId,
                TotalPass = handlerStatistics.TotalPass,
                TotalFail = handlerStatistics.TotalFail,
                Uph = handlerStatistics.UPH,
                SystemYield = double.TryParse(handlerStatistics.SystemYield, out var systemYield) ? systemYield : 0,
                HandlerYield = double.TryParse(handlerStatistics.HandlerYield, out var handlerYield) ? handlerYield : 0,
                ProgrammerYield = double.TryParse(handlerStatistics.ProgrammerYield, out var programmerYield) ? programmerYield : 0,
                DevicesFailedOnProgrammer = handlerStatistics.DevicesFailedOnProgrammer,
                DevicesPickedInput = handlerStatistics.DevicesPickedInput,
                DevicesFailedOnLaser = handlerStatistics.DevicesFailedOnLaser,
                DevicesFailedOn3DSystem = handlerStatistics.DevicesFailedOn3DSystem,
                DevicesFailedVision = handlerStatistics.DevicesFailedVision,
                DevicesFailedREST = handlerStatistics.DevicesFailedREST,
                JobProcessingTime = int.TryParse(handlerStatistics.JobProcessingTime, out var jobProcessingTime) ? jobProcessingTime : 0,
                JobAssistanceTime = int.TryParse(handlerStatistics.JobAssistanceTime, out var jobAssistanceTime) ? jobAssistanceTime : 0,
                JobCompletionEstimate = DateTime.TryParse(handlerStatistics.JobCompletionEstimate, out var jobCompletionEstimate) ? jobCompletionEstimate.ToUniversalTime() : DateTime.MinValue
            });
            
            await AddHandlerEventToDatabase(new HandlerEvent
            {
                Type = SYSTEM_STATISTICS_EVENT,
                Title = $"System statistics updated ({systemName})",
                RawData = message.PayloadAsString,
                Timestamp = message.Timestamp.ToUniversalTime(),
                RelatedMessageId = message.Id,
                HandlerName = systemName,
                SessionId = sessionId
            });
            
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private async Task ParseAhLightTowerStatusChanged(MqttMessage message)
    {
        var jsonObject = JObject.Parse(message.PayloadAsString);

        string[] levels = message.Topic.Split("/");

        string systemName = levels[2];
        string sessionId = levels[3];
        
        await AddHandlerEventToDatabase(new HandlerEvent
        {
            Type = SYSTEM_LIGHT_TOWER_CHANGED,
            Title = $"Light tower status changed ({systemName})",
            Message = $"{jsonObject["OldState"]} -> {jsonObject["NewState"]}",
            RawData = message.PayloadAsString,
            Timestamp = message.Timestamp.ToUniversalTime(),
            RelatedMessageId = message.Id,
            HandlerName = systemName,
            SessionId = sessionId
        });
    }
    
    private async Task ParseAhOperationDeviceComplete(MqttMessage message)
    {
        var jsonObject = JObject.Parse(message.PayloadAsString);

        string[] levels = message.Topic.Split("/");

        string systemName = levels[3];
        string sessionId = levels[4];
            
        string messageText = $"""     
                              Device ID: {jsonObject["DeviceId"]}
                              Status: {jsonObject["Status"]}
                              """;

        await AddHandlerEventToDatabase(new HandlerEvent
        {
            Type = SYSTEM_OPERATION_DEVICE_COMPLETE,
            Title = $"Device complete ({systemName})",
            Message = messageText,
            RawData = message.PayloadAsString,
            Timestamp = message.Timestamp.ToUniversalTime(),
            RelatedMessageId = message.Id,
            HandlerName = systemName,
            SessionId = sessionId
        });
    }
    
    private async Task ParseAhPick(MqttMessage message)
    {
        var jsonObject = JObject.Parse(message.PayloadAsString);

        string[] levels = message.Topic.Split("/");

        string systemName = levels[3];
        string sessionId = levels[4];
        
        string messageText = $"""     
                               Status: {jsonObject["Status"]}
                               Device ID: {jsonObject["DeviceID"]}
                               Location: {jsonObject["Location"]}
                               Position: {jsonObject["Position"]}
                               Pick Head: {jsonObject["PickHead"]}
                               """;

        await AddHandlerEventToDatabase(new HandlerEvent
        {
            Type = SYSTEM_OPERATION_PICK,
            Title = $"Device picked ({systemName})",
            Message = messageText,
            RawData = message.PayloadAsString,
            Timestamp = message.Timestamp.ToUniversalTime(),
            RelatedMessageId = message.Id,
            HandlerName = systemName,
            SessionId = sessionId
        });
    }
    
    private async Task ParseAhPlace(MqttMessage message)
    {
        var jsonObject = JObject.Parse(message.PayloadAsString);

        string[] levels = message.Topic.Split("/");

        string systemName = levels[3];
        string sessionId = levels[4];
        
        string messageText = $"""     
                              Status: {jsonObject["Status"]}
                              Device ID: {jsonObject["DeviceID"]}
                              Location: {jsonObject["Location"]}
                              Position: {jsonObject["Position"]}
                              Pick Head: {jsonObject["PickHead"]}
                              """;

        await AddHandlerEventToDatabase(new HandlerEvent
        {
            Type = SYSTEM_OPERATION_PLACE,
            Title = $"Device placed ({systemName})",
            Message = messageText,
            RawData = message.PayloadAsString,
            Timestamp = message.Timestamp.ToUniversalTime(),
            RelatedMessageId = message.Id,
            HandlerName = systemName,
            SessionId = sessionId
        });
    }

    private async Task ParseAhShutdown(MqttMessage message)
    {
        string[] levels = message.Topic.Split("/");

        string systemName = levels[2];
        string sessionId = levels[3];

        await AddHandlerEventToDatabase(new HandlerEvent
        {
            Type = SYSTEM_SHUTDOWN,
            Title = $"AH shutdown ({systemName})",
            RawData = message.PayloadAsString,
            Timestamp = message.Timestamp.ToUniversalTime(),
            RelatedMessageId = message.Id,
            HandlerName = systemName,
            SessionId = sessionId
        });
    }

    private async Task ParseAhStartup(MqttMessage message)
    {
        var jsonObject = JObject.Parse(message.PayloadAsString);

        string[] levels = message.Topic.Split("/");

        string systemName = levels[2];
        string sessionId = levels[3];
            
        await AddHandlerEventToDatabase(new HandlerEvent
        {
            Type = SYSTEM_STARTUP,
            Title = $"AH started ({systemName})",
            Message = $"Machine type: {jsonObject["MachineType"]}",
            RawData = message.PayloadAsString,
            Timestamp = message.Timestamp.ToUniversalTime(),
            RelatedMessageId = message.Id,
            HandlerName = systemName,
            SessionId = sessionId
        });
    }
    
    private async Task ParseAhStatus(MqttMessage message)
    {
        var jsonObject = JObject.Parse(message.PayloadAsString);

        string[] levels = message.Topic.Split("/");

        string systemName = levels[2];
        string sessionId = levels[3];
            
        string runState = jsonObject["RunState"].Value<int>() switch
        {
            0 => "Job Idle",
            1 => "Job Paused",
            2 => "Job Running",
            3 => "Job Stopped",
            _ => "Unknown"
        };

        string errorLevel = jsonObject["ErrorMessage"]["ErrorLevel"].Value<int>() switch
        {
            0 => "Warning",
            1 => "Error",
            2 => "Fatal",
            _ => "Unknown"
        };
        
        string messageText = $"""
                              Run state: {runState}
                              Error Level: {errorLevel}
                              Message: {jsonObject["ErrorMessage"]["Message"]}
                              Error Code: {jsonObject["ErrorMessage"]["ErrorCode"]}
                              """;
        
        await AddHandlerEventToDatabase(new HandlerEvent
        {
            Type = SYSTEM_STATUS_EVENT,
            Title = $"AH message displayed ({systemName})",
            Message = messageText,
            RawData = message.PayloadAsString,
            Timestamp = message.Timestamp.ToUniversalTime(),
            RelatedMessageId = message.Id,
            HandlerName = systemName,
            SessionId = sessionId
        });
    }
    
    private async Task AddHandlerEventToDatabase(HandlerEvent handlerEvent)
    {
        await using var context = await _connexMetricsProviderContextFactory.CreateDbContextAsync();
        await context.HandlerEvents.AddAsync(handlerEvent);
        await context.SaveChangesAsync();
    }
    
    private class HandlerStatisticsMessage
    {
        public string HandlerIdentifier { get; set; }
        public int TotalPass { get; set; }
        public int TotalFail { get; set; }
        public int UPH { get; set; }
        public string SystemYield { get; set; }
        public string HandlerYield { get; set; }
        public string ProgrammerYield { get; set; }
        public int DevicesFailedOnProgrammer { get; set; }
        public int DevicesPickedInput { get; set; }
        public int DevicesFailedOnLaser { get; set; }
        public int DevicesFailedOn3DSystem { get; set; }
        public int DevicesFailedVision { get; set; }
        public int DevicesFailedREST { get; set; }
        public string JobProcessingTime { get; set; }
        public string JobAssistanceTime { get; set; }
        public string JobCompletionEstimate { get; set; }
    }


    
    // private static string ConvertPayload(string hexPayload)
    // {
    //     return Encoding.UTF8.GetString(
    //         FromHexString(
    //             hexPayload.Replace("\\", string.Empty)
    //                 .Replace("\\x", string.Empty)));
    // }
    //
    // /// <summary>
    // /// Converts the specified string, which encodes binary data as hex characters, to an equivalent 8-bit unsigned integer array.
    // /// </summary>
    // /// <param name="s">The string to convert.</param>
    // /// <returns>An array of 8-bit unsigned integers that is equivalent to <paramref name="s"/>.</returns>
    // /// <exception cref="ArgumentNullException"><paramref name="s"/> is <code>null</code>.</exception>
    // /// <exception cref="FormatException">The length of <paramref name="s"/>, is not zero or a multiple of 2.</exception>
    // /// <exception cref="FormatException">The format of <paramref name="s"/> is invalid. <paramref name="s"/> contains a non-hex character.</exception>
    // private static byte[] FromHexString(string s)
    // {
    //     if (s == null)
    //     {
    //         throw new ArgumentNullException("s");
    //         // ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
    //     }
    //
    //     return FromHexString(s.AsSpan());
    // }
    
    /*
     *         private ReportEvent ParseAhMessage(string[] levels, MessageModelImport model)
        {
            return levels[0] switch
            {
                "beginrun" => ParseAhBeginRun(model),
                "endrun" => ParseAhEndRun(model),
                "lightowerchanged" => ParseAhLightTowerStatusChanged(model),
                "operations" => ParseAhOperationMessage(levels.Skip(1).ToArray(), model),
                "shutdown" => ParseAhShutdown(model),
                "startup" => ParseAhStartup(model),
                "systemstatistics" => ParseAhSystemStatisticsMessage(model),
                "systemstatus" => ParseAhStatus(model),
                "users" => ParseAhUsersMessage(levels.Skip(1).ToArray(), model),
                _ => null
            };
        }
     */
}