/*
 * Author : Nguyen Nhat Linh – Navisoft.
 * Summary: Chứa các biến cấu hình đọc được từ file config và cấu hình trong DB
 * Modification Logs:
 * DATE             AUTHOR      DESCRIPTION
 * --------------------------------------------------------
 * Mar 27, 2021  	Linh.Nguyen     Created
 */


using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CommonLib
{
    public class ConfigData
    {
        public static char SOH;
        public static string Cascade_Path; 
        public static List<string> ListPreModel = new List<string>();
        public static Dictionary<string, string> ListPreModelPath = new Dictionary<string, string>();
        public static string OpenCV_Wrapper_Path;
        public static string OpenCV_Wrapper_Eye_Path;
        public static void InitConfigData(IConfigurationRoot configurationRoot)
        {
            SOH = (char)0x01;
            Cascade_Path = "";
            


        }
    }
}