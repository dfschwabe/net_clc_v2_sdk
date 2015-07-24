using System;
using System.Collections.Generic;

namespace CenturyLinkCloudSdk.Models
{
    public class AlertPolicyDefniition
    {
        public string Name { get; set; }

        public IEnumerable<AlertAction> Actions { get; set; }

        public IEnumerable<AlertTrigger> Triggers { get; set; }
    }

    public class AlertPolicy : AlertPolicyDefniition
    {
        public string Id { get; set; }
    }

    public class AlertPolicyCollection
    {
        public List<AlertPolicy> Items { get; set; }
    }

    public class AlertAction
    {
        public AlertActionType Action { get; set; }

        public AlertActionSettings Settings { get; set; }
    }

    public class AlertActionSettings
    {
        public IEnumerable<string> Recipients { get; set; }
    }

    public class AlertTrigger
    {
        public AlertTriggerMetric Metric { get; set; }

        public TimeSpan Duration { get; set; }

        public int Threshold { get; set; }
    }

    public enum AlertActionType
    {
        Email
    }

    public enum AlertTriggerMetric
    {
        Cpu,
        Memory,
        Disk
    }
}