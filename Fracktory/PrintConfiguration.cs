using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fracktory
{
    class PrintConfiguration
    {
        public Dictionary<String, String> ExtraConfiguration = new Dictionary<string, string>();
        public PrintConfiguration()
        {
            ExtraConfiguration["layer-height"] = "0.2";
            ExtraConfiguration["spiral-vase"] = "0";
            ExtraConfiguration["fill-density"] = "0.15";
            ExtraConfiguration["infill-every-layers"] = "1";
            ExtraConfiguration["infill-only-where-needed"] = "0";
            ExtraConfiguration["solid-infill-every-layers"] = "0";
            ExtraConfiguration["solid-infill-below-area"] = "70";
            ExtraConfiguration["only-retract-when-crossing-perimeters"] = "1";
            ExtraConfiguration["support-material"] = "0";
            ExtraConfiguration["temperature"] = "240";
            ExtraConfiguration["first-layer-temperature"] = "240";
            ExtraConfiguration["bed-temperature"] = "110";
            ExtraConfiguration["first-layer-bed-temperature"] = "110";
            ExtraConfiguration["skirt-distance"] = "6";
            ExtraConfiguration["skirt-height"] = "1";
            ExtraConfiguration["skirts"] = "1";
            ExtraConfiguration["min-skirt-length"] = "0";
            ExtraConfiguration["brim-width"] = "0";
            ExtraConfiguration["rotate"] = "0";
            ExtraConfiguration["scale"] = "1";

        }

        public String generateString()
        {
            string param = "";
            foreach (KeyValuePair<String, String> item in ExtraConfiguration)
                param += " --" + item.Key + " " + item.Value;
            return param;
        }

    }
}
