namespace GameUtils
{
    public static class MathUtils
    {
        public static int Factorial(int num)
        {
            int ans = 1;

            for (int i = 2; i <= num; i++)
                ans *= i;

            return ans;
        }

        public static int C(int n, int m)
        {
            if (n < m) return -1;

            return Factorial(n) / (Factorial(m) * Factorial(n - m));
        }
    }
}