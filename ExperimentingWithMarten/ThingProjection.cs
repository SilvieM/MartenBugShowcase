using Marten.Events;
using Marten.Events.Aggregation;

namespace ExperimentingWithMarten;

public class ThingProjection : SingleStreamProjection<Thing>
{
    public Thing Create(IEvent<ThingCreatedEvent> evt)
    {
        return new Thing();
    }
}