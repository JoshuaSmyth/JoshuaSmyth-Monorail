
using Monorail.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Monorail.Audio
{
    public class AudioDevice : IPlatformAudioDevice
    {
        // TODO:(Joshua)
        // We probably want better error handling

        IntPtr alDevice;
        IntPtr alContext;

        List<uint> m_Buffers = new List<uint>();    // TODO:(Joshua) Make fixed size array
                                                    // TODO:(Joshua) Want a list of sources

        // TODO force;
        [System.Diagnostics.Conditional("DEBUG")]
        internal static void CheckError(string message, IntPtr device, params object[] args)
        {
            int error;
            if ((error = OpenAL.ALC10.alcGetError(device)) != 0)
            {
                if (args != null && args.Length > 0)
                    message = String.Format(message, args);

                throw new InvalidOperationException(message + " (Reason: " + error.ToString() + ")");
            }
        }

        public void Initalise()
        {
            
            alDevice = OpenAL.ALC10.alcOpenDevice(String.Empty);
            CheckError("Could not Create Open AL Device", alDevice);

            int[] attribute = new int[0];
            alContext = OpenAL.ALC10.alcCreateContext(alDevice, attribute);
            CheckError("Could not Create Open AL Context", alDevice);

            OpenAL.ALC10.alcMakeContextCurrent(alContext);
            CheckError("Could not make OpenAL context current", alDevice);

            // Device properties
            var SupportsEfx = OpenAL.AL10.alIsExtensionPresent("AL_EXT_EFX");
            Console.WriteLine("Supportes EFX:" + SupportsEfx);

            SetListener();
            LoadSound();
            
        }

        public void LoadSound()
        {
            uint buffer = 0;
            //uint buffer2 = 0;
            OpenAL.AL10.alGenBuffers(1, out buffer);
            //OpenAL.AL10.alGenBuffers(1, out buffer2);

            CheckError("Could not GenBuffer", alDevice);

            int error = OpenAL.AL10.alGetError();
            
            var waveFile = new WaveFileData();
            waveFile.LoadFromFile("Resources/Audio/bounce.wav");
            
            //var handle = GCHandle.Alloc(waveFile.Data, GCHandleType.Pinned);
            OpenAL.AL10.alBufferData(buffer, (int)waveFile.SoundFormat1, waveFile.Data, waveFile.Data.Length, (int) waveFile.SampleRate);
            //handle.Free();

            CheckError("Could not alBufferData", alDevice);
            m_Buffers.Add(buffer);

            var reverb = AudioEffectPreset.SewerPipe;

            // https://github.com/kcat/openal-soft/blob/073829f26a4a509d11de5375d43169a8f6ba9e12/examples/alreverb.c
            uint effect;
            OpenAL.EFX.alGenEffects(1, out effect);
            CheckError("Could not generate effect",alDevice);
            if (OpenAL.AL10.alGetEnumValue("AL_EFFECT_EAXREVERB") != 0)
            {
                Console.WriteLine("EAX Reverb");
                OpenAL.EFX.alEffecti(effect, OpenAL.EFX.AL_EFFECT_TYPE, OpenAL.EFX.AL_EFFECT_EAXREVERB);

                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_DENSITY, reverb.flDensity);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_DIFFUSION, reverb.flDiffusion);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_GAIN, reverb.flGain);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_GAINHF, reverb.flGainHF);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_GAINLF, reverb.flGainLF);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_DECAY_TIME, reverb.flDecayTime);

                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_DECAY_HFRATIO, reverb.flDecayHFRatio);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_DECAY_LFRATIO, reverb.flDecayLFRatio);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_REFLECTIONS_DELAY, reverb.flReflectionsDelay);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_REFLECTIONS_GAIN, reverb.flReflectionsGain);
                OpenAL.EFX.alEffectfv(effect, OpenAL.EFX.AL_EAXREVERB_REFLECTIONS_PAN, new float[] { reverb.flReflectionsPan.f1, reverb.flReflectionsPan.f2, reverb.flReflectionsPan.f3 });
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_LATE_REVERB_GAIN, reverb.flLateReverbGain);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_LATE_REVERB_DELAY, reverb.flLateReverbDelay);
                OpenAL.EFX.alEffectfv(effect, OpenAL.EFX.AL_EAXREVERB_LATE_REVERT_PAN, new float[] { reverb.flLateReverbPan.f1, reverb.flLateReverbPan.f2, reverb.flLateReverbPan.f3 });

                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_ECHO_TIME, reverb.flEchoTime);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_ECHO_DEPTH, reverb.flEchoDepth);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_MODULATION_TIME, reverb.flModulationTime);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_MODULATION_DEPTH, reverb.flModulationDepth);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_AIR_ABSORPTION_GAINHF, reverb.flAirAbsorptionGainHF);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_HFREFERENCE, reverb.flHFReference);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_LFREFERENCE, reverb.flLFReference);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_ROOM_ROLLOFF_FACTOR, reverb.flRoomRolloffFactor);
                OpenAL.EFX.alEffectf(effect, OpenAL.EFX.AL_EAXREVERB_DECAY_HFLIMIT, reverb.iDecayHFLimit);

            }
            else {
                // TODO:(Joshua) Implement
                // See  // https://github.com/kcat/openal-soft/blob/073829f26a4a509d11de5375d43169a8f6ba9e12/examples/alreverb.c
                // Use Standard Reverb
                Console.WriteLine("Standard Reverb");
            }

            // Generate an effect slot and put the effect in the slot
            uint effectSlotId;
            OpenAL.EFX.alGenAuxiliaryEffectSlots(1, out effectSlotId);
            OpenAL.EFX.alAuxiliaryEffectSloti(effectSlotId, OpenAL.EFX.AL_EFFECTSLOT_EFFECT, (int) effect);


            // TODO:(Joshua) Put sources into a sources list
            // Generate source
            uint sourceId;
            OpenAL.AL10.alGenSources(1, out sourceId);
            OpenAL.AL10.alSourcef(sourceId, OpenAL.AL10.AL_GAIN, 1.0f);
            OpenAL.AL10.alSourcef(sourceId, OpenAL.AL10.AL_PITCH, 1.0f);
            OpenAL.AL10.alSource3f(sourceId, OpenAL.AL10.AL_POSITION, 0.0f, 0.0f, 0.0f);

            // Connect the source to the effect slot
            OpenAL.AL10.alSource3i(sourceId, OpenAL.EFX.AL_AUXILIARY_SEND_FILTER, (int)effectSlotId , 0, OpenAL.EFX.AL_FILTER_NULL);

            // Unbind with this call
            //OpenAL.AL10.alSource3i(sourceId, OpenAL.EFX.AL_AUXILIARY_SEND_FILTER, 0, 0, OpenAL.EFX.AL_FILTER_NULL);

            OpenAL.AL10.alSourcei(sourceId, OpenAL.AL10.AL_BUFFER, (int) buffer);
            OpenAL.AL10.alSourcePlay(sourceId);
        }

        public void SetListener()
        {
            OpenAL.AL10.alListener3f(OpenAL.AL10.AL_POSITION,0, 0, 0);
            OpenAL.AL10.alListener3f(OpenAL.AL10.AL_VELOCITY, 0, 0, 0);
        }

        public void PlaySound()
        {
            // TODO Pass a lookup

        }

        public void Unload()
        {
            // TODO:(Joshua)
            // Perform this in one call
            for(int i=0;i<m_Buffers.Count;i++)
            {
                var b = m_Buffers[i];
                OpenAL.AL10.alDeleteBuffers(1, ref b);
            }

            OpenAL.ALC10.alcDestroyContext(alContext);
            OpenAL.ALC10.alcCloseDevice(alDevice);

        }
    }
}
