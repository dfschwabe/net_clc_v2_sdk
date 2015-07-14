using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CenturyLinkCloudSdk.Models;
using CenturyLinkCloudSdk.UAT.Mock;
using CenturyLinkCloudSdk.UAT.Mock.Controllers;
using NUnit.Framework;

namespace CenturyLinkCloudSdk.UAT
{
    [TestFixture]
    public class AlertPolicyServiceTests : FixtureBase
    {
        private AlertPolicyDefniition _policyDefinition;
        private AlertPolicy _policyResult;

        [Test]
        public void Create_PostsPolicyDefinition_ToCorrectAccount()
        {
            Given_I_Am(Users.A);

            When_I_Create_A_New_Policy();

            Then_I_Recieve_The_New_Policy();
            Then_The_New_Policy_Is_Associated_With_My_Account();
        }

        private void When_I_Create_A_New_Policy()
        {
            _policyDefinition = BuildPolicyDefinition();

            _policyResult = ServiceFactory.CreateAlertPolicyService().Create(_policyDefinition, CancellationToken.None).Result;
        }

        private void Then_I_Recieve_The_New_Policy()
        {
            Assert.AreEqual(_policyDefinition.Name, _policyResult.Name);
            CollectionAssert.AreEqual(_policyDefinition.Actions, _policyResult.Actions, new AlertActionComparer());
            CollectionAssert.AreEqual(_policyDefinition.Triggers, _policyResult.Triggers, new AlertTriggerComparer());
        }

        private void Then_The_New_Policy_Is_Associated_With_My_Account()
        {
            var policy = CurrentUser.AlertPolicies.FirstOrDefault(p => p.name.Equals(_policyDefinition.Name));

            Assert.NotNull(policy);
            Assert.AreEqual(policy.id, _policyResult.Id);
            Assert.AreEqual(_policyDefinition.Name, policy.name);
            AssertMockActionsEqual(_policyDefinition.Actions, policy.actions);
            AssertMockTriggersEqual(_policyDefinition.Triggers, policy.triggers);

        }

        private static void AssertMockActionsEqual(IEnumerable<AlertAction> expected, IEnumerable<MockAlertAction> actual)
        {
            expected.ToList().ForEach(e =>
            {
                var candidate = actual.SingleOrDefault(a => a.action.Equals(e.Action.ToString().ToLower()));

                Assert.NotNull(candidate);
                CollectionAssert.AreEqual(e.Settings.Recipients, candidate.settings.recipients);
            });
        }

        private void AssertMockTriggersEqual(IEnumerable<AlertTrigger> expected, IEnumerable<MockAlertTrigger> actual)
        {
            Assert.True(expected.All(e => actual.Any(a => a.duration.Equals(e.Duration)
                                                          && a.metric.Equals(e.Metric.ToString().ToLower())
                                                          && a.threshold.Equals(a.threshold))));
        }

        private static AlertPolicyDefniition BuildPolicyDefinition()
        {
            return new AlertPolicyDefniition
            {
                Name = "new policy",
                Actions = new List<AlertAction>
                {
                    new AlertAction
                    {
                        Action = AlertActionType.Email,
                        Settings = new AlertActionSettings
                        {
                            Recipients = new List<string>
                            {
                                "r1@domain.com",
                                "r2@domain.com",
                            }
                        }
                    }
                },
                Triggers = new List<AlertTrigger>
                {
                    new AlertTrigger
                    {
                        Duration = new TimeSpan(0,1,2,3),
                        Metric = AlertTriggerMetric.Memory,
                        Threshold = 90
                    },
                    new AlertTrigger
                    {
                        Duration = new TimeSpan(0,1,2,3),
                        Metric = AlertTriggerMetric.Disk,
                        Threshold = 50
                    }
                }
            };
        }
    }

    public class AlertActionComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var a1 = x as AlertAction;
            var a2 = y as AlertAction;

            if (!a1.Action.Equals(a2.Action))
                return 1;

            if (!a1.Settings.Recipients.SequenceEqual(a2.Settings.Recipients))
                return 1;

            return 0;
        }
    }

    public class AlertTriggerComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var a1 = x as AlertTrigger;
            var a2 = y as AlertTrigger;

            if (!a1.Duration.Equals(a2.Duration))
                return 1;

            if (!a1.Metric.Equals(a2.Metric))
                return 1;

            if (!a1.Threshold.Equals(a2.Threshold))
                return 1;

            return 0;
        }
    }
}