using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using CenturyLinkCloudSdk.Models;
using CenturyLinkCloudSdk.UAT.Mock;
using CenturyLinkCloudSdk.UAT.Mock.Controllers;
using NUnit.Framework;

namespace CenturyLinkCloudSdk.UAT
{
    [TestFixture]
    public class CrosscuttingTests : FixtureBase
    {
        private AggregateException _exceptionResult;
        private ErrorResponse _errorResponse;

        [Test]
        public void Create_ThrowsDetailedException_OnBadRequest()
        {
            Given_I_Am(Users.A);

            When_My_Invocation_Results_In_An_Invalid_Request();

            Then_I_Recieve_A_Detailed_Exception();
        }

        private void When_My_Invocation_Results_In_An_Invalid_Request()
        {
            _errorResponse = new ErrorResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
                Reason = new ErrorReason
                {
                    message = "The request is invalid.",
                    modelState = new Dictionary<string, string[]>
                    {
                        {"model.field1", new[] {"The field1 field is required.", "The field1 field is must be between X and Y"}},
                        {"model.field2", new[] {"The field2 field is required."}},
                    }
                }
            };

            AuthenticationController.Error = _errorResponse;

            _exceptionResult = Assert.Throws<AggregateException>(
                () => ServiceFactory.CreateAlertPolicyService().Create(new AlertPolicyDefniition(), CancellationToken.None).Wait()
            );
        }

        private void Then_I_Recieve_A_Detailed_Exception()
        {
            Assert.NotNull(_exceptionResult);
            Assert.AreEqual(1, _exceptionResult.InnerExceptions.Count);

            var serviceException = _exceptionResult.InnerExceptions[0] as CloudServiceException;
            Assert.NotNull(serviceException);
            Assert.AreEqual(HttpStatusCode.BadRequest, serviceException.StatusCode);
            Assert.AreEqual("Bad Request", serviceException.ReasonPhrase);
            Assert.AreEqual(_errorResponse.Reason.message, serviceException.ErrorMessage);
            Assert.AreEqual(_errorResponse.Reason.modelState.ToList(), serviceException.ValidationErrors.ToList());
        }
    }
}