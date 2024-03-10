using Grasshopper.Kernel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PluginTemplate.PluginGrasshopper
{
    public class ListEPDs : GH_Component
    {
        public ListEPDs()
          : base("List EPDs", "ListEPDs",
              "List EPDs names and UUIDs from JSON data",
              "goeko", "Read")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("JSONData", "JSONData", "JSON data to parse", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Names", "Names", "Names extracted from JSON data", GH_ParamAccess.list);
            pManager.AddTextParameter("UUIDs", "UUIDs", "UUIDs extracted from JSON data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string jsonData = "";

            DA.GetData(0, ref jsonData);

            if (!string.IsNullOrWhiteSpace(jsonData))
            {
                try
                {
                    var names = new List<string>();
                    var uuids = new List<string>();

                    JObject jsonObject = JObject.Parse(jsonData);
                    JArray data = (JArray)jsonObject["data"];

                    foreach (JToken item in data)
                    {
                        string name = item["name"].ToString();
                        string uuid = item["uuid"].ToString();

                        names.Add(name);
                        uuids.Add(uuid);
                    }

                    DA.SetDataList(0, names);
                    DA.SetDataList(1, uuids);
                }
                catch (Exception ex)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Error parsing JSON data: {ex.Message}");
                }
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "JSON data is empty.");
            }
        }

        protected override System.Drawing.Bitmap Icon => ResourceLoader.LoadBitmap("PluginGrasshopper_24.png");

        public override Guid ComponentGuid => new Guid("9874febf-56d9-40f6-8aa0-7b9efec4c5f2");
    }
}
