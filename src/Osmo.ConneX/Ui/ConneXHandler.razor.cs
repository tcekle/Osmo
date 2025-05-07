using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace Osmo.ConneX.Ui;

using Consumers.Processors;
using GraphQl;
using Models.EventViewer;
using Providers;

public partial class ConneXHandler
{
    private readonly List<HandlerEvent> _events = new();
    
    private bool _loading = true;
    private bool _systemFound;
    private IGetSystem_System _system;
    private List<EventViewerEvent> _eventViewerEvents = new();

    [Inject]
    internal IConneXClient ConnexGraphql { get; set; }

    [Inject]
    internal IDbContextFactory<ConneXMetricsProviderContext> ConnexMetricsProviderContextFactory { get; set; }
    
    [Parameter]
    public int Id { get; set; }
    
    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree.
    /// Override this method if you will perform an asynchronous operation and
    /// want the component to refresh when that operation is completed.
    /// </summary>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
    protected override async Task OnInitializedAsync()
    {
        var handler = await ConnexGraphql.GetSystem.ExecuteAsync(Id);
        _system = handler.Data.System;

        _systemFound = _system != null;

        if (_systemFound)
        {
            await GetHandlerEvents();
        }
        
        _loading = false;
    }
    
        /// <summary>
    /// Gets the handler events from ConneX.
    /// </summary>
    private async Task GetHandlerEvents()
    {
        await using var context = await ConnexMetricsProviderContextFactory.CreateDbContextAsync();
        
        _eventViewerEvents = await context.HandlerEvents
            .Where(e => e.Timestamp > DateTime.UtcNow.AddMonths(-100)
                && (e.HandlerName == _system.HostName.ToLower() || e.HandlerIdentifier == _system.Entity.EntityIdentifier))
            .OrderByDescending(e => e.Timestamp)
            .Select(e => new EventViewerEvent
            {
                Type = e.Type,
                Title = e.Title,
                Message = e.Message,
                RawData = e.RawData,
                Timestamp = e.Timestamp
            }).ToListAsync();

        string[] notableEventTypes =
        [
            HandlerEventProcessor.SYSTEM_BEGIN_RUN,
            HandlerEventProcessor.SYSTEM_END_RUN,
            HandlerEventProcessor.USER_LOGIN,
        ];

        var notableEvents = _eventViewerEvents
            .Where(e => notableEventTypes.Any(e.Type.Contains))
            .Take(25);
        
        foreach (var evt in notableEvents)
        {
            if (evt.Type == HandlerEventProcessor.SYSTEM_BEGIN_RUN)
            {
                _events.Add(new BeginRunEvent
                {
                    Title = "Begin job",
                    Icon = Icons.Material.Filled.PlayCircleFilled,
                    Message = evt.Message.Replace("\n", "<br>"),
                    Timestamp = evt.Timestamp
                });
            }
            
            if (evt.Type == HandlerEventProcessor.SYSTEM_END_RUN)
            {
                _events.Add(new EndRunEvent
                {
                    Title = "End job",
                    Icon = Icons.Material.Filled.StopCircle,
                    Message = evt.Message.Replace("\n", "<br>"),
                    Timestamp = evt.Timestamp
                });
            }

            if (evt.Type == HandlerEventProcessor.USER_LOGIN)
            {
                _events.Add(new UserLogin
                {
                    Title = "User login",
                    Icon = Icons.Material.Filled.Person,
                    Message = evt.Message.Replace("\n", "<br>"),
                    Timestamp = evt.Timestamp
                });
            }
        }
    }
    
    /// <summary>
    /// Base class for all handler events.
    /// </summary>
    private record HandlerEvent
    {
        public string Icon { get; set; }
        public Color Color { get; set; } = Color.Primary;
        public DateTimeOffset Timestamp { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
    }
    
    /// <summary>
    /// End run event.
    /// </summary>
    private record EndRunEvent : HandlerEvent
    {
        // public string TerminationReason { get; init; }
    }
    
    /// <summary>
    /// Begin run event.
    /// </summary>
    private record BeginRunEvent : HandlerEvent
    {
        // public string TaskName { get; init; }
    }

    private record UserLogin : HandlerEvent;
}