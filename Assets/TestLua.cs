using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

[LuaCallCSharp]
public class TestLua : MonoBehaviour
{
    public Text output;
    private LuaEnv luaEnv;
    // Start is called before the first frame update
    void Start()
    {
        luaEnv = new LuaEnv();
        Test1();
        Test2();
        Test3();
        luaEnv.Dispose();
    }

    private void Test1()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        luaEnv.DoString(@"

        for i = 0, 1000000, 1 do

        local a = CS.UnityEngine.Vector3(1,2,3)

        local b = CS.UnityEngine.Vector3(4,5,6)

        local c = a + b

        end

        ");
        sw.Stop();
        output.text = "lua test1:" + sw.ElapsedMilliseconds + "\n";
    }

    private void Test2()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        luaEnv.DoString(@"

        local c = function(o, x)
            local r = o + x
        end

        for i = 0, 1000000, 1 do

        local a = CS.UnityEngine.Vector3(1,2,3)

        local b = CS.UnityEngine.Vector3(4,5,6)

        c(a,b)

        end

        ");
        sw.Stop();

        output.text += "lua test2(luacalllocal):" + sw.ElapsedMilliseconds + "\n";

        sw.Reset();
        sw.Start();

        luaEnv.DoString(@"

        local c = CS.LuaCallTest
        local co = c()

        for i = 0, 1000000, 1 do

        local a = CS.UnityEngine.Vector3(1,2,3)

        local b = CS.UnityEngine.Vector3(4,5,6)

        co:LuaAdd(a, b)

        end

        ");
        sw.Stop();

        output.text += "lua test2(luacallcs):" + sw.ElapsedMilliseconds + "\n";
    }

    private void Test3()
    {
        //int[] data = Generate();
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        /*
        int result = 0;
        for(int i = 0; i < 10000000; i++)
        {
            result += Test(1,2,4);
        }*/

        
        luaEnv.DoString(@"
math.randomseed(os.time())
local t = {}
for i = 0, 10000, 1 do
    table.insert(t, math.random(0,10000))
end

print(#t)
function qsort (t, lo, hi)
    if lo > hi then
        return
    end
    local p = lo
    for i=lo+1, hi do
        if (t[i] < t[lo]) then
            p = p + 1
            t[p], t[i] = t[i], t[p]
        end
    end
        t[p], t[lo] = t[lo], t[p]
        qsort(t, lo, p-1)
        qsort(t, p+1, hi)
end
qsort(t,1,#t)

        ");
        //QSort(data, 0, data.Length - 1);
        sw.Stop();

        output.text += "lua test3:" + sw.ElapsedMilliseconds;
    }

    private int Test(int a, int b, int d)
    {
        return (a + b) / b * a;
    }

    private static int[] Generate()
    {
        const int length = 10000;
        System.Random r = new System.Random();
        List<int> ary = new List<int>(length);
        for (int i = 0; i < length; i++)
        {
            ary.Add(r.Next(length));
        }
        return ary.ToArray();
    }

    private static void QSort(int[] data, int low, int high)
    {
        // 要素数が1以下の領域ができた場合、その領域は確定とする。
        if (high - low < 1)
            return;

        // 1. ピボットとして一つ選びそれをPとする。
        int P = data[(low + high + 1) / 2];

        // 2. 左から順に値を調べ、P以上のものを見つけたらその位置をiとする。
        // 3. 右から順に値を調べ、P以下のものを見つけたらその位置をjとする。
        // 4. iがjより左にあるのならばその二つの位置を入れ替え、2に戻る。ただし、次の2での探索はiの一つ右、次の3での探索はjの一つ左から行う。
        int i = low - 1;
        int j = high + 1;
        while (Scan(data, P, ref i, ref j))
        {
            Swap(data, i, j);
        }

        // 5. 2に戻らなかった場合、iの左側を境界に分割を行って2つの領域に分け、そのそれぞれに対して再帰的に1からの手順を行う。
        QSort(data, low, i - 1);
        QSort(data, i, high);
    }

    private static bool Scan(int[] data, int P, ref int i, ref int j)
    {
        while (data[++i] < P) ;
        while (data[--j] > P) ;
        return i < j;
    }

    private static void Swap(int[] data, int i, int j)
    {
        int temp = data[i];
        data[i] = data[j];
        data[j] = temp;
    }

}


[LuaCallCSharp]
public class LuaCallTest
{
    public void LuaAdd(Vector3 a, Vector3 b)
    {
        Vector3 c = a + b;
    }
}
