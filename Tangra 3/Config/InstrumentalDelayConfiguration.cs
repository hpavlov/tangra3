using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tangra.Config
{
    public abstract class InstrumentalDelayConfiguration
    {        
        public abstract void Serialize(BinaryWriter writer);

        public abstract float? CalculateDelay(int integratedFrames, DelayRequest request);
    }

    public class NotSupportedInstrumentalDelayConfiguration : InstrumentalDelayConfiguration
    {
        internal const int TYPE = 0;

        public override void Serialize(BinaryWriter writer)
        { }

        public override float? CalculateDelay(int integratedFrames, DelayRequest request)
        {
            return null;
        }
    }

    public class FixedDelayInstrumentalDelayConfiguration : InstrumentalDelayConfiguration
    {
        internal const int TYPE = 1;
        private static int CURRENT_VERSION = 0;

        private Dictionary<int, float> m_Delays = new Dictionary<int, float>();

        public FixedDelayInstrumentalDelayConfiguration(BinaryReader reader)
        {
            var version = reader.ReadInt32();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int intFrames = reader.ReadInt32();
                float delay = reader.ReadSingle();
                m_Delays.Add(intFrames, delay);
            }
        }

        public FixedDelayInstrumentalDelayConfiguration(Dictionary<int, float> delays)
        {
            foreach(var kvp in delays)
                m_Delays.Add(kvp.Key, kvp.Value);
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(CURRENT_VERSION);
            writer.Write(m_Delays.Count);
            foreach (var entry in m_Delays)
            {
                writer.Write(entry.Key);
                writer.Write(entry.Value);
            }
        }

        public override float? CalculateDelay(int integratedFrames, DelayRequest request)
        {
            float delay;
            if (m_Delays.TryGetValue(integratedFrames, out delay))
                return delay;

            return null;
        }
    }

    public class ImageOffsetInstrumentalDelayConfiguration : InstrumentalDelayConfiguration
    {
        internal const int TYPE = 2;
        private static int CURRENT_VERSION = 0;

        private Dictionary<int, Tuple<float, float>> m_DelayData = new Dictionary<int, Tuple<float, float>>();

        public ImageOffsetInstrumentalDelayConfiguration(Dictionary<int, Tuple<float, float>> delayData)
        {
            foreach (var kvp in delayData)
                m_DelayData.Add(kvp.Key, kvp.Value);
        }

        public ImageOffsetInstrumentalDelayConfiguration(BinaryReader reader)
        {
            var version = reader.ReadInt32();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int intFrames = reader.ReadInt32();
                float param1 = reader.ReadSingle();
                float param2 = reader.ReadSingle();
                m_DelayData.Add(intFrames, Tuple.Create(param1, param2));
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(CURRENT_VERSION);
            writer.Write(m_DelayData.Count);
            foreach (var entry in m_DelayData)
            {
                writer.Write(entry.Key);
                writer.Write(entry.Value.Item1);
                writer.Write(entry.Value.Item2);
            }
        }

        public override float? CalculateDelay(int integratedFrames, DelayRequest request)
        {
            Tuple<float, float> delay;
            if (m_DelayData.TryGetValue(integratedFrames, out delay))
                return -1 * (delay.Item1 * request.YPosRatio + delay.Item2);

            return null;
        }
    }

    public class InstrumentalDelayConfigurationFactory
    {
        private static int CURRENT_VERSION = 0;

        public static InstrumentalDelayConfiguration Deserialize(BinaryReader reader)
        {
            var version = reader.ReadInt32();

            var type = reader.ReadInt32();
            switch (type)
            {
                case NotSupportedInstrumentalDelayConfiguration.TYPE:
                    return new NotSupportedInstrumentalDelayConfiguration();

                case FixedDelayInstrumentalDelayConfiguration.TYPE:
                    return new FixedDelayInstrumentalDelayConfiguration(reader);

                case ImageOffsetInstrumentalDelayConfiguration.TYPE:
                    return new ImageOffsetInstrumentalDelayConfiguration(reader);

                default:
                    Trace.WriteLine("Unsupported InstrumentalDelayConfiguration type: " + type);
                    return null;
            }
        }

        public static void Serialize(InstrumentalDelayConfiguration config, BinaryWriter writer)
        {
            if (config == null) 
                config = new NotSupportedInstrumentalDelayConfiguration();

            writer.Write(CURRENT_VERSION);
            int type = -1;
            if (config is NotSupportedInstrumentalDelayConfiguration)
                type = NotSupportedInstrumentalDelayConfiguration.TYPE;
            else if (config is FixedDelayInstrumentalDelayConfiguration)
                type = FixedDelayInstrumentalDelayConfiguration.TYPE;
            else if (config is ImageOffsetInstrumentalDelayConfiguration)
                type = ImageOffsetInstrumentalDelayConfiguration.TYPE;

            if (type == -1)
                throw new NotSupportedException();

            writer.Write(type);
            config.Serialize(writer);
        }
    }
}
