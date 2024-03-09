using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.IO;
using System.Net;

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
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Data", "Data", "Loaded data from the database", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            string uuid = "";

            DA.GetData(0, ref run);
            DA.GetData(1, ref uuid); // Get the UUID parameter

            if (run && !string.IsNullOrWhiteSpace(uuid))
            {
                string loadedData = LoadDataFromDatabase(uuid);
                DA.SetData(0, loadedData);
            }
        }

        private string LoadDataFromDatabase(string uuid)
        {
            string apiUrl = $"https://www.oekobaudat.de/OEKOBAU.DAT/resource/processes/{uuid}";

            string responseData = "";

            try
            {
                WebRequest request = WebRequest.Create(apiUrl);
                request.Timeout = 5000; // Reduced timeout to 5 seconds
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

        public override Guid ComponentGuid => new Guid("f9fa9c30-76c4-45a9-92ac-df104238634e");
    }
}
