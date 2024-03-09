using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.IO;
using System.Net;
using System.Xml;
using Newtonsoft.Json;

namespace PluginTemplate.PluginGrasshopper
{
    public class FindMaterial : GH_Component
    {
        public FindMaterial()
            : base("Find Material by UUID", "FindMat",
                "Loads material through API",
                "goeko", "API")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "Run", "Set to true to execute loading process", GH_ParamAccess.item);
            pManager.AddTextParameter("UUID", "UUID", "UUID of the material to retrieve", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Timeout", "Timeout", "Timeout in seconds (up to 20 seconds)", GH_ParamAccess.item, 5);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Data", "Data", "Loaded data from the database", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            string uuid = "";
            int timeout = 5;

            DA.GetData(0, ref run);
            DA.GetData(1, ref uuid); // Get the UUID parameter
            DA.GetData(2, ref timeout); // Get the Timeout parameter

            // Ensure the timeout value is within the range of 1 to 20 seconds
            timeout = Math.Max(1, Math.Min(timeout, 20));

            if (run && !string.IsNullOrWhiteSpace(uuid))
            {
                string loadedData = LoadDataFromDatabase(uuid, timeout * 1000); // Convert timeout to milliseconds
                DA.SetData(0, loadedData);
            }
        }

        private string LoadDataFromDatabase(string uuid, int timeoutMilliseconds)
        {
            string apiUrl = $"https://www.oekobaudat.de/OEKOBAU.DAT/resource/processes/{uuid}";

            string responseData = "";

            try
            {
                WebRequest request = WebRequest.Create(apiUrl);
                request.Timeout = timeoutMilliseconds; // Set the timeout
                request.Method = "GET";

                using (WebResponse response = request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseData = reader.ReadToEnd();
                }

                // Process XML data and convert to JSON
                responseData = ProcessXmlData(responseData);
            }
            catch (WebException e)
            {
                responseData = e.Message;
            }

            return responseData;
        }

        private string ProcessXmlData(string xmlData)
        {
            // Load XML data into XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            // Create a namespace manager
            XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsManager.AddNamespace("sapi", "http://www.ilcd-network.org/ILCD/ServiceAPI");

            // Select the 'sapi:name' element using the namespace manager
            XmlNode nameNode = xmlDoc.SelectSingleNode("//sapi:name", nsManager);

            // Get the value of the 'sapi:name' element
            string nameValue = nameNode.InnerText;

            return nameValue;
        }

        protected override System.Drawing.Bitmap Icon => ResourceLoader.LoadBitmap("PluginGrasshopper_24.png");

        public override Guid ComponentGuid => new Guid("f9fa9c30-76c4-45a9-92ac-df104238634e");
    }
}
