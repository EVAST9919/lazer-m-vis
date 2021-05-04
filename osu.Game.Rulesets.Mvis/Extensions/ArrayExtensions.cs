namespace osu.Game.Rulesets.Mvis.Extensions
{
    public static class ArrayExtensions
    {
        public static void Smooth(this float[] src, int severity = 1)
        {
            for (int i = 0; i < src.Length; i++)
            {
                var start = i - severity > 0 ? i - severity : 0;
                var end = i + severity < src.Length ? i + severity : src.Length;

                float sum = 0;

                for (int j = start; j < end; j++)
                    sum += src[j];

                src[i] = sum / (end - start);
            }
        }
    }
}
