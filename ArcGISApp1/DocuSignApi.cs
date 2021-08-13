using System;
using System.Collections.Generic;
using System.Text;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.eSign.Client;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using ArcGISApp1.Models;

namespace ArcGISApp1
{
    public class DocuSignApi
    {
        ApiClient _apiClient;
        ApiCallsInfo _apiCallsInfo;
        string _appBaseDir = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Get or Refresh the Token if is expired
        /// </summary>
        public void GetOrRefreshApiCallsInfo()
        {
            if(_apiCallsInfo is null)
            {
                _apiCallsInfo = JWTAuth.AuthenticateWithJWT();
                _apiClient = new ApiClient(_apiCallsInfo.Baseuri);
                _apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + _apiCallsInfo.AuthToken.access_token);
            }
            else
            {
                DateTime difDateTime = _apiCallsInfo.CreatedTokenTime.AddSeconds((double)_apiCallsInfo.AuthToken.expires_in - 300);
                if(DateTime.Now > difDateTime)
                {
                    _apiCallsInfo = JWTAuth.AuthenticateWithJWT();
                    _apiClient = new ApiClient(_apiCallsInfo.Baseuri);
                    _apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + _apiCallsInfo.AuthToken.access_token);
                }
            }
        }

        /// <summary>
        /// Send an envelope of a Map to the stakeholders
        /// </summary>
        /// <param name="stakeholdersList"></param>
        /// <returns>SignProcess</returns>
        public SignProcess SendEnvelope(List<Stakeholder> stakeholdersList)
        {
            GetOrRefreshApiCallsInfo();

            // Read a file from disk to use as a document.
            byte[] fileBytes = File.ReadAllBytes($"{_appBaseDir}\\Resources\\Map.pdf");

            EnvelopeDefinition envDef = new EnvelopeDefinition();
            envDef.EmailSubject = "[DocuSign-JGI] - Please sign this doc";

            // Add a document to the envelope
            Document doc = new Document();
            doc.DocumentBase64 = System.Convert.ToBase64String(fileBytes);
            doc.Name = "Map.pdf";
            doc.DocumentId = "1";

            envDef.Documents = new List<Document>();
            envDef.Documents.Add(doc);

            envDef.Recipients = new Recipients();
            envDef.Recipients.Signers = new List<Signer>();

            int xPostion = 40;
            int yPostion = 475;
            int recipientId = 0;

            foreach(Stakeholder stakeholder in stakeholdersList)
            {
                recipientId++;
                if(recipientId %2 == 0)
                {
                    xPostion = 300;
                }
                else
                {
                    xPostion = 40;
                    yPostion += 75;
                }               
                               

                // Add a recipient to sign the documeent
                Signer signer = new Signer();
                signer.Email = stakeholder.Email;
                signer.Name = stakeholder.Name;
                signer.RecipientId = recipientId.ToString();

                // Create a |SignHere| tab somewhere on the document for the recipient to sign
                signer.Tabs = new Tabs();
                signer.Tabs.SignHereTabs = new List<SignHere>();
                SignHere signHere = new SignHere();
                signHere.DocumentId = "1";
                signHere.PageNumber = "1";
                signHere.RecipientId = recipientId.ToString();
                
                signHere.XPosition = xPostion.ToString();
                signHere.YPosition = yPostion.ToString();
                signer.Tabs.SignHereTabs.Add(signHere);


                envDef.Recipients.Signers.Add(signer);
            }            

            // set envelope status to "sent" to immediately send the signature request
            envDef.Status = "sent";

            // |EnvelopesApi| contains methods related to creating and sending Envelopes (aka signature requests)
            EnvelopesApi envelopesApi = new EnvelopesApi(_apiClient);
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(_apiCallsInfo.AccountId, envDef);

            // print the JSON response
            Trace.WriteLine("EnvelopeSummary:\n" + JsonConvert.SerializeObject(envelopeSummary));
            //Trace.WriteLine("Envelope has been sent to " + string.Join(',', stakeholdersList.ForEach(x => x.Email);

            SignProcess signProcess = new SignProcess();
            if (!string.IsNullOrEmpty(envelopeSummary?.EnvelopeId))
            {
                signProcess.EnvelopeId = envelopeSummary.EnvelopeId;
                signProcess.State = envelopeSummary.Status;
                signProcess.SendDateTime = envelopeSummary.StatusDateTime;
                signProcess.StakeholderList = stakeholdersList;
            }

            return signProcess;
        }

        /// <summary>
        /// Get the status of an envelope
        /// </summary>
        /// <param name="envelopeId"></param>
        /// <returns>string with the status</returns>
        public string GetEnvelopeStatus(string envelopeId)
        {
            GetOrRefreshApiCallsInfo();

            EnvelopesApi envelopesApi = new EnvelopesApi(_apiClient);
            Envelope envInfo = envelopesApi.GetEnvelope(_apiCallsInfo.AccountId, envelopeId);

            Trace.WriteLine("EnvelopeSummary:\n" + JsonConvert.SerializeObject(envInfo));
            return envInfo.Status;
        }

        /// <summary>
        /// Download the envelope documents with certificate included
        /// </summary>
        /// <param name="envelopeId"></param>
        /// <returns>if it is downloaded</returns>
        public bool DownloadEnvelopeDocuments(string envelopeId)
        {
            GetOrRefreshApiCallsInfo();

            EnvelopesApi envelopesApi = new EnvelopesApi(_apiClient);
            EnvelopesApi.GetDocumentOptions getDocumentOptions = new EnvelopesApi.GetDocumentOptions();
            getDocumentOptions.certificate = "true";
            Stream resultStream = envelopesApi.GetDocument(_apiCallsInfo.AccountId, envelopeId, "combined", getDocumentOptions);

            Utils.StreamToFile(streamOrig: resultStream, pathFileDest: $"{_appBaseDir}\\Resources\\Map_signed.pdf");

            return File.Exists($"{_appBaseDir}\\Resources\\Map_signed.pdf");
        }
    }
}
