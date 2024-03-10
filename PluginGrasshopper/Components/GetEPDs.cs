using System;
using System.IO;
using System.Net;
using System.Xml;
using Grasshopper.Kernel;
using Newtonsoft.Json;

namespace PluginTemplate.PluginGrasshopper
{
    public class GetEPDs : GH_Component
    {
        private const string OKOBAU_URL = "https://oekobaudat.de/OEKOBAU.DAT/resource/datastocks/cd2bda71-760b-4fcc-8a0b-3877c10000a8";

        public GetEPDs()
            : base("Get EPDs from Ökobau", "GetEPDs",
                "Get EPDs from Ökobau",
                "goeko", "API")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "Run", "Set to true to execute loading process", GH_ParamAccess.item, false);
            pManager.AddIntegerParameter("Limit", "Limit", "Limit the number of EPDs to retrieve", GH_ParamAccess.item, 10);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Data", "Data", "Loaded EPD data from Ökobau", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            int limit = 10;

            DA.GetData(0, ref run);
            DA.GetData(1, ref limit);

            if (run)
            {
                string epdData = GetEPDData(limit);
                DA.SetData(0, epdData);
            }
            else
            {
                DA.SetData(0, "Component not executed. Set 'Run' to true to retrieve EPD data.");
            }
        }

        private string GetEPDData(int limit)
        {
            string responseData = "";

            try
            {
                string apiUrl = $"{OKOBAU_URL}/processes?format=json&pageSize={limit}";
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

        public override Guid ComponentGuid => new Guid("32c2713d-749c-4903-9c9e-d2ef70ad38fc");
    }
}
