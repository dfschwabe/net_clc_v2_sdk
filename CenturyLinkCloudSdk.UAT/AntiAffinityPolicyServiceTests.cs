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
    public class AntiAffinityPolicyServiceTests : FixtureBase
    {
        private AntiAffinityPolicy _policyResult;
        private List<AntiAffinityPolicy> _policyCollectionResult;

        [Test]
        public void Get_RetreivesAllPolicies()
        {
            Given_I_Am(Users.A);
            Given_I_Have_Multiple_AntiAffinity_Policies();

            When_I_Get_My_Policies();

            Then_I_Receive_All_Of_My_Policies();
        }

        [Test]
        public void Get_RetrievesSpecifiedPolicy_WhenValidIdSupplied()
        {
            Given_I_Am(Users.A);
            Given_I_Have_An_AntiAffinity_Policy();

            When_I_Get_My_Policy();

            Then_I_Receive_My_Policy();
        }

        private void Given_I_Have_Multiple_AntiAffinity_Policies()
        {
            AddPolicy("policy1");
            AddPolicy("policy2");
        }

        private void Given_I_Have_An_AntiAffinity_Policy()
        {
            AddPolicy("policy1");
        }

        private void When_I_Get_My_Policies()
        {
            _policyCollectionResult = ServiceFactory.CreateAntiAffinityPolicyService().Get(CancellationToken.None).Result;
        }

        private void When_I_Get_My_Policy()
        {
            _policyResult = ServiceFactory.CreateAntiAffinityPolicyService().Get(CurrentUser.AntiAffinityPolicies.First().Key, CancellationToken.None).Result;
        }

        private void Then_I_Receive_All_Of_My_Policies()
        {
            CurrentUser.AntiAffinityPolicies.Values.ToList().ForEach(mockPolicy =>
            {
                var receivedPolicy = _policyCollectionResult.Single(p => p.Id.Equals(mockPolicy.id));
                AssertPoliciesEqual(mockPolicy, receivedPolicy);
            });
        }

        private void Then_I_Receive_My_Policy()
        {
            AssertPoliciesEqual(CurrentUser.AntiAffinityPolicies[_policyResult.Id], _policyResult);
        }

        private void AddPolicy(string name)
        {
            var policy = new MockAntiAffinityPolicy
            {
                id = Guid.NewGuid().ToString(),
                name = name,
                location = "WA1"
            };

            CurrentUser.AntiAffinityPolicies.Add(policy.id, policy);
        }

        private void AssertPoliciesEqual(MockAntiAffinityPolicy expected, AntiAffinityPolicy actual)
        {
            Assert.AreEqual(expected.id, actual.Id);
            Assert.AreEqual(expected.name, actual.Name);
            Assert.AreEqual(expected.location, actual.Location);
        }
    }
}