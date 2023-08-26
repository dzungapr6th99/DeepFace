using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClrProject
{
    public class ConfigData
    {
        public static string OpenCVPath;
        public static string ImagePath;

        public static void InitConfig(IConfigurationRoot configurationRoot)
        {
            OpenCVPath = configurationRoot["OpenCVPath"]?.ToString();
            ImagePath = configurationRoot["ImagePath"]?.ToString();
            
        }
    }
}
