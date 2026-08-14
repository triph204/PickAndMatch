namespace PickAndMatch.Gameplay.Score
{
    public static class StarCalculator
    {
        // normalizedTimeRemaining: 0..1 (lấy từ GameTimer.NormalizedTime lúc thắng).
        // Còn nhiều thời gian -> nhiều sao. Thắng thì luôn được ít nhất 1 sao.
        public static int Calculate(float normalizedTimeRemaining)
        {
            if (normalizedTimeRemaining >= 0.5f)
                return 3;

            if (normalizedTimeRemaining >= 0.2f)
                return 2;

            return 1;
        }
    }
}