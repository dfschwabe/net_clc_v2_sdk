using System;
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

        [SetUp]
        public void Setup()
        {
            Users.All.ForEach(u => u.AlertPolicies.Clear());
        }

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
            _policyDefinition = GetPolicyDefinition();

            _policyResult = ServiceFactory.CreateAlertPolicyService().Create(_policyDefinition, CancellationToken.None).Result;
        }

        private void Then_I_Recieve_The_New_Policy()
        {
            Assert.AreEqual(_policyDefinition.Name, _policyResult.Name);
            CollectionAssert.AreEqual(_policyDefinition.Actions, _policyResult.Actions);
            CollectionAssert.AreEqual(_policyDefinition.Triggers, _policyResult.Triggers);
        }

        private void Then_The_New_Policy_Is_Associated_With_My_Account()
        {
            var policy = CurrentUser.AlertPolicies.FirstOrDefault(p => p.name.Equals(_policyDefinition.Name));

            Assert.NotNull(policy);
            Assert.AreEqual(policy.id, _policyResult.Id);
            Assert.AreEqual(_policyDefinition.Name, policy.name);
            AssertActionsEqual(_policyDefinition.Actions, policy.actions);
            AssertTriggersEqual(_policyDefinition.Triggers, policy.triggers);

        }

        private static void AssertActionsEqual(IEnumerable<AlertAction> expected, IEnumerable<MockAlertAction> actual)
        {
            expected.ToList().ForEach(e =>
            {
                var candidate = actual.SingleOrDefault(a => a.action.Equals(e.Action.ToString().ToLower()));

                Assert.NotNull(candidate);
                CollectionAssert.AreEqual(e.Settings.Recipients, candidate.settings.recipients);
            });
        }

        private void AssertTriggersEqual(IEnumerable<AlertTrigger> expected, IEnumerable<MockAlertTrigger> actual)
        {
            Assert.True(expected.All(e => actual.Any(a => a.duration.Equals(e.Duration)
                                                          && a.metric.Equals(e.Metric.ToString().ToLower())
                                                          && a.threshold.Equals(a.threshold))));
        }

        private static AlertPolicyDefniition GetPolicyDefinition()
        {
            return new AlertPolicyDefniition
            {
                Name = "new policy",
                Actions = new List<AlertAction>
                {
                    new AlertAction
                    {
                        Action = AlertActionType.email,
                        Settings = new AlertActionSettings
                        {
                            Recipients = new List<string>()
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
}