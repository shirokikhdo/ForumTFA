using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Forums.Domain.Monitoring;

public class DomainMetrics
{
    private const string APPLICATION_NAME = "Forums.Domain";

    private readonly Meter _meter;
    private readonly ConcurrentDictionary<string, Counter<int>> _counters;

    public static readonly ActivitySource ActivitySource = new ActivitySource(APPLICATION_NAME);

    public DomainMetrics(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create(APPLICATION_NAME);
        _counters = new ConcurrentDictionary<string, Counter<int>>();
    }

    public void IncrementCount(string name, int value, IDictionary<string, object?>? additionalTags = null)
    {
        var counter = _counters.GetOrAdd(name, _ => _meter.CreateCounter<int>(name));
        counter.Add(value, additionalTags?.ToArray() 
                           ?? ReadOnlySpan<KeyValuePair<string, object?>>.Empty);
    }

    public static IDictionary<string, object?> ResultTags(bool success) => new Dictionary<string, object?>
    {
        ["success"] = success
    };
}