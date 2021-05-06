
namespace osu.Game.Rulesets.Mvis.Extensions
{
    public static class MathExtensions
    {
        public static float Map(float value, float minValue, float maxValue, float minEndValue, float maxEndValue)
        {
            return (value - minValue) / (maxValue - minValue) * (maxEndValue - minEndValue) + minEndValue;
        }

        public static double Map(double value, double minValue, double maxValue, double minEndValue, double maxEndValue)
        {
            return (value - minValue) / (maxValue - minValue) * (maxEndValue - minEndValue) + minEndValue;
        }
    }
}
