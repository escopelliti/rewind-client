using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace WpfApplication1
{
    public class ConfigurationManager
    {
        public string path { get; set; } 

        public ConfigurationManager() 
        {
            path = @"../../resources/config.json";
            
            if (!File.Exists(path))
            {
                CreateConfigurationFile();
            }
            
        }

        public void CreateConfigurationFile()
        {
            Configuration stdConfiguration = new Configuration();
                 
            stdConfiguration.hotkeyList.Add(
                new Hotkey(System.Windows.Input.ModifierKeys.Control, System.Windows.Input.Key.Space, Hotkey.SWITCH_SERVER_CMD));
            stdConfiguration.hotkeyList.Add(
                new Hotkey(System.Windows.Input.ModifierKeys.Control, System.Windows.Input.Key.Enter, Hotkey.OPEN_PANEL_CMD));
            
            string s = JsonConvert.SerializeObject(stdConfiguration, Formatting.Indented);

            using (FileStream fs = new FileStream(path, FileMode.CreateNew))
            {
                using (StreamWriter outfile = new StreamWriter(fs))
                {
                    outfile.Write(s);
                }
                
            }
        }

        
        public Configuration ReadConfiguration()
        {
            Configuration config = new Configuration();
            string confRead = string.Empty;

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader infile = new StreamReader(fs))
                {
                    confRead = infile.ReadToEnd(); 
                }
            }
            config = JsonConvert.DeserializeObject<Configuration>(confRead);
            return config;
        }

        public void ModifyHotkey(Hotkey newHotkey)
        {
            Configuration config = new Configuration();
            config = ReadConfiguration();


            Hotkey toRemove = config.hotkeyList.Find(x => x.Command == newHotkey.Command);
            if (toRemove != null)
            {
                config.hotkeyList.Remove(toRemove);
            }

            config.hotkeyList.Add(newHotkey);

            UpdateConfigFile(config);
            
        }

        public void UpdateConfigFile(Configuration newConfig)
        {
            string s = JsonConvert.SerializeObject(newConfig, Formatting.Indented);

            using (FileStream fs = new FileStream(path,FileMode.Create))
            {
                using (StreamWriter outfile = new StreamWriter(fs))
                {
                    outfile.Write(s);
                }

            }
        }
    }
}
