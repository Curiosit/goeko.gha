using Grasshopper.Kernel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace PluginTemplate.PluginGrasshopper
{
    public class ParseGWP : GH_Component
    {
        public ParseGWP()
          : base("Parse GWP Data", "ParseGWP",
              "Parse GWP data from EPD result",
              "goeko", "Read")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("EPDResult", "EPDResult", "EPD result to parse", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("A1-A3", "A1-A3", "GWP data for A1-A3", GH_ParamAccess.item);
            pManager.AddNumberParameter("A4", "A4", "GWP data for A4", GH_ParamAccess.item);
            pManager.AddNumberParameter("A5", "A5", "GWP data for A5", GH_ParamAccess.item);
            pManager.AddNumberParameter("B1", "B1", "GWP data for B1", GH_ParamAccess.item);
            pManager.AddNumberParameter("C1", "C1", "GWP data for C1", GH_ParamAccess.item);
            pManager.AddNumberParameter("C2", "C2", "GWP data for C2", GH_ParamAccess.item);
            pManager.AddNumberParameter("C3", "C3", "GWP data for C3", GH_ParamAccess.item);
            pManager.AddNumberParameter("C4", "C4", "GWP data for C4", GH_ParamAccess.item);
            pManager.AddNumberParameter("D", "D", "GWP data for D", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string epdResult = "";

            DA.GetData(0, ref epdResult);

            if (!string.IsNullOrWhiteSpace(epdResult))
            {
                try
                {
                    string[] results = epdResult.Split(',');

                    foreach (string result in results)
                    {
                        string[] parts = result.Split(':');
                        string category = parts[0].Trim();
                        double value = double.Parse(parts[1].Trim());
                        DA.SetData(category, value);
                    }
                }
                catch (Exception ex)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Error parsing GWP data: {ex.Message}");
                }
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "EPD result is empty.");
            }
        }

        protected override System.Drawing.Bitmap Icon => ResourceLoader.LoadBitmap("PluginGrasshopper_24.png");

        public override Guid ComponentGuid => new Guid("bf9e2c05-3179-4c38-a145-4b3dfe0c0f79");
    }
}
