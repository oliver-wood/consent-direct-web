using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Web;
using Nethereum.Hex.HexTypes;
using Consent.Direct.Api.Models;
using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Consent.Direct.Api.Services
{
    public class ConsentDirectService
    {
        private readonly Nethereum.Web3.Web3 _web3;
        private string _abi = @"[ { ""constant"": false, ""inputs"": [ { ""name"": ""_address"", ""type"": ""address"" }, { ""name"": ""_name"", ""type"": ""string"" } ], ""name"": ""addDataProcessor"", ""outputs"": [], ""payable"": false, ""stateMutability"": ""nonpayable"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [ { ""name"": """", ""type"": ""uint256"" } ], ""name"": ""consentRequestToDataProcessor"", ""outputs"": [ { ""name"": """", ""type"": ""address"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": false, ""inputs"": [ { ""name"": ""_crhash"", ""type"": ""bytes32"" } ], ""name"": ""deactivateConsentRequest"", ""outputs"": [], ""payable"": false, ""stateMutability"": ""nonpayable"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [], ""name"": ""owner"", ""outputs"": [ { ""name"": """", ""type"": ""address"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": false, ""inputs"": [ { ""name"": ""_requestText"", ""type"": ""string"" }, { ""name"": ""_isActive"", ""type"": ""bool"" } ], ""name"": ""addConsentRequest"", ""outputs"": [], ""payable"": false, ""stateMutability"": ""nonpayable"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [ { ""name"": ""_dataProcessorAddress"", ""type"": ""address"" }, { ""name"": ""_start"", ""type"": ""uint256"" } ], ""name"": ""getNextRequest"", ""outputs"": [ { ""name"": """", ""type"": ""string"" }, { ""name"": """", ""type"": ""bool"" }, { ""name"": """", ""type"": ""uint256"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [ { ""name"": """", ""type"": ""bytes32"" } ], ""name"": ""consentRequestMap"", ""outputs"": [ { ""name"": ""requestText"", ""type"": ""string"" }, { ""name"": ""isActive"", ""type"": ""bool"" }, { ""name"": ""listpointer"", ""type"": ""uint256"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [ { ""name"": ""_questionPointerId"", ""type"": ""uint256"" } ], ""name"": ""getConsentRequestById"", ""outputs"": [ { ""name"": """", ""type"": ""string"" }, { ""name"": """", ""type"": ""bool"" }, { ""name"": """", ""type"": ""uint256"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [ { ""name"": """", ""type"": ""uint256"" } ], ""name"": ""consentRequests"", ""outputs"": [ { ""name"": """", ""type"": ""bytes32"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [], ""name"": ""getConsentRequestsLength"", ""outputs"": [ { ""name"": """", ""type"": ""uint256"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": false, ""inputs"": [ { ""name"": ""_crhash"", ""type"": ""bytes32"" } ], ""name"": ""activateConsentRequest"", ""outputs"": [], ""payable"": false, ""stateMutability"": ""nonpayable"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [ { ""name"": ""_dataProcessorAddress"", ""type"": ""address"" } ], ""name"": ""getConsentRequestIds"", ""outputs"": [ { ""name"": """", ""type"": ""uint256[]"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [ { ""name"": ""_address"", ""type"": ""address"" } ], ""name"": ""getDataProcessorByAddress"", ""outputs"": [ { ""name"": """", ""type"": ""string"" }, { ""name"": """", ""type"": ""uint256"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": false, ""inputs"": [ { ""name"": ""newOwner"", ""type"": ""address"" } ], ""name"": ""transferOwnership"", ""outputs"": [], ""payable"": false, ""stateMutability"": ""nonpayable"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [ { ""name"": ""_crhash"", ""type"": ""bytes32"" } ], ""name"": ""getConsentRequestByHash"", ""outputs"": [ { ""name"": """", ""type"": ""string"" }, { ""name"": """", ""type"": ""bool"" }, { ""name"": """", ""type"": ""uint256"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""anonymous"": false, ""inputs"": [ { ""indexed"": true, ""name"": ""account"", ""type"": ""address"" }, { ""indexed"": true, ""name"": ""consentRequestId"", ""type"": ""uint256"" } ], ""name"": ""ConsentGiven"", ""type"": ""event"" }, { ""anonymous"": false, ""inputs"": [ { ""indexed"": true, ""name"": ""account"", ""type"": ""address"" }, { ""indexed"": true, ""name"": ""consentRequestId"", ""type"": ""uint256"" } ], ""name"": ""ConsentRevoked"", ""type"": ""event"" }, { ""anonymous"": false, ""inputs"": [ { ""indexed"": true, ""name"": ""account"", ""type"": ""address"" }, { ""indexed"": false, ""name"": ""emailhash"", ""type"": ""bytes32"" } ], ""name"": ""SubjectAdded"", ""type"": ""event"" }, { ""anonymous"": false, ""inputs"": [ { ""indexed"": false, ""name"": ""message"", ""type"": ""string"" } ], ""name"": ""Debug"", ""type"": ""event"" }, { ""anonymous"": false, ""inputs"": [ { ""indexed"": false, ""name"": ""message"", ""type"": ""string"" }, { ""indexed"": false, ""name"": ""_address"", ""type"": ""address"" } ], ""name"": ""DebugAddress"", ""type"": ""event"" }, { ""anonymous"": false, ""inputs"": [ { ""indexed"": false, ""name"": ""message"", ""type"": ""string"" }, { ""indexed"": false, ""name"": ""_data"", ""type"": ""bytes32"" } ], ""name"": ""DebugBytes32"", ""type"": ""event"" }, { ""anonymous"": false, ""inputs"": [ { ""indexed"": true, ""name"": ""_dataProcessor"", ""type"": ""address"" }, { ""indexed"": false, ""name"": ""_id"", ""type"": ""uint256"" }, { ""indexed"": false, ""name"": ""_crhash"", ""type"": ""bytes32"" } ], ""name"": ""ConsentRequestAdded"", ""type"": ""event"" }, { ""anonymous"": false, ""inputs"": [ { ""indexed"": true, ""name"": ""_address"", ""type"": ""address"" }, { ""indexed"": false, ""name"": ""_position"", ""type"": ""uint256"" } ], ""name"": ""DataProcessorAdded"", ""type"": ""event"" }, { ""anonymous"": false, ""inputs"": [ { ""indexed"": true, ""name"": ""previousOwner"", ""type"": ""address"" }, { ""indexed"": true, ""name"": ""newOwner"", ""type"": ""address"" } ], ""name"": ""OwnershipTransferred"", ""type"": ""event"" }, { ""constant"": false, ""inputs"": [ { ""name"": ""_account"", ""type"": ""address"" }, { ""name"": ""_emailHash"", ""type"": ""bytes32"" } ], ""name"": ""registerSubject"", ""outputs"": [], ""payable"": false, ""stateMutability"": ""nonpayable"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [], ""name"": ""getSenderSubject"", ""outputs"": [ { ""name"": """", ""type"": ""uint256"" }, { ""name"": """", ""type"": ""uint256"" }, { ""name"": """", ""type"": ""bytes32"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": false, ""inputs"": [ { ""name"": ""_consentRequestId"", ""type"": ""uint256"" } ], ""name"": ""giveConsent"", ""outputs"": [], ""payable"": false, ""stateMutability"": ""nonpayable"", ""type"": ""function"" }, { ""constant"": false, ""inputs"": [ { ""name"": ""_consentRequestId"", ""type"": ""uint256"" } ], ""name"": ""revokeConsent"", ""outputs"": [], ""payable"": false, ""stateMutability"": ""nonpayable"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [], ""name"": ""getSubjectConsentResponseIds"", ""outputs"": [ { ""name"": """", ""type"": ""uint256[]"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [], ""name"": ""getSubjectResponsesLength"", ""outputs"": [ { ""name"": """", ""type"": ""uint256"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [ { ""name"": ""_start"", ""type"": ""uint256"" } ], ""name"": ""getNextSubjectResponse"", ""outputs"": [ { ""name"": """", ""type"": ""bool"" }, { ""name"": """", ""type"": ""uint256"" }, { ""name"": """", ""type"": ""bytes32"" }, { ""name"": """", ""type"": ""uint256"" }, { ""name"": """", ""type"": ""bool"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [ { ""name"": ""_requestId"", ""type"": ""uint256"" } ], ""name"": ""getRequestResponsesLength"", ""outputs"": [ { ""name"": """", ""type"": ""uint256"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [ { ""name"": ""_requestId"", ""type"": ""uint256"" } ], ""name"": ""getRequestConsentResponseIds"", ""outputs"": [ { ""name"": """", ""type"": ""uint256[]"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [ { ""name"": ""_requestId"", ""type"": ""uint256"" }, { ""name"": ""_start"", ""type"": ""uint256"" } ], ""name"": ""getNextRequestResponse"", ""outputs"": [ { ""name"": """", ""type"": ""bool"" }, { ""name"": """", ""type"": ""uint256"" }, { ""name"": """", ""type"": ""bytes32"" }, { ""name"": """", ""type"": ""address"" }, { ""name"": """", ""type"": ""bool"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": true, ""inputs"": [ { ""name"": ""_responseId"", ""type"": ""uint256"" } ], ""name"": ""getResponse"", ""outputs"": [ { ""name"": """", ""type"": ""uint256"" }, { ""name"": """", ""type"": ""address"" }, { ""name"": """", ""type"": ""bool"" } ], ""payable"": false, ""stateMutability"": ""view"", ""type"": ""function"" }, { ""constant"": false, ""inputs"": [ { ""name"": ""v"", ""type"": ""uint8"" }, { ""name"": ""r"", ""type"": ""bytes32"" }, { ""name"": ""s"", ""type"": ""bytes32"" }, { ""name"": ""data"", ""type"": ""bytes32"" }, { ""name"": ""_consentRequestId"", ""type"": ""uint256"" } ], ""name"": ""giveConsentWithSignedMessage"", ""outputs"": [], ""payable"": false, ""stateMutability"": ""nonpayable"", ""type"": ""function"" } ]";
        private Contract _contract;

        public ConsentDirectService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            _web3 = web3;
            _contract = web3.Eth.GetContract(_abi, contractAddress);
        }

        /// <summary>
        ///  calls getSenderSubject() public view returns (uint index, uint numresponses, bytes32 emailhash)
        ///  this function has this requirement for completion
        ///   require(cs.emailhash != bytes32(0));
        /// </summary>
        public async Task<bool> SubjectHasRegistered(string subjectAddress) 
        {
            try
            {
                var getSenderSubject = _contract.GetFunction("getSenderSubject");
                var subjectRegistration = await getSenderSubject.CallDeserializingToObjectAsync<SubjectRegistration>(subjectAddress, null, null, new object[] { });
                return subjectRegistration.EmailHash != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> RegisterSubject(string senderAddress, string subjectAddress, string emailhash)
        {
            var registerSubjectFunction = _contract.GetFunction("registerSubject");
            var subjectAddedEvent = _contract.GetEvent("SubjectAdded");
            var filterForAddress = await subjectAddedEvent.CreateFilterAsync(subjectAddress);

            var gas = await registerSubjectFunction.EstimateGasAsync(senderAddress, null, null, new object[] { subjectAddress, emailhash.HexToByteArray() });
            var receipt = await registerSubjectFunction.SendTransactionAndWaitForReceiptAsync(senderAddress, gas, null, null, new object[] { subjectAddress, emailhash.HexToByteArray() });

            var filteredlog = await subjectAddedEvent.GetFilterChanges<SubjectAddedEvent>(filterForAddress);
            
            if (filteredlog.Count() > 0)
            {
                Console.WriteLine($"Subject was added for: ${filteredlog[0].Event.Address}");
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    class SubjectAddedEvent
    {
        [Parameter("address", "account", 1, true)]
        public string Address { get; set; }

        [Parameter("bytes32", "emailhash", 2, false)]
        public string EmailHash { get; set; }
    }

    [FunctionOutput]
    class SubjectRegistration
    {
        [Parameter("uint256", 1)]
        public BigInteger Index { get; set; }

        [Parameter("uint256", 1)]
        public BigInteger NumResponses { get; set; }

        [Parameter("bytes32", 1)]
        public byte[] EmailHash { get; set; }
    }
}