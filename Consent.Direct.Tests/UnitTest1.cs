using System;
using Xunit;
using Consent.Direct.Api;
using Consent.Direct.Api.Services;

namespace Consent.Direct.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
            string consentDirectSmartContractAddress = "0x8a3239306c13ca56c1d73f883e77fcdd9682cb0f";
            string consentDirectAccount = "0x42c3b5107df5cb714f883ecaf4896f69d2b06a67";
            
            var web3 = new Nethereum.Web3.Web3(); // localhost 8545
            var cds = new ConsentDirectService(web3, consentDirectSmartContractAddress);

            bool success = await cds.RegisterSubject(consentDirectAccount, "0x47d6bb71fbdc161794ac6d4db623da7c8de24e31", "0x7c492afa7e42bf537557c3cdf470fc843ddc532d72f6fae27a755f74115fb557");
            Assert.True(success);
        }
    }
}
