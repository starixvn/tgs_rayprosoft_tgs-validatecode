using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Services.ValidateCode;

namespace TGS.Tests.ValidateCode.UnitTests
{
    [TestFixture]
    public class GetValidateDataTests
    {
        [Test]
        public async Task Input_Validate_Code_Should_Exist()
        {
            var fakeValidateCodeStore = Substitute.For<IValidateCodeStore>();

            var sut = new GetValidateDataService(fakeValidateCodeStore);

            var validateCode = "123456";
            var token = "98FBCC27-C3F1-4479-AE0D-D68493853623";
            var query = new GetValidateDataQuery(validateCode, token);

            var expected = "jsonContent";

            fakeValidateCodeStore.GetDataAsync(Arg.Is(validateCode), Arg.Is(token)).Returns("jsonContent");

            var actual = await sut.GetAsync(query);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task Input_Validate_Code_Not_Exist()
        {
            var fakeValidateCodeStore = Substitute.For<IValidateCodeStore>();

            var sut = new GetValidateDataService(fakeValidateCodeStore);

            var validateCode = "123456";
            var token = "98FBCC27-C3F1-4479-AE0D-D68493853623";
            var query = new GetValidateDataQuery(validateCode, token);

            var expected = (string)null;

            fakeValidateCodeStore.GetDataAsync(Arg.Is(validateCode), Arg.Is(token)).Returns((string)null);

            var actual = await sut.GetAsync(query);

            Assert.AreEqual(expected, actual);
        }
    }
}
