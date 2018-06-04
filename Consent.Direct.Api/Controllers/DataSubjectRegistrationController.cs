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
using SendGrid;
using SendGrid.Helpers.Mail;
using Newtonsoft.Json.Linq;
using System.Net;

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
        public async Task<IActionResult> Create([FromBody] DataSubject value)
        {
            var sr = new StandardResponse();

            try 
            {
                // Create a token and generate a little JSON to contain it
                string token = Guid.NewGuid().ToString("N").ToLower();
                // I am certain there is a more elegant way to achieve this, but having the
                // token as the key field in Firebase is very useful.
                var json = $"{{ \"{token}\" : {JsonConvert.SerializeObject(value)} }}";

                // Put the token data in "temporary" storage
                FirebaseResponse resp = _firebaseDBRegistrations.Put(json);
                sr.Success = resp.Success;
                
                var linkurl = $"{Request.Scheme}://{Request.Host}/api/datasubjectregistration/confirm/{token}";

                // Send the email that requires action by the user
                var sentsuccess = await SendActionEmail(value.EmailAddress, linkurl);

                // Maybe replay the sentsuccess as part of the response?
                return Ok(sr);
            }     
            catch (Exception ex)
            {
                return StatusCode(500, value: ex);
            }       
        }

        /// <summary>
        /// This needs to be a Get as it will be called directly from a link in the email
        /// Returns a success code of true if the subject has not already registered
        /// </summary>
        [HttpGet("subjecteligible/{subjectAddress}")]
        public async Task<IActionResult> SubjectEligible(string subjectAddress)
        {
            var sr = new StandardResponse();
            try 
            {
                // register the subject into the Consent Direct smart contract
                var web3 = new Nethereum.Web3.Web3(_configuration["Ethereum:Provider"]);
                var cds = new ConsentDirectService(web3, _configuration["Ethereum:SmartContracts:Consent.Direct:address"]);

                bool hasregistered = await cds.SubjectHasRegistered(subjectAddress);
                if (hasregistered)
                {
                    sr.Success = false;
                    sr.ErrorMessage = "This account has already been registered with Consent.Direct.";
                    return Ok(sr);
                }
                else
                {
                    sr.Success = true;
                    return Ok(sr);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, value: ex);
            }
        }

        /// <summary>
        /// This needs to be a Get as it will be called directly from a link in the email
        /// </summary>
        [HttpGet("confirm/{token}")]
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
                var cds = new ConsentDirectService(web3, _configuration["Ethereum:SmartContracts:Consent.Direct:address"]);

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

        /// <summary>
        /// Sends the email with confirmation link via SendGrid
        /// </summary>
        private async Task<bool> SendActionEmail(string to, string linkurl)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_configuration["Email:Confirm:From:Address"], _configuration["Email:Confirm:From:Name"]),
                Subject = _configuration["Email:Confirm:Subject"],
                PlainTextContent = $"Your confirmation code is {linkurl}",
                HtmlContent = $"<p>Your confirmation code is <a href=\"{linkurl}\">{linkurl}</a></p>"
            };
            msg.AddTo(new EmailAddress(to));
            var response = await client.SendEmailAsync(msg);

            // Might want to throw if this is not Ok instead sending a bool?
            return (response.StatusCode.Equals(HttpStatusCode.OK) || response.StatusCode.Equals(HttpStatusCode.OK));
        }
    }
}
