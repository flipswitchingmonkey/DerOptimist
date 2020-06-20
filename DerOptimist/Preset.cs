using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerOptimist
{
    [Serializable]
    public class Preset
    {
        public string Name { get; set; } = "Presets";
        public bool IsFolder { get; set; } = true;
        public string PresetFileName { get; set; }
        public RenderSettingsItem Settings { get; set; }

        public ObservableCollection<Preset> Items { get; set; }

        public Preset()
        {
            this.Items = new ObservableCollection<Preset>();
        }
    }
}
