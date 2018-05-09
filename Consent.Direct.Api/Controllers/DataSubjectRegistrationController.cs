using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FirebaseNet.Database;
using Microsoft.Extensions.Configuration;
using Consent.Direct.Api.Models;
using Consent.Direct.Api.Utils;
using Newtonsoft.Json;
using Consent.Direct.Api.Services;

namespace Consent.Direct.Api.Controllers
{
    [Route("api/[controller]")]
    public class DataSubjectRegistrationController : Controller
    {
        private IConfiguration _configuration;
        private FirebaseDB _firebaseDBRegistrations;

        public DataSubjectRegistrationController(IConfiguration Configuration)
        {
            _configuration = Configuration;
            FirebaseDB firebaseDB = new FirebaseDB(_configuration["Firebase:url"]);
            _firebaseDBRegistrations = firebaseDB.Node("data-subject-registrations");
        }

        /// <summary>
        /// Register a data subject. The idea is to receive an email address and account
        /// and send an email as a result. This email will contain a code which is stored 
        /// in Firebase alongside the user address and hash of the email address
        /// </summary>
        [HttpPost]
        public IActionResult Create([FromBody] DataSubject value)
        {
            var sr = new StandardResponse();

            try 
            {
                string token = Guid.NewGuid().ToString("N").ToLower();
                DataSubjectRegistration _reg = new DataSubjectRegistration()
                {
                    Token = token,
                    Subject = value
                };

                //var _serializer = JsonSerializer.Create();

                var json = $"{{ \"{token}\" : {JsonConvert.SerializeObject(value)} }}";

                FirebaseResponse resp = _firebaseDBRegistrations.Put(json);
                sr.Success = resp.Success;
                // sr.JSONContent = resp.JSONContent;
                
                return Ok(sr);
            }     
            catch (Exception ex)
            {
                return StatusCode(500, value: ex);
            }       
        }

        [HttpPut("{token}")]
        public async Task<IActionResult> Confirm(string token)
        {
            try 
            {
                // Confirm that the token exists and retrieve the emailhash and account
                var _firebaseDBRegistrationToken = _firebaseDBRegistrations.Node(token);
                FirebaseResponse resp = _firebaseDBRegistrationToken.Get();
                
                if (!resp.Success)
                {
                    var sr = new StandardResponse();
                    sr.Success = false;
                    sr.ErrorMessage = $"Could not retrieve token {token}";
                    return StatusCode(500, value: sr);
                }
            
                var subjectDetails = JsonConvert.DeserializeObject<DataSubject>(resp.JSONContent);
                
                // register the subject into the Consent Direct smart contract
                var web3 = new Nethereum.Web3.Web3(_configuration["Ethereum:Provider"]);
                var cds = new ConsentDirectService(web3, _configuration["SmartContracts:Consent.Direct:address"]);

                bool success = await cds.RegisterSubject(_configuration["Ethereum:Accounts:Consent.Direct"], subjectDetails.Account, subjectDetails.EmailHash);
                
                if (success)
                {
                    // Remove the registration token information from temp storage
                    resp = _firebaseDBRegistrationToken.Delete();
                }
                else 
                {
                    throw new Exception("Could not confirm registration because the DataSubject registration failed");
                }
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, value: ex);
            }            
        }
    }
}
