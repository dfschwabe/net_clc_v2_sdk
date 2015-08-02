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
        private AntiAffinityPolicyDefinition _policyDefinition;
        private List<AntiAffinityPolicy> _policyCollectionResult;

        [Test]
        public void Create_PostsPolicyDefinition_ToCorrectAccount()
        {
            Given_I_Am(Users.A);

            When_I_Create_A_New_Policy();

            Then_I_Recieve_The_New_Policy();
            Then_The_New_Policy_Is_Associated_With_My_Account();
        }

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

        private void When_I_Create_A_New_Policy()
        {
            _policyDefinition = new AntiAffinityPolicyDefinition
            {
                Name = "new policy",
                Location = "CA3"
            };

            _policyResult = ServiceFactory.CreateAntiAffinityPolicyService().Create(_policyDefinition, CancellationToken.None).Result;
        }
        
        private void When_I_Get_My_Policies()
        {
            _policyCollectionResult = ServiceFactory.CreateAntiAffinityPolicyService().Get(CancellationToken.None).Result;
        }

        private void When_I_Get_My_Policy()
        {
            _policyResult = ServiceFactory.CreateAntiAffinityPolicyService().Get(CurrentUser.AntiAffinityPolicies.First().Key, CancellationToken.None).Result;
        }

        private void Then_I_Recieve_The_New_Policy()
        {
            Assert.AreEqual(_policyDefinition.Name, _policyResult.Name);
            Assert.AreEqual(_policyDefinition.Location, _policyResult.Location);
        }

        private void Then_The_New_Policy_Is_Associated_With_My_Account()
        {
            var mockPolicy = CurrentUser.AlertPolicies.Single().Value;

            Assert.AreEqual(mockPolicy.id, _policyResult.Id);
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