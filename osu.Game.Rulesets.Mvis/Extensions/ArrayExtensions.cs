namespace osu.Game.Rulesets.Mvis.Extensions
{
    public static class ArrayExtensions
    {
        public static void Smooth(this float[] src)
        {
            var amps = new float[src.Length];

            for (int i = 0; i < src.Length; i++)
            {
                if (i == 0)
                {
                    amps[i] = src[i];
                    continue;
                }

                var nextAmp = i == src.Length - 1 ? 0 : src[i + 1];

                amps[i] = (amps[i - 1] + src[i] + nextAmp) / 3f;
            }

            for (int i = 0; i < src.Length; i++)
                src[i] = amps[i];
        }
    }
}
