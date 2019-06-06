using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using SIS.MvcFramework;
using System.Reflection;
using System.Text.RegularExpressions;

namespace IRunes.App
{
    public static class Program
    {
        public static void Main()
        {
            WebHost.Start(new Startup());
        }
    }
}