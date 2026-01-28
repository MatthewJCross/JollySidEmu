# Understanding the C64 SID

The **SID (Sound Interface Device)** in the Commodore 64 has:

- 3 voices  
- ADSR envelopes (Attack, Decay, Sustain, Release)  
- Registers mapped to memory that can be controlled via `POKE` commands  

---

## SID Base Addresses

- Hex: `$D400 – $D418`  
- Decimal: `53248 – 53280`  

This range contains **29 registers**, each 1 byte wide.

---

## SID Register Map

| Register | Purpose |
|----------|---------|
| 0–1      | Voice 1 Frequency Low / High |
| 2        | Voice 1 Pulse Width Low |
| 3        | Voice 1 Pulse Width High |
| 4        | Voice 1 Control (Waveform, Gate, Sync, Ring Mod) |
| 5        | Voice 1 Attack / Decay |
| 6        | Voice 1 Sustain / Release |
| 7–12     | Voice 2 (Frequency, Pulse, Control, ADSR) |
| 13–18    | Voice 3 (Frequency, Pulse, Control, ADSR) |
| 19       | Filter Cutoff Low |
| 20       | Filter Cutoff High |
| 21       | Filter Resonance & Mode |
| 22       | Filter Enable |
| 23       | Volume / Control |

---

### Notes

- Frequency registers are **16-bit** (Low + High byte)  
- Pulse width is **12-bit** (Low + High byte)  
- Control register bits determine waveform, gate, sync, ring modulation  
- Filter registers allow per-voice filtering and volume control  


# C64 SID Overview (Registers & Voices)

$D400–$D406 : Voice 1
+---------+---------+---------+---------+---------+---------+
| FREQLO | FREQHI | PWLO | PWHI | CONTROL | ADSR |
| 0 | 1 | 2 | 3 | 4 | 5/6 |
+---------+---------+---------+---------+---------+---------+
16-bit freq 12-bit pulse Control ADSR
(Low + High) (Gate/Wave) Attack/Decay + Sustain/Release

$D407–$D40D : Voice 2
+---------+---------+---------+---------+---------+---------+
| FREQLO | FREQHI | PWLO | PWHI | CONTROL | ADSR |
| 7 | 8 | 9 | 10 | 11 | 12/13 |
+---------+---------+---------+---------+---------+---------+

$D40E–$D414 : Voice 3
+---------+---------+---------+---------+---------+---------+
| FREQLO | FREQHI | PWLO | PWHI | CONTROL | ADSR |
| 14 | 15 | 16 | 17 | 18 | 19/20 |
+---------+---------+---------+---------+---------+---------+

$D415–$D418 : Filter & Volume
+----------------+----------------+----------------+---------+
| CUTOFF LO | CUTOFF HI | RES/MODE | ENABLE/ |
| 21 | 22 | 23 | 24 |
+----------------+----------------+----------------+---------+
Volume: lower 4 bits of last register

### Legend

- **FREQLO / FREQHI** → 16-bit frequency  
- **PWLO / PWHI** → 12-bit pulse width  
- **CONTROL** → Gate / Sync / Ring / Waveform bits  
- **ADSR** → Attack / Decay / Sustain / Release (2 registers)  
- **Filter** → Cutoff, Resonance, Filter Mode  
- **Volume** → 4-bit master volume  

# C64 SID Signal Path (ASCII Diagram)
+----------------------------+
Voice 1 --> | SID VOICE 1 |
| FREQ LO $D400 |
| FREQ HI $D401 |
| PW LO $D402 |
| PW HI $D403 |
| CONTROL $D404 (Gate/Wave/Sync/Ring) |
| ADSR $D405/$D406 (A/D + S/R) |
+----------------------------+
|
+----------------------------+
Voice 2 --> | SID VOICE 2 |
| FREQ LO $D407 |
| FREQ HI $D408 |
| PW LO $D409 |
| PW HI $D40A |
| CONTROL $D40B |
| ADSR $D40C/$D40D |
+----------------------------+
|
+----------------------------+
Voice 3 --> | SID VOICE 3 |
| FREQ LO $D40E |
| FREQ HI $D40F |
| PW LO $D410 |
| PW HI $D411 |
| CONTROL $D412 |
| ADSR $D413/$D414 |
+----------------------------+
|
v
+--------------+
| FILTER |
| Cutoff LO $D415 |
| Cutoff HI $D416 |
| Reson/Mode $D417 |
| Enable $D418 |
+--------------+
|
v
+--------------+
| Master Volume|
| Lower 4 bits of $D418 |
+--------------+
|
v
Output

### Notes

- **Voices**: each 6 registers for frequency, pulse width, control, ADSR  
- **Filter**: can selectively affect voices (voice enable bits in $D418)  
- **Volume**: 4-bit master volume in last register $D418  
- **Control bits**: waveform, gate, sync, ring modulation  
- All addresses are **hex ($D400-$D418)**

---

# C64 SID Registers Overview (Compact)

$D400 $D401 $D402 $D403 $D404 $D405 $D406 Voice 1
FREQ FREQ PW PW CTRL ADSR ADSR
LO HI LO HI A/D S/R
| | | | | | |
v v v v v v v
+-----------------------------------------------+
| Voice 1 Signal |
+-----------------------------------------------+

$D407 $D408 $D409 $D40A $D40B $D40C $D40D Voice 2
FREQ FREQ PW PW CTRL ADSR ADSR
LO HI LO HI A/D S/R
| | | | | | |
v v v v v v v
+-----------------------------------------------+
| Voice 2 Signal |
+-----------------------------------------------+

$D40E $D40F $D410 $D411 $D412 $D413 $D414 Voice 3
FREQ FREQ PW PW CTRL ADSR ADSR
LO HI LO HI A/D S/R
| | | | | | v
+------+-----+-----+-----+-----+-----+
|
v
+-----------+
$D415 $D416 $D417 $D418 Filter & Volume
CUTOFF CUTOFF RES/MODE ENABLE/VOL
LO HI
|
v
Output

### How to read it

- **Voices 1–3**: 7 registers each  
- **Frequency**: 16-bit (LO + HI)  
- **Pulse width**: 12-bit (LO + HI)  
- **Control**: waveform, gate, sync, ring modulation  
- **ADSR**: Attack/Decay + Sustain/Release  
- **Filter**: cutoff, resonance, mode, enable bits  
- **Volume**: lower 4 bits of $D418  
- **Arrows** indicate signal flow from voice → filter → output  

---
