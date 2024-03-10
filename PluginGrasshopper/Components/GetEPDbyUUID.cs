using System;
using System.IO;
using System.Net;
using System.Xml;
using Grasshopper.Kernel;
using Newtonsoft.Json;

namespace PluginTemplate.PluginGrasshopper
{
    public class GetEPDbyUUID : GH_Component
    {
        private const string OKOBAU_URL = "https://oekobaudat.de/OEKOBAU.DAT/resource/datastocks/cd2bda71-760b-4fcc-8a0b-3877c10000a8";

        public GetEPDbyUUID()
            : base("Get EPD by UUID", "GetEPDbyUUID",
                "Get EPD by UUID from Ökobau",
                "goeko", "API")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "Run", "Set to true to execute loading process", GH_ParamAccess.item, false);
            pManager.AddTextParameter("UUID", "UUID", "UUID of the EPD to retrieve", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Data", "Data", "Loaded EPD data from Ökobau", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            string uuid = "";

            DA.GetData(0, ref run);
            DA.GetData(1, ref uuid);

            if (run && !string.IsNullOrWhiteSpace(uuid))
            {
                string epdData = GetFullEPDData(uuid);
                DA.SetData(0, epdData);
            }
            else
            {
                DA.SetData(0, "Component not executed. Set 'Run' to true and provide a valid UUID to retrieve EPD data.");
            }
        }

        private string GetFullEPDData(string uuid)
        {
            string responseData = "";

            try
            {
                string apiUrl = $"{OKOBAU_URL}/processes/{uuid}?format=json&view=extended";
                WebRequest request = WebRequest.Create(apiUrl);
                request.Method = "GET";

                using (WebResponse response = request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseData = reader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                responseData = e.Message;
            }

            return responseData;
        }

        protected override System.Drawing.Bitmap Icon => ResourceLoader.LoadBitmap("PluginGrasshopper_24.png");

        public override Guid ComponentGuid => new Guid("f9fa9c30-76c4-45a9-92ab-df114238634e");
    }
}

