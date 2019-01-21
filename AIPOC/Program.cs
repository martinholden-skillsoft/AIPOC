using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Common.Logging;
using AIPOC.Models;
using Olsa;
using Olsa.WCF.Extensions;
using Newtonsoft.Json;
using System.ServiceModel.Channels;
using CommandLine;

namespace AIPOC
{
    class Program
    {
        public static ILog log;

        #region OLSA Call Wrappers

        /// <summary>
        /// Initiates the asset meta data.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="metadataFormat">The metadata format.</param>
        /// <param name="initiationMode">The initiation mode.</param>
        /// <param name="onsuccessclose">if set to <c>true</c> [onsuccessclose].</param>
        /// <returns></returns>
        /// <exception cref="AIPOC.Exceptions.OlsaSecurityException"></exception>
        public static string AI_InitiateAssetMetaData(OlsaPortTypeClient client, assetMetadataFormat metadataFormat, assetInitiationMode initiationMode, bool onsuccessclose = false)
        {
            HandleResponse handleResponse = new HandleResponse();

            try
            {
                log.InfoFormat("Sending AI_InitiateAssetMetadata Request. Format: {0} Mode: {1}", metadataFormat, initiationMode);
                InitiateAssetMetaDataRequest request = new InitiateAssetMetaDataRequest();

                //Pull the OlsaAuthenticationBehviour so we can extract the customerid
                AuthenticationBehavior olsaCredentials = (AuthenticationBehavior)client.ChannelFactory.Endpoint.Behaviors.Where(p => p.GetType() == typeof(AuthenticationBehavior)).FirstOrDefault();
                request.customerId = olsaCredentials.UserName;
                request.initiationMode = initiationMode;
                request.metadataFormat = metadataFormat;

                handleResponse = client.AI_InitiateAssetMetaData(request);
            }
            catch (WebException)
            {
                // This captures any Web Exepctions such as DNS lookup errors, HTTP status errors such as 404, proxy errors etc
                // See http://msdn.microsoft.com/en-us/library/48ww3ee9(VS.80).aspx
                throw;
            }
            catch (TimeoutException)
            {
                //This captures the WCF timeout exception
                throw;
            }
            //Olsa.GeneralFault exception will be thrown for issues like parameters invalid, user does could not be created etc
            catch (FaultException<Olsa.GeneralFault>)
            {
                throw;
            }
            //WCF fault exception will be thrown for any other issues such as Security
            catch (FaultException fe)
            {
                if (fe.Message.ToLower(CultureInfo.InvariantCulture).Contains("the security token could not be authenticated or authorized"))
                {
                    //The OLSA Credentials specified could not be authenticated
                    //Check the values in the web.config are correct for OLSA.CustomerID and OLSA.SharedSecret - these are case sensitive
                    //Check the time on the machine, the SOAP message is valid for 5 minutes. This means that if the time on the calling machine
                    //is to slow OR to fast then the SOAP message will be invalid.
                    throw new Exceptions.OlsaSecurityException();
                }
                throw;
            }
            catch (CommunicationException)
            {
                throw;
            }
            catch (Exception)
            {
                //Any other type of exception, perhaps out of memory
                throw;
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == CommunicationState.Faulted)
                    {
                        client.Abort();
                    }
                    if (onsuccessclose)
                    {
                        client.Close();
                    }
                }
            }
            return handleResponse.handle;
        }

        /// <summary>
        /// Executes the specified poll for assetmetadata
        /// </summary>
        /// <param name="client">The OLSA client.</param>
        /// <param name="handle">The handle.</param>
        /// <param name="onsuccessclose">if set to <c>true</c> if call is successful we close the channel.</param>
        /// <returns>
        /// Url to report
        /// </returns>
        /// <exception cref="AIPOC.Exceptions.OlsaDataNotReadyException">The specified data is not yet ready</exception>
        /// <exception cref="AIPOC.Exceptions.OlsaSecurityException"></exception>
        public static Uri AI_PollForAssetMetaData(OlsaPortTypeClient client, string handle, bool onsuccessclose = false)
        {
            UrlResponse response;
            PollForAssetMetaDataRequest request = new PollForAssetMetaDataRequest();
            request.handle = handle;

            try
            {
                log.InfoFormat("Sending AI_PollForAssetMetaData Request. Handle: {0}", handle);
                //Pull the OlsaAuthenticationBehviour so we can extract the customerid
                AuthenticationBehavior olsaCredentials = (AuthenticationBehavior)client.ChannelFactory.Endpoint.Behaviors.Where(p => p.GetType() == typeof(AuthenticationBehavior)).FirstOrDefault();
                request.customerId = olsaCredentials.UserName;

                response = client.AI_PollForAssetMetaData(request);
            }
            catch (WebException)
            {
                // This captures any Web Exepctions such as DNS lookup errors, HTTP status errors such as 404, proxy errors etc
                // See http://msdn.microsoft.com/en-us/library/48ww3ee9(VS.80).aspx
                throw;
            }
            catch (TimeoutException)
            {
                //This captures the WCF timeout exception
                throw;
            }
            //Olsa.DataNotReadyYetFault
            catch (FaultException<Olsa.DataNotReadyYetFault> dnyrfe)
            {
                //The report has not completed generation
                throw new Exceptions.OlsaDataNotReadyException("The specified data is not yet ready", dnyrfe);
            }
            //Olsa.GeneralFault exception will be thrown for issues like parameters invalid, user does could not be created etc
            catch (FaultException<Olsa.GeneralFault>)
            {
                throw;
            }
            //WCF fault exception will be thrown for any other issues such as Security
            catch (FaultException fe)
            {
                if (fe.Message.ToLower(CultureInfo.InvariantCulture).Contains("the security token could not be authenticated or authorized"))
                {
                    //The OLSA Credentials specified could not be authenticated
                    //Check the values in the web.config are correct for OLSA.CustomerID and OLSA.SharedSecret - these are case sensitive
                    //Check the time on the machine, the SOAP message is valid for 5 minutes. This means that if the time on the calling machine
                    //is to slow OR to fast then the SOAP message will be invalid.
                    throw new Exceptions.OlsaSecurityException();
                }
                throw;
            }
            catch (CommunicationException)
            {
                throw;
            }
            catch (Exception)
            {
                //Any other type of exception, perhaps out of memory
                throw;
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == CommunicationState.Faulted)
                    {
                        client.Abort();
                    }
                    if (onsuccessclose)
                    {
                        client.Close();
                    }
                }
            }
            return new Uri(response.olsaURL);
        }

        /// <summary>
        /// Acks the asset meta data.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="handle">The handle.</param>
        /// <param name="onsuccessclose">if set to <c>true</c> [onsuccessclose].</param>
        /// <exception cref="AIPOC.Exceptions.OlsaSecurityException"></exception>
        public static void AI_AcknowledgeAssetMetaData(OlsaPortTypeClient client, string handle, bool onsuccessclose = false)
        {
            VoidResponse response;
            AcknowledgeAssetMetaDataRequest request = new AcknowledgeAssetMetaDataRequest();
            request.handle = handle;

            try
            {
                log.InfoFormat("Sending AI_AcknowledgeAssetMetaData Request. Handle: {0}", handle != null ? handle : "NULL");
                //Pull the OlsaAuthenticationBehviour so we can extract the customerid
                AuthenticationBehavior olsaCredentials = (AuthenticationBehavior)client.ChannelFactory.Endpoint.Behaviors.Where(p => p.GetType() == typeof(AuthenticationBehavior)).FirstOrDefault();
                request.customerId = olsaCredentials.UserName;

                response = client.AI_AcknowledgeAssetMetaData(request);
            }
            catch (WebException)
            {
                // This captures any Web Exepctions such as DNS lookup errors, HTTP status errors such as 404, proxy errors etc
                // See http://msdn.microsoft.com/en-us/library/48ww3ee9(VS.80).aspx
                throw;
            }
            catch (TimeoutException)
            {
                //This captures the WCF timeout exception
                throw;
            }
            //Olsa.GeneralFault exception will be thrown for issues like parameters invalid, user does could not be created etc
            catch (FaultException<Olsa.GeneralFault>)
            {
                throw;
            }
            //WCF fault exception will be thrown for any other issues such as Security
            catch (FaultException fe)
            {
                if (fe.Message.ToLower(CultureInfo.InvariantCulture).Contains("the security token could not be authenticated or authorized"))
                {
                    //The OLSA Credentials specified could not be authenticated
                    //Check the values in the web.config are correct for OLSA.CustomerID and OLSA.SharedSecret - these are case sensitive
                    //Check the time on the machine, the SOAP message is valid for 5 minutes. This means that if the time on the calling machine
                    //is to slow OR to fast then the SOAP message will be invalid.
                    throw new Exceptions.OlsaSecurityException();
                }
                throw;
            }
            catch (CommunicationException)
            {
                throw;
            }
            catch (Exception)
            {
                //Any other type of exception, perhaps out of memory
                throw;
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == CommunicationState.Faulted)
                    {
                        client.Abort();
                    }
                    if (onsuccessclose)
                    {
                        client.Close();
                    }
                }
            }
        }

        #endregion


        #region High Level Functions
        /// <summary>
        /// Checks if metadata is ready, runs thru a polling cycle
        /// </summary>
        /// <param name="client">The service.</param>
        /// <param name="metadataHandle">The report handle.</param>
        /// <param name="retries">The retries.</param>
        /// <param name="interval">The interval in ms</param>
        /// <returns></returns>
        /// <exception cref="OlsaPollTimeOutException"></exception>
        private static Uri CheckIfDataReady(OlsaPortTypeClient client, string metadataHandle, int retries, int interval)
        {
            //Start Download Loop
            bool pollComplete = false;
            Uri reportUri = null;

            int _attempt = 0;
            //Loop until the report is ready OR we exceed our retries number
            do
            {
                log.InfoFormat("Waiting before checking if data ready. Attempt: {0} of {1}", _attempt + 1, retries);
                // wait before Polling for the response
                if (!pollComplete)
                {
                    Thread.Sleep(interval);
                }


                //Increment attempt
                _attempt++;

                //Check attempts not greater than number of retries  
                if (_attempt > retries)
                {
                    throw new Exceptions.OlsaPollTimeOutException(string.Format(CultureInfo.CurrentCulture, "Number of retries exceeded. retries={0} interval={1}", retries, interval));
                }

                try
                {
                    reportUri = AI_PollForAssetMetaData(client, metadataHandle);
                    pollComplete = true;
                }
                //Catch OLSA Specific Faults
                catch (Exceptions.OlsaDataNotReadyException)
                {
                    //The report requested has not completed running
                    pollComplete = false;
                }




            } while (!pollComplete);
            return reportUri;
        }

        /// <summary>
        /// This calls the AI_InitiateAssetMetadata function, then the AI_PollForAssetMetadata, saves the results (metadata_<paramref name="metadataFormat" />.zip and extracts them.
        /// </summary>
        /// <param name="client">The OLSA client.</param>
        /// <param name="metadataFormat">The metadata format.</param>
        /// <param name="initiationMode">The initiation mode.</param>
        /// <param name="acknowledge">if set to <c>true</c> send an AI_AcknowledgeAssetMetadata</param>
        /// <param name="onsuccessclose">if set to <c>true</c> [onsuccessclose].</param>
        /// <param name="pollInterval">The poll interval.</param>
        /// <returns></returns>
        static FileInfo DownloadMetadata(OlsaPortTypeClient client, assetMetadataFormat metadataFormat, assetInitiationMode initiationMode, bool acknowledge = false, bool onsuccessclose = false, int pollInterval = 5, int pollRetries = 10)
        {
            DateTime now = DateTime.UtcNow;
            string _localFile = String.Format("METADATA-{0}-{1:D4}{2:D2}{3:D2}T{4:D2}{5:D2}{6:D2}Z.zip",
                                                metadataFormat, now.Year, now.Month, now.Day,
                                                now.Hour, now.Minute, now.Second);

            FileInfo localFile = null;
            Uri metadataDownloadUri;

            string metadataHandle = null;

            try
            {
                log.InfoFormat("DownloadMetadata. Format: {0} Mode: {1}", metadataFormat, initiationMode);

                metadataHandle = AI_InitiateAssetMetaData(client, metadataFormat, initiationMode, onsuccessclose);
                log.InfoFormat("Returned Handle: {0}", metadataHandle);
            }
            catch (FaultException<Olsa.RequestAlreadyInProgressFault>)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Fatal("Issue while submitting request", ex);
                throw;
            }


            if (!string.IsNullOrEmpty(metadataHandle))
            {
                //We have a report handle so poll every x minutes until report ready or for max y attempts
                int interval = (int)(pollInterval * 60 * 1000);
                int retries = pollRetries;

                log.InfoFormat("Starting Loop to check if data is ready. Interval: {0} minutes. Retries:{1}", pollInterval, retries);

                //Poll for the report
                try
                {
                    metadataDownloadUri = CheckIfDataReady(client, metadataHandle, retries, interval);
                    log.InfoFormat("Returned Url: {0}", metadataDownloadUri.OriginalString);
                }
                catch (Exception ex1)
                {
                    log.Fatal("Issue while checking if data ready", ex1);
                    throw;
                }


                log.InfoFormat("Starting Download of file. Saving to: {0}", _localFile);
                using (WebClient webClient = new WebClient())
                {
                    try
                    {
                        webClient.DownloadFile(metadataDownloadUri, _localFile);
                        log.Info("Download Completed");
                    }
                    catch (Exception ex2)
                    {
                        log.Fatal("Issue while downloading report", ex2);
                        throw;
                    }
                }

                if (client != null)
                {
                    if (client.State == CommunicationState.Faulted)
                    {
                        client.Abort();
                    }
                }
            }

            if (acknowledge)
            {
                log.InfoFormat("Sending Acknowledgement. Handle: {0}", metadataHandle);
                AI_AcknowledgeAssetMetaData(client, metadataHandle);
            }
            else
            {
                //Send NULL Handle Ack
                log.Info("Sending Acknowledgement. Null Handle");
                AI_AcknowledgeAssetMetaData(client, null);
            }


            localFile = new FileInfo(_localFile);
            return localFile;
        }
        #endregion

        #region Data Processing Functions
        /// <summary>
        /// Extracts the zip.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        static string ExtractZip(FileInfo filename)
        {
            string destinationFolder = Path.GetFileNameWithoutExtension(filename.Name);

            log.InfoFormat("Extracting files from {0} to {1}", filename, destinationFolder);

            ZipFile.ExtractToDirectory(filename.FullName, destinationFolder);

            log.Info("Extract Complete");
            return destinationFolder;
        }

        /// <summary>
        /// Gets the xpath navigator for customer catalog XML.
        /// </summary>
        /// <param name="xmlFolder">The XML folder.</param>
        /// <returns></returns>
        static XPathNavigator GetXpathNavigatorForCustomerCatalogXML(string xmlFolder)
        {
            DirectoryInfo d = new DirectoryInfo(xmlFolder);
            FileInfo catalog = d.GetFiles("Customer_Catalog_*.xml").FirstOrDefault();

            log.InfoFormat("Located the downloaded XML catalog: {0}", catalog.Name);
            XmlTextReader reader = new XmlTextReader(catalog.FullName);
            XPathDocument document = new XPathDocument(reader);
            return document.CreateNavigator();
        }


        /// <summary>
        /// Gets the combined object from metadata.
        /// </summary>
        /// <param name="assetid">The assetid.</param>
        /// <param name="status">The status.</param>
        /// <param name="navigator">The navigator.</param>
        /// <param name="aiccFolder">The aicc folder.</param>
        /// <returns></returns>
        /// TODO: NO Error Checking
        static CombinedAssetObject GetCombinedObject(string assetid, string status, XPathNavigator navigator, string aiccFolder)
        {
            CombinedAssetObject results = new CombinedAssetObject();
            results.ASSETID = assetid;
            results.STATUS = status;

            //If status NOT not_entitled then extract the extra data
            if (!status.Equals("not_entitled", StringComparison.InvariantCultureIgnoreCase))
            {
                //Get AICC first

                string baseFolder = string.Format("{0}\\{1}\\", aiccFolder, assetid);
                string aufile = string.Format("{0}\\{1}.AU", baseFolder, assetid);
                string crsfile = string.Format("{0}\\{1}.CRS", baseFolder, assetid);
                string cstfile = string.Format("{0}\\{1}.CST", baseFolder, assetid);
                string desfile = string.Format("{0}\\{1}.DES", baseFolder, assetid);
                string ortfile = string.Format("{0}\\{1}.ORT", baseFolder, assetid);

                log.InfoFormat("Populating the CombinedAssetObject by loading content of each AICC file as a string. Base Filename: {0}\\{1}.*", baseFolder, assetid);
                log.DebugFormat("Reading AU File: {0}", aufile);
                results.AU = System.IO.File.ReadAllText(aufile);

                log.DebugFormat("Reading CRS File: {0}", crsfile);
                results.CRS = System.IO.File.ReadAllText(crsfile);

                log.DebugFormat("Reading CST File: {0}", cstfile);
                results.CST = System.IO.File.ReadAllText(cstfile);

                log.DebugFormat("Reading DES File: {0}", desfile);
                results.DES = System.IO.File.ReadAllText(desfile);

                log.DebugFormat("Reading ORT File: {0}", ortfile);
                results.ORT = System.IO.File.ReadAllText(ortfile);


                //Read the XML
                
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(navigator.NameTable);
                nsmgr.AddNamespace("olsa", "http://www.skillsoft.com/services/olsa_v1_0/");
                nsmgr.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");

                string xpath = "//olsa:asset/dc:identifier[text()='" + assetid + "']/parent::*";
                log.InfoFormat("Populating the CombinedAssetObject by reading the elements from XML. xpath: {0}", xpath);
                var node = navigator.SelectSingleNode(xpath, nsmgr);

                log.DebugFormat("Select Language using xPath at the Node: {0}", "//dc:language");
                results.LANGUAGE = node.SelectSingleNode("//dc:language", nsmgr) == null ? "" : node.SelectSingleNode("//dc:language", nsmgr).Value;
                log.DebugFormat("Select Description using xPath at the Node: {0}", "//dc:description");
                results.DESCRIPTION = node.SelectSingleNode("//dc:description", nsmgr) == null ? "" : node.SelectSingleNode("//dc:description", nsmgr).Value;
                log.DebugFormat("Select Duration using xPath at the Node: {0}", "//olsa:description");
                results.DURATION = node.SelectSingleNode("//olsa:duration", nsmgr) == null ? "" : node.SelectSingleNode("//olsa:duration", nsmgr).Value;
            }
            return results;
        }
        #endregion



        /// <summary>
        /// Gets the olsa client
        /// </summary>
        /// <param name="olsaServerEndpoint">The olsa server endpoint.</param>
        /// <param name="olsaCustomerId">The olsa customer identifier.</param>
        /// <param name="olsaSharedSecret">The olsa shared secret.</param>
        /// <returns></returns>
        private static OlsaPortTypeClient GetOLSAClient(Uri olsaServerEndpoint, string olsaCustomerId, string olsaSharedSecret)
        {
            //Set the encoding to SOAP 1.1, Disable Addressing and set encoding to UTF8
            TextMessageEncodingBindingElement messageEncoding = new TextMessageEncodingBindingElement();
            messageEncoding.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap11, AddressingVersion.None);
            messageEncoding.WriteEncoding = Encoding.UTF8;

            //Setup Binding Elemment
            HttpTransportBindingElement transportBinding = new HttpsTransportBindingElement();

            //Set the maximum received messages sizes to 1Mb
            transportBinding.MaxReceivedMessageSize = 1024 * 1024;
            transportBinding.MaxBufferPoolSize = 1024 * 1024;

            //Create the CustomBinding
            Binding customBinding = new CustomBinding(messageEncoding, transportBinding);

            //Create the OLSA Service
            EndpointAddress serviceAddress = new EndpointAddress(olsaServerEndpoint);

            //Set the endPoint URL YOUROLSASERVER/olsa/services/Olsa has to be HTTPS
            OlsaPortTypeClient service = new OlsaPortTypeClient(customBinding, serviceAddress);

            //Add Behaviour to support SOAP UserNameToken Password Digest
            AuthenticationBehavior behavior1 = new AuthenticationBehavior(olsaCustomerId, olsaSharedSecret);
            service.Endpoint.Behaviors.Add(behavior1);

            //Add Behaviour to support fix of Namespaces to address AXIS2 / VWCF incompatability
            NameSpaceFixUpBehavior behavior2 = new NameSpaceFixUpBehavior();
            service.Endpoint.Behaviors.Add(behavior2);

            return service;
        }

        /// <summary>
        /// Processes the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="assetMode">The asset mode.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="retries">The retries.</param>
        static void Process(OlsaPortTypeClient client, assetInitiationMode assetMode = assetInitiationMode.all, int interval = 1, int retries = 10)
        {
            log.InfoFormat("Starting Process. assetInitiationMode : {0}", assetMode.ToString());


            //Best practice for OLSA AI cycle, on startup - send NULL Acknowledgement to cancel any existing pending requests
            AI_AcknowledgeAssetMetaData(client, null);

            //Call and get the XML data, send NULL acknowledgement
            var xmlResultsFile = DownloadMetadata(client, assetMetadataFormat.XML, assetMode, false, false, interval, retries);

            //Call and get the AICC data, send acknowledgement of the returned handled
            var aiccResultsFile = DownloadMetadata(client, assetMetadataFormat.AICC, assetMode, true, false, interval, retries);

            //Extract the downloaded ZIP metadata
            var xmlFolder = ExtractZip(xmlResultsFile);
            var aiccFolder = ExtractZip(aiccResultsFile);

            //Now perform AI processing
            List<CombinedAssetObject> combinedObjects = new List<CombinedAssetObject>();

            //Loop thru the XML _ss_entitlements.xml from the AICC folder
            XmlTextReader reader = new XmlTextReader(string.Format("{0}\\{1}", aiccFolder, "_ss_entitlement_status.xml"));
            XPathDocument document = new XPathDocument(reader);
            XPathNavigator navigator = document.CreateNavigator();

            XPathNavigator catalogNavigator = GetXpathNavigatorForCustomerCatalogXML(xmlFolder);

            //Get the ASSET nodes
            XPathNodeIterator nodeIterator = navigator.Select("//ASSET");
            while (nodeIterator.MoveNext())
            {
                if (nodeIterator.Current.HasAttributes)
                {
                    //Extract the ID/STATUS
                    var id = nodeIterator.Current.GetAttribute("ID", "");
                    var status = nodeIterator.Current.GetAttribute("STATUS", "");
                    //Generate a combine object using the XML and AICC data
                    var result = GetCombinedObject(id, status, catalogNavigator, aiccFolder);
                    combinedObjects.Add(result);
                }
            }
            DateTime now = DateTime.UtcNow;
            string jsonFilename = String.Format("JSONDATA-{0:D4}{1:D2}{2:D2}T{3:D2}{4:D2}{5:D2}Z.json",
                                              now.Year, now.Month, now.Day,
                                              now.Hour, now.Minute, now.Second);

            log.InfoFormat("Saving Results to JSON. File : {0}", jsonFilename);

            string jsonStr = JsonConvert.SerializeObject(combinedObjects);
            File.WriteAllText(jsonFilename, jsonStr);
        }


        static void Main(string[] args)
        {
            log = LogManager.GetLogger("AI_POC");

            OlsaPortTypeClient client = null;

            if (!Parser.TryParse(args, out Options options))
            {
                return;
            }
            try
            {
                client = GetOLSAClient(new Uri(options.endpoint), options.customerid, options.sharedsecret);
                Process(client, options.mode, options.interval, options.retries);
            }
            catch (Exception ex)
            {
                log.FatalFormat("Issue while Processing.", ex);
            }
        }
    }
}
