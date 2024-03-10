using Grasshopper.Kernel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PluginTemplate.PluginGrasshopper
{
    public class ParseEPD : GH_Component
    {
        public ParseEPD()
            : base("Parse EPD", "ParseEPD",
                "Parse EPD result and extract information",
                "goeko", "Read")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("EPDResult", "EPDResult", "EPD result to parse", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "Name", "Name parsed from EPD result", GH_ParamAccess.item);
            pManager.AddTextParameter("Unit", "Unit", "Unit parsed from EPD result", GH_ParamAccess.item);
            pManager.AddNumberParameter("Amount", "Amount", "Amount parsed from EPD result", GH_ParamAccess.item);
            pManager.AddTextParameter("GWPData", "GWPData", "GWP data parsed from EPD result", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string epdResult = "";

            DA.GetData(0, ref epdResult);

            if (!string.IsNullOrWhiteSpace(epdResult))
            {
                try
                {
                    string name = "";
                    string unit = "";
                    double amount = 0;
                    JObject epdObject = JObject.Parse(epdResult);

                    // Extract English name
                    JToken nameToken = epdObject.SelectToken("processInformation.dataSetInformation.name.baseName")
                        .FirstOrDefault(x => x["lang"].ToString() == "en")?["value"];
                    if (nameToken != null)
                        name = nameToken.ToString();

                    // Extract unit and amount
                    JToken flowPropertiesToken = epdObject.SelectToken("exchanges.exchange[0].flowProperties[0]");
                    if (flowPropertiesToken != null)
                    {
                        unit = flowPropertiesToken["referenceUnit"]?.ToString();
                        if (double.TryParse(flowPropertiesToken["meanValue"]?.ToString(), out double parsedAmount))
                        {
                            amount = parsedAmount;
                        }
                        else
                        {
                            throw new Exception("Failed to parse meanValue to double.");
                        }
                    }

                    // Extract GWP data
                    string gwpData = ExtractGWP(epdObject);

                    DA.SetData(0, name);
                    DA.SetData(1, unit);
                    DA.SetData(2, amount);
                    DA.SetData(3, gwpData);
                }
                catch (Exception ex)
                {
                    DA.SetData(0, "Error parsing EPD result: " + ex.Message);
                    DA.SetData(1, "");
                    DA.SetData(2, 0);
                    DA.SetData(3, "");
                }
            }
            else
            {
                DA.SetData(0, "EPD result is empty.");
                DA.SetData(1, "");
                DA.SetData(2, 0);
                DA.SetData(3, "");
            }
        }

        private string ExtractGWP(JObject epdObject)
        {
            var gwpData = new Dictionary<string, double>();

            try
            {
                var gwpResults = epdObject.SelectToken("LCIAResults.LCIAResult[0].other.anies");
                if (gwpResults != null)
                {
                    foreach (var result in gwpResults)
                    {
                        var module = result["module"]?.ToString();
                        var value = result["value"]?.ToString();

                        if (!string.IsNullOrEmpty(module) && !string.IsNullOrEmpty(value))
                        {
                            // Parse value to double
                            double parsedValue;
                            if (double.TryParse(value, out parsedValue))
                            {
                                // If the module exists in the dictionary, add the value
                                if (gwpData.ContainsKey(module))
                                {
                                    gwpData[module] += parsedValue;
                                }
                                else
                                {
                                    gwpData.Add(module, parsedValue);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error extracting GWP data: " + e.Message);
            }

            // If A1-A3 is not present, calculate and add it
            if (!gwpData.ContainsKey("A1-A3"))
            {
                double sumA1A3 = (gwpData.ContainsKey("A1") ? gwpData["A1"] : 0) +
                                 (gwpData.ContainsKey("A2") ? gwpData["A2"] : 0) +
                                 (gwpData.ContainsKey("A3") ? gwpData["A3"] : 0);
                gwpData.Add("A1-A3", sumA1A3);
            }

            // Remove individual A1, A2, A3 entries
            foreach (var module in new List<string> { "A1", "A2", "A3" })
            {
                gwpData.Remove(module);
            }

            // Construct the output string
            List<string> formattedResults = new List<string>();
            foreach (var kv in gwpData)
            {
                formattedResults.Add($"{kv.Key}: {kv.Value}");
            }

            return string.Join(", ", formattedResults);
        }





        protected override System.Drawing.Bitmap Icon => ResourceLoader.LoadBitmap("PluginGrasshopper_24.png");

        public override Guid ComponentGuid => new Guid("98a9c3b5-9911-4a7f-85ec-d56c1cfe62f8");
    }
}
