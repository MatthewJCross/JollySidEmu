Initial creation of a C64 SID emulation

Understanding the SID basics

The C64 SID has 3 voices, ADSR envelopes, and a few registers mapped to memory.

SID base addresses: $D400 – $D418 (hex) → 53248 – 53280 decimal.

Registers (29 in total), each corresponds to a POKE-able byte:

Register	Purpose

0–1	Voice 1 Frequency Low/High

2	Voice 1 Pulse Width Low

3	Voice 1 Pulse Width High

4	Voice 1 Control

5	Voice 1 Attack/Decay

6	Voice 1 Sustain/Release

7–12	Voice 2, same as above

13–18	Voice 3, same as above

19	Filter cutoff low

20	Filter cutoff high

21	Filter resonance & mode

22	Filter enable

23	Volume / Control

