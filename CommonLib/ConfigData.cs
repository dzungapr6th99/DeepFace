﻿/*
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
        public static string ModelDetector_Face_Path;
        public static string ModelDetector_Eye_Path;
        public static string ModelVerifyPath;
        public static string ModelPath;
        public static bool IsRunOnGpu = false;
        public static Dictionary<string, Dictionary<string, float>> DictThreshold;
        public static void InitConfigData(IConfigurationRoot configurationRoot)
        {
            LOG.log.Info("Start init");
            ModelPath = configurationRoot["ModelPath"]?.ToString();
            ModelDetector_Face_Path = Path.Combine(ModelPath, configurationRoot["ModelFaceDetector"]?.ToString());
            ModelDetector_Eye_Path = Path.Combine(ModelPath, configurationRoot["ModelEyesDetector"]?.ToString());
            ModelVerifyPath = Path.Combine(ModelPath, configurationRoot["ModelVerifyFace"]?.ToString());
            DictThreshold = configurationRoot.GetSection("Threshold").Get<Dictionary<string, Dictionary<string, float>>>();
            IsRunOnGpu = configurationRoot["UseGpu"]?.ToString() == "true";
            LOG.log.Info("Init configuration success");
        }
    }
}