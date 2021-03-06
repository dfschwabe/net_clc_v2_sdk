﻿using System;
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
        private List<AlertPolicy> _policyCollectionResult;

        [SetUp]
        public void Setup()
        {
            _policyDefinition = null;
            _policyResult = null;
            _policyCollectionResult = null;
        }

        [Test]
        public void Create_PostsPolicyDefinition_ToCorrectAccount()
        {
            Given_I_Am(Users.A);

            When_I_Create_A_New_Policy();

            Then_I_Recieve_The_Policy();
            Then_The_Policy_Is_Associated_With_My_Account();
        }

        [Test]
        public void Delete_DeletesCorrectPolicy()
        {
            Given_I_Am(Users.A);
            Given_I_Have_An_Alert_Policy();

            When_I_Delete_The_Policy();

            Then_The_Policy_Is_Removed_From_My_Account();
        }

        [Test]
        public void Get_RetrievesSpecifiedPolicy_WhenValidIdSupplied()
        {
            Given_I_Am(Users.A);
            Given_I_Have_An_Alert_Policy();

            When_I_Get_My_Policy();

            Then_I_Receive_My_Policy();
        }

        [Test]
        public void Update_PostsPolicyUpdate_ToCorrectAccount()
        {
            Given_I_Am(Users.A);
            Given_I_Have_An_Alert_Policy();

            When_I_Update_My_Policy();

            Then_I_Recieve_The_Policy();
            Then_The_Policy_Is_Associated_With_My_Account();
        }

        [Test]
        public void Get_RetreivesAllPolicies()
        {
            Given_I_Am(Users.A);
            Given_I_Have_Multiple_Alert_Policies();

            When_I_Get_My_Policies();

            Then_I_Receive_All_Of_My_Policies();
        }

        private void Given_I_Have_An_Alert_Policy()
        {
            var policy = BuildMockPolicy();
            
            CurrentUser.AlertPolicies.Add(policy.id, policy);
        }

        private void Given_I_Have_Multiple_Alert_Policies()
        {
            var policy1 = BuildMockPolicy();
            var policy2 = BuildMockPolicy();

            CurrentUser.AlertPolicies.Add(policy1.id, policy1);
            CurrentUser.AlertPolicies.Add(policy2.id, policy2);
        }

        private void When_I_Create_A_New_Policy()
        {
            _policyDefinition = BuildPolicyDefinition();

            _policyResult = ServiceFactory.CreateAlertPolicyService().Create(_policyDefinition, CancellationToken.None).Result;
        }

        private void When_I_Update_My_Policy()
        {
            _policyDefinition = new AlertPolicyDefniition
            {
                Name = "updated name",
                Actions = new List<AlertAction>
                {
                    new AlertAction
                    {
                        Action = AlertActionType.Email,
                        Settings = new AlertActionSettings {Recipients = new List<string> {"update@domain.com"}}
                    }
                },
                Triggers = new List<AlertTrigger>
                {
                    new AlertTrigger
                    {
                        Duration = TimeSpan.FromSeconds(30),
                        Metric = AlertTriggerMetric.Cpu,
                        Threshold = 5
                    }
                }
            };

            var existingId = CurrentUser.AlertPolicies.First().Key;
            _policyResult = ServiceFactory.CreateAlertPolicyService().Update(existingId, _policyDefinition, CancellationToken.None).Result;
        }

        private void When_I_Delete_The_Policy()
        {
            ServiceFactory.CreateAlertPolicyService().Delete(CurrentUser.AlertPolicies.First().Key, CancellationToken.None).Wait();
        }

        private void When_I_Get_My_Policy()
        {
            _policyResult = ServiceFactory.CreateAlertPolicyService().Get(CurrentUser.AlertPolicies.First().Key, CancellationToken.None).Result;
        }

        private void When_I_Get_My_Policies()
        {
            _policyCollectionResult = ServiceFactory.CreateAlertPolicyService().Get(CancellationToken.None).Result;
        }

        private void Then_I_Recieve_The_Policy()
        {
            Assert.AreEqual(_policyDefinition.Name, _policyResult.Name);
            CollectionAssert.AreEqual(_policyDefinition.Actions, _policyResult.Actions, new AlertActionComparer());
            CollectionAssert.AreEqual(_policyDefinition.Triggers, _policyResult.Triggers, new AlertTriggerComparer());
        }

        private void Then_The_Policy_Is_Associated_With_My_Account()
        {
            var mockPolicy = CurrentUser.AlertPolicies.Single().Value;

            AssertPoliciesEqual(_policyResult, mockPolicy);
        }

        private void Then_The_Policy_Is_Removed_From_My_Account()
        {
            CollectionAssert.IsEmpty(CurrentUser.AlertPolicies);
        }

        private void Then_I_Receive_My_Policy()
        {
            AssertPoliciesEqual(_policyResult, CurrentUser.AlertPolicies[_policyResult.Id]);
        }

        private void Then_I_Receive_All_Of_My_Policies()
        {
            CurrentUser.AlertPolicies.Values.ToList().ForEach(mockPolicy =>
            {
                var receivedPolicy = _policyCollectionResult.Single(p => p.Id.Equals(mockPolicy.id));
                AssertPoliciesEqual(receivedPolicy, mockPolicy);
            });
        }

        private static void AssertPoliciesEqual(AlertPolicy policy, MockAlertPolicy mock)
        {
            Assert.AreEqual(policy.Id, mock.id);
            Assert.AreEqual(policy.Name, mock.name);
            AssertMockActionsEqual(policy.Actions, mock.actions);
            AssertMockTriggersEqual(policy.Triggers, mock.triggers);
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

        private static void AssertMockTriggersEqual(IEnumerable<AlertTrigger> expected, IEnumerable<MockAlertTrigger> actual)
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
                        Duration = new TimeSpan(0, 1, 2, 3),
                        Metric = AlertTriggerMetric.Memory,
                        Threshold = 90
                    },
                    new AlertTrigger
                    {
                        Duration = new TimeSpan(0, 1, 2, 3),
                        Metric = AlertTriggerMetric.Disk,
                        Threshold = 50
                    }
                }
            };
        }

        private static MockAlertPolicy BuildMockPolicy()
        {
            return new MockAlertPolicy
            {
                id = Guid.NewGuid().ToString(),
                name = "new policy",
                actions = new List<MockAlertAction>
                {
                    new MockAlertAction
                    {
                        action = "email",
                        settings = new MockAlertActionSettings
                        {
                            recipients = new List<string>
                            {
                                "r1@domain.com",
                                "r2@domain.com",
                            }
                        }
                    }
                },
                triggers = new List<MockAlertTrigger>
                {
                    new MockAlertTrigger
                    {
                        duration = new TimeSpan(0, 1, 2, 3),
                        metric = "memory",
                        threshold = 90
                    },
                    new MockAlertTrigger
                    {
                        duration = new TimeSpan(0, 1, 2, 3),
                        metric = "disk",
                        threshold = 50
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