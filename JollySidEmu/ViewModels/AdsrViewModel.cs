using JollySidEmu.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JollySidEmu.ViewModels
{
    public class AdsrViewModel : ViewModelBase
    {
        private readonly C64Memory _memory;
        private readonly int _baseAddress;

        public AdsrViewModel(C64Memory memory, int baseAddress)
        {
            _memory = memory;
            _baseAddress = baseAddress;
            Attack = 0;
            Decay = 0;
            Sustain = 0;
            Release = 0;
        }

        // ---------- Attack (0–15) ----------
        private int _attack;
        public int Attack
        {
            get => _attack;
            set
            {
                _attack = Clamp4Bit(value);
                OnPropertyChanged();
                WriteAttackDecay();
            }
        }

        // ---------- Decay (0–15) ----------
        private int _decay;
        public int Decay
        {
            get => _decay;
            set
            {
                _decay = Clamp4Bit(value);
                OnPropertyChanged();
                WriteAttackDecay();
            }
        }

        // ---------- Sustain (0–15) ----------
        private int _sustain;
        public int Sustain
        {
            get => _sustain;
            set
            {
                _sustain = Clamp4Bit(value);
                OnPropertyChanged();
                WriteSustainRelease();
            }
        }

        // ---------- Release (0–15) ----------
        private int _release;
        public int Release
        {
            get => _release;
            set
            {
                _release = Clamp4Bit(value);
                OnPropertyChanged();
                WriteSustainRelease();
            }
        }

        // ---------- Register Writes ----------

        private void WriteAttackDecay()
        {
            byte value = (byte)((_attack << 4) | _decay);
            _memory.Poke(_baseAddress + 5, value);
        }

        private void WriteSustainRelease()
        {
            byte value = (byte)((_sustain << 4) | _release);
            _memory.Poke(_baseAddress + 6, value);
        }

        private static int Clamp4Bit(int value) => value < 0 ? 0 : value > 15 ? 15 : value;
    }
}

