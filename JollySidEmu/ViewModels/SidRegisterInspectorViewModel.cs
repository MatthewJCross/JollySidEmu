using System.Collections.ObjectModel;
using JollySidEmu.Audio;
using JollySidEmu.Models;

namespace JollySidEmu.ViewModels
{
    public class SidRegisterInspectorViewModel : ViewModelBase
    {
        public ObservableCollection<SidRegisterRow> Registers { get; } = new ObservableCollection<SidRegisterRow>();

        private readonly SidRegisters _sid;

        public SidRegisterInspectorViewModel(SidRegisters sid)
        {
            _sid = sid;

            Add(0xD400, "V1 Freq Lo");
            Add(0xD401, "V1 Freq Hi");
            Add(0xD402, "V1 PW Lo");
            Add(0xD403, "V1 PW Hi");
            Add(0xD404, "V1 Control");
            Add(0xD405, "V1 Attack/Decay");
            Add(0xD406, "V1 Sustain/Release");

            Add(0xD407, "V2 Freq Lo");
            Add(0xD408, "V2 Freq Hi");
            Add(0xD409, "V2 PW Lo");
            Add(0xD40A, "V2 PW Hi");
            Add(0xD40B, "V2 Control");
            Add(0xD40C, "V2 Attack/Decay");
            Add(0xD40D, "V2 Sustain/Release");

            Add(0xD40E, "V3 Freq Lo");
            Add(0xD40F, "V3 Freq Hi");
            Add(0xD410, "V3 PW Lo");
            Add(0xD411, "V3 PW Hi");
            Add(0xD412, "V3 Control");
            Add(0xD413, "V3 Attack/Decay");
            Add(0xD414, "V3 Sustain/Release");

            Add(0xD415, "Filter Cutoff Lo");
            Add(0xD416, "Filter Cutoff Hi");
            Add(0xD417, "Filter Routing/Res");
            Add(0xD418, "Filter Mode/Volume");

            _sid.RegisterChanged += OnRegisterChanged;
        }

        private void Add(int addr, string name) => Registers.Add(new SidRegisterRow(addr, name));

        private void OnRegisterChanged(int index, byte value)
        {
            int address = 0xD400 + index;
            var row = Registers.FirstOrDefault(r => r.Address == address);
            if (row != null)
                row.Value = value;
        }
    }
}
