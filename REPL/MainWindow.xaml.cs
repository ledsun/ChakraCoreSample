using ChakraHost.Hosting;
using Reactive.Bindings;
using System.Windows;

namespace REPL
{

    public class ViewModel
    {
        public ReactiveProperty<string> Script { get; } = new ReactiveProperty<string>("(()=>{return \'Hello world!\';})()");
        public ReactiveProperty<string> Result { get; } = new ReactiveProperty<string>();
    }

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private JavaScriptRuntime runtime;
        private ViewModel vm;

        public MainWindow()
        {
            InitializeComponent();

            vm = new ViewModel();
            DataContext = vm;

            InitChakraRuntime();
            Run(runtime, vm);
        }

        private void InitChakraRuntime()
        {
            runtime = JavaScriptRuntime.Create();
            JavaScriptContext.Current = runtime.CreateContext(); 
        }

        private static void Run(JavaScriptRuntime runtime, ViewModel vm)
        {
            try
            {
                vm.Result.Value = JavaScriptContext.RunScript(vm.Script.Value)
                    .ConvertToString()
                    .ToString();
            }
            catch (JavaScriptScriptException e)
            {
                vm.Result.Value = e.Error
                    .ConvertToString()
                    .ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Run(runtime, vm);
        }

        private void ResetContext(object sender, RoutedEventArgs e)
        {
            JavaScriptContext.Current = runtime.CreateContext();
        }

        private void DisposeChakraRuntime(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Dispose runtime
            JavaScriptContext.Current = JavaScriptContext.Invalid;
            runtime.Dispose();
        }
    }
}
