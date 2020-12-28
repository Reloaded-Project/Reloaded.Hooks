using System;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Tests.Shared;
using Reloaded.Hooks.Tools;
using Xunit;

namespace Reloaded.Hooks.Tests.X64
{
    public class VTableTest : IDisposable
    {
        private NativeCalculator _nativeCalculator;

        private IHook<NativeCalculator.AddFunction> _addHook;
        private IHook<NativeCalculator.SubtractFunction> _subHook;
        private IHook<NativeCalculator.DivideFunction> _divideHook;
        private IHook<NativeCalculator.MultiplyFunction> _multiplyHook;

        public VTableTest()
        {
            _nativeCalculator = new NativeCalculator();
        }

        public void Dispose()
        {
            _nativeCalculator?.Dispose();
        }

        [Fact]
        public void TestFunctionPtr()
        {
            // Note: All delegates are the same; this test will need to be changed if they ever change.
            FunctionPtr<NativeCalculator.AddFunction> functionPtr = new FunctionPtr<NativeCalculator.AddFunction>((ulong) _nativeCalculator.VTable);

            for (int x = 1; x < 100; x++)
            {
                for (int y = 100; y > 0; y--)
                {
                    int add = x + y;
                    int subtract = x - y;
                    int multiply = x * y;
                    int divide = x / y;

                    Assert.Equal(add, functionPtr[(int) NativeCalculator.VTableFunctions.Add](x, y));
                    Assert.Equal(subtract, functionPtr[(int) NativeCalculator.VTableFunctions.Subtract](x, y));
                    Assert.Equal(multiply, functionPtr[(int) NativeCalculator.VTableFunctions.Multiply](x, y));
                    Assert.Equal(divide, functionPtr[(int) NativeCalculator.VTableFunctions.Divide](x, y));
                }
            }
        }

        [Fact]
        public void TestVTableCall()
        {
            var vTable = VirtualFunctionTable.FromAddress(_nativeCalculator.VTable, Enum.GetNames(typeof(NativeCalculator.VTableFunctions)).Length);

            // Setup calling functions.
            var addFunction = vTable.CreateWrapperFunction<NativeCalculator.AddFunction>((int) NativeCalculator.VTableFunctions.Add);
            var subtractFunction = vTable.CreateWrapperFunction<NativeCalculator.SubtractFunction>((int) NativeCalculator.VTableFunctions.Subtract);
            var multiplyFunction = vTable.CreateWrapperFunction<NativeCalculator.MultiplyFunction>((int) NativeCalculator.VTableFunctions.Multiply);
            var divideFunction = vTable.CreateWrapperFunction<NativeCalculator.DivideFunction>((int) NativeCalculator.VTableFunctions.Divide);

            // Test Calling
            for (int x = 1; x < 100; x++)
            {
                for (int y = 100; y > 0; y--)
                {
                    int add = x + y;
                    int subtract = x - y;
                    int multiply = x * y;
                    int divide = x / y;

                    Assert.Equal(add, addFunction(x, y));
                    Assert.Equal(subtract, subtractFunction(x, y));
                    Assert.Equal(multiply, multiplyFunction(x, y));
                    Assert.Equal(divide, divideFunction(x, y));
                }
            }
        }

        [Fact]
        public void TestVTableHook()
        {
            var vTable = VirtualFunctionTable.FromAddress(_nativeCalculator.VTable, Enum.GetNames(typeof(NativeCalculator.VTableFunctions)).Length);
            
            var addFunction = vTable.CreateWrapperFunction<NativeCalculator.AddFunction>((int)NativeCalculator.VTableFunctions.Add);
            var subtractFunction = vTable.CreateWrapperFunction<NativeCalculator.SubtractFunction>((int)NativeCalculator.VTableFunctions.Subtract);
            var multiplyFunction = vTable.CreateWrapperFunction<NativeCalculator.MultiplyFunction>((int)NativeCalculator.VTableFunctions.Multiply);
            var divideFunction = vTable.CreateWrapperFunction<NativeCalculator.DivideFunction>((int)NativeCalculator.VTableFunctions.Divide);

            int addHook(int a, int b) { return _addHook.OriginalFunction(a, b) + 1; }
            int subHook(int a, int b) { return _subHook.OriginalFunction(a, b) - 1; }
            int mulHook(int a, int b) { return _multiplyHook.OriginalFunction(a, b) * 2; }
            int divHook(int a, int b) { return _divideHook.OriginalFunction(a, b) * 2; }

            _addHook = vTable.CreateFunctionHook<NativeCalculator.AddFunction>((int)NativeCalculator.VTableFunctions.Add, addHook).Activate();
            _subHook = vTable.CreateFunctionHook<NativeCalculator.SubtractFunction>((int)NativeCalculator.VTableFunctions.Subtract, subHook).Activate();
            _multiplyHook = vTable.CreateFunctionHook<NativeCalculator.MultiplyFunction>((int)NativeCalculator.VTableFunctions.Multiply, mulHook).Activate();
            _divideHook = vTable.CreateFunctionHook<NativeCalculator.DivideFunction>((int)NativeCalculator.VTableFunctions.Divide, divHook).Activate();

            for (int x = 1; x < 100; x++)
            {
                for (int y = 100; y > 0; y--)
                {
                    int add = (x + y) + 1;
                    int subtract = (x - y) - 1;
                    int multiply = (x * y) * 2;
                    int divide = (x / y) * 2;

                    Assert.Equal(add, addFunction(x, y));
                    Assert.Equal(subtract, subtractFunction(x, y));
                    Assert.Equal(multiply, multiplyFunction(x, y));
                    Assert.Equal(divide, divideFunction(x, y));
                }
            }
        }
    }
}
