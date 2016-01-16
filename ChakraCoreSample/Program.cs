using ChakraHost.Hosting;
using System;

namespace hoge
{
    class Program
    {
        // 実行するスクリプト
        const string script = @"
                class Greeter {
                    hello() { return 'コンニチハ!'; }
                }
                new Greeter().hello();
            ";

        static void Main(string[] args)
        {
            Do();
            Console.ReadLine();
        }

        private static void Do()
        {
            // ランタイムを作る
            JavaScriptRuntime runtime;
            Native.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out runtime);

            using (runtime) {
                // 実行コンテキストを作る
                var context = runtime.CreateContext();

                // 現在のスレッドの実行コンテキストをセットする
                Native.JsSetCurrentContext(context);

                // スクリプトのソースの位置を記録するためのコンテキスト
                var currentSourceContext = JavaScriptSourceContext.FromIntPtr(IntPtr.Zero);

                // スクリプトを実行する
                JavaScriptValue result;
                Native.JsRunScript(script, currentSourceContext++, "", out result);

                // 戻り値をJavaScriptの値からCLRのStringに変換する
                // ちなみにConvertToStringメソッドはJavaScriptのStringなので注意。
                var resultString = result.ToString();

                // 出力
                Console.WriteLine(resultString);

                // 後片付け
                Native.JsSetCurrentContext(JavaScriptContext.Invalid);
            }
        }
    }
}