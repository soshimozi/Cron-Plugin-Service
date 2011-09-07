using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ProtectAppConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            Program prog = new Program();
            prog.Run(args);
        }

        public void Run(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: ProtectAppConfig <path to executable>");
            }
            else
            {
                ProtectConfig(args[0]);
            }
        }

        private void ProtectConfig(string exePath)
        {
            // Takes the executable file name without the
            // .config extension.
            try
            {
                // Open the configuration file and retrieve 
                // the connectionStrings section.
                Configuration config = ConfigurationManager.
                    OpenExeConfiguration(exePath);

                ConnectionStringsSection section =
                    config.GetSection("connectionStrings")
                    as ConnectionStringsSection;

                if (section != null)
                {
                    if (!section.SectionInformation.IsProtected)
                    {
                        // Encrypt the section.
                        section.SectionInformation.ProtectSection(
                            "DataProtectionConfigurationProvider");

                        // Save the current configuration.
                        config.Save();
                    }
                }

                Console.WriteLine("{0}.config is now protected.", exePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
