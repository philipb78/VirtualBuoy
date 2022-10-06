using Models.Admin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class SettingsDatabase
    {
        public Settings GetSettings()
        {
            Settings settings;
            using (SailDisplayDataContext sailDisplayDataContext = new SailDisplayDataContext())
            {
                settings = sailDisplayDataContext.Settings.Find(1);
                if (settings == null)
                {
                    settings = new Settings();
                    settings.Id = 1;
                    settings.TackAngle = 90;
                    sailDisplayDataContext.Add(settings);
                    sailDisplayDataContext.SaveChanges();
                }
            }
            return settings;
        }

        public void UpdateSettings(Settings settings)
        {
            using (SailDisplayDataContext sailDisplayDataContext = new SailDisplayDataContext())
            {
                sailDisplayDataContext.Update(settings);
                sailDisplayDataContext.SaveChanges();
            }
        }
    }
}