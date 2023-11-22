
public class NewtonMethod
{
    public delegate float Func(float x);

    public static float run(Func func, Func derive, float initX, int maxLoop)
    {
        float x = initX;
        for (int i = 0; i < maxLoop; i++)
        {
            float curY = func(x);
            if (curY < 0.00001f && curY > -0.00001f)
                break;
            x = x - curY / derive(x);

        }
        return x;
    }
}
