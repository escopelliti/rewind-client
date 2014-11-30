using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

using KeyboardMouseController;

namespace ClientConfiguration
{
    public class ConfigurationManager
    {
        public string path { get; set; }
        
        public ConfigurationManager() 
        {
            path = @"../../resources/config.json";
            
            if (!File.Exists(path))
            {
                WriteConfigFile(createStdConfiguration());
            }
        }

        public Configuration createStdConfiguration()
        {
            Configuration stdConfiguration = new Configuration();
            stdConfiguration.hotkeyList.Add(
               new Hotkey(System.Windows.Input.ModifierKeys.Control, System.Windows.Input.Key.Space, Hotkey.SWITCH_SERVER_CMD));
            stdConfiguration.hotkeyList.Add(
                new Hotkey(System.Windows.Input.ModifierKeys.Control, System.Windows.Input.Key.Enter, Hotkey.OPEN_PANEL_CMD));
            stdConfiguration.hotkeyList.Add(
                new Hotkey(System.Windows.Input.ModifierKeys.Control, System.Windows.Input.Key.P, Hotkey.REMOTE_PAST_CMD));
            return stdConfiguration;
        }

        public Configuration ReadConfiguration()
        {
            Configuration config = new Configuration();
            string confRead = string.Empty;

            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader infile = new StreamReader(fs))
                    {
                        confRead = infile.ReadToEnd();
                    }
                }
            }
            catch(FileNotFoundException) 
            {
                config = createStdConfiguration();
                WriteConfigFile(config);
                return config;
            }
            catch(Exception)
            {
                return createStdConfiguration();
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
            WriteConfigFile(config);
        }

        public void WriteConfigFile(Configuration newConfig)
        {
            string s = JsonConvert.SerializeObject(newConfig, Formatting.Indented);

            using (FileStream fs = new FileStream(path,FileMode.Create))
            {
                using (StreamWriter outfile = new StreamWriter(fs))
                {
                    try 
                    {
                        outfile.Write(s);
                    }

                    catch(Exception)
                    {
                        return;
                    }
                }
            }
        }
    }
}
