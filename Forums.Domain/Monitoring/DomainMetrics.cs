using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace Forums.Domain.Monitoring;

internal class DomainMetrics
{
    private readonly Meter _meter;
    private readonly ConcurrentDictionary<string, Counter<int>> _counters;

    public DomainMetrics()
    {
        _meter = new Meter("Forums.Domain");
        _counters = new ConcurrentDictionary<string, Counter<int>>();
    }

    public void ForumsFetched(bool success) =>
        IncrementCount("forums.fetched", 1, new Dictionary<string, object?>
        {
            ["success"] = success
        });

    private void IncrementCount(string name, int value, IDictionary<string, object?>? additionalTags = null)
    {
        var counter = _counters.GetOrAdd(name, _ => _meter.CreateCounter<int>(name));
        counter.Add(value, additionalTags?.ToArray() 
                           ?? ReadOnlySpan<KeyValuePair<string, object?>>.Empty);
    }
}